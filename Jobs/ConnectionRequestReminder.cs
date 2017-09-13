using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Quartz;
using Quartz.Util;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;

namespace com.bricksandmortarstudio.TheCrossing.Jobs
{
    [SystemEmailField( "Email", "The System Email to send for each person with open connection requests" )]
    [IntegerField( "Exclude Hours", "Only connection requests older than this many hours will be sent. This prevents new requests being sent as a reminder.", true, 18 )]
    public class ConnectionRequestReminder : IJob
    {
        public ConnectionRequestReminder() { }
        public void Execute( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var dataMap = context.JobDetail.JobDataMap;
            var systemEmailGuid = dataMap.GetString( "Email" ).AsGuidOrNull();

            string appRoot = Rock.Web.Cache.GlobalAttributesCache.Read().GetValue( "PublicApplicationRoot" );

            if ( appRoot.IsNullOrWhiteSpace() )
            {
                throw new Exception( "Couldn't fetch application root!" );
            }

            if ( systemEmailGuid == null )
            {
                throw new Exception( "A system email template needs to be set." );
            }
            var systemEmailTemplate = new SystemEmailService( rockContext ).Get( systemEmailGuid.Value );

            if ( systemEmailTemplate == null )
            {
                throw new Exception( "The system email template setting is not a valid system email template." );
            }

            var cutOffHours = dataMap.GetString( "ExcludeHours" ).AsIntegerOrNull();
            if ( !cutOffHours.HasValue )
            {
                throw new Exception( "A cutoff period needs to be set." );
            }




            var cutoff = RockDateTime.Now.AddHours( -1 * cutOffHours.Value );


            var connectionRequestService = new ConnectionRequestService( rockContext );
            var midnightToday = new DateTime( RockDateTime.Now.Year, RockDateTime.Now.Month, RockDateTime.Now.Day );
            var currentDateTime = RockDateTime.Now;
            var openConnectionRequests =
                connectionRequestService.Queryable()
                                        .AsNoTracking()
                                        .Where( cr => cr.CreatedDateTime < cutoff && ( cr.ConnectionState == ConnectionState.Active || ( cr.ConnectionState == ConnectionState.FutureFollowUp && cr.FollowupDate.HasValue && cr.FollowupDate.Value < midnightToday ) ) );

            if (!openConnectionRequests.Any())
            {
                context.Result = "There are no open and assigned connection requests to send reminders for";
                return;
            }

            int totalCriticalCount = openConnectionRequests.Count( cr => cr.ConnectionStatus.IsCritical );
            int totalIdleCount = openConnectionRequests
                                        .Count( cr =>
                                            ( cr.ConnectionRequestActivities.Any() && cr.ConnectionRequestActivities.Max( ra => ra.CreatedDateTime ) < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )
                                            || ( !cr.ConnectionRequestActivities.Any() && cr.CreatedDateTime < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )
                                        );

            var groupedRequests = openConnectionRequests
                .Where(cr => cr.ConnectorPersonAliasId != null)
                .GroupBy( cr => cr.ConnectorPersonAlias );

            int mailedCount = 0;
            foreach ( var connectionRequestGrouping in groupedRequests )
            {
                var connectionRequests = connectionRequestGrouping.ToList();

                var mergeFields = new Dictionary<string, object>
                            {
                                {"ConnectionRequests", connectionRequests},
                                {"Person", connectionRequestGrouping.Key.Person},
                                {"CriticalCount", connectionRequests.Count(cr => cr.ConnectionStatus.IsCritical) },
                                {"IdleCount", connectionRequests.Count(cr =>
                                {
                                    var idleDate = currentDateTime.AddDays(-cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle);
                                    return cr.ConnectionRequestActivities.Any() &&
                                           cr.ConnectionRequestActivities.Max(ra => ra.CreatedDateTime) < idleDate
                                           || (!cr.ConnectionRequestActivities.Any() && cr.CreatedDateTime < idleDate);
                                }) },
                                {"TotalIdleCount", totalIdleCount },
                                {"TotalCriticalCount", totalCriticalCount }
                            };

                var recipients = new List<string> { connectionRequestGrouping.Key.Person.Email };

                Email.Send( systemEmailTemplate.From.ResolveMergeFields( mergeFields ), systemEmailTemplate.FromName.ResolveMergeFields( mergeFields ), systemEmailTemplate.Subject.ResolveMergeFields( mergeFields ), recipients, systemEmailTemplate.Body.ResolveMergeFields( mergeFields ), appRoot, null, null );
                mailedCount++;
            }
            context.Result = string.Format( "{0} reminders were sent ", mailedCount );
        }
    }
}
