using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;

namespace com.bricksandmortarstudio.TheCrossing.Jobs
{
    [SystemEmailField( "Email", "The System Email to send for each person with open connection requests")]
    public class ConnectionRequestReminder : IJob
    {
        public ConnectionRequestReminder() { }
        public void Execute( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var dataMap = context.JobDetail.JobDataMap;
            var systemEmailGuid = dataMap.GetString( "Email" ).AsGuidOrNull();
            string appRoot = Rock.Web.Cache.GlobalAttributesCache.Read().GetValue( "ExternalApplicationRoot" );
            if ( systemEmailGuid == null )
            {
                throw new Exception( "A system email template needs to be set." );
            }


            var systemEmailTemplate = new SystemEmailService( rockContext ).Get( systemEmailGuid.Value );

            if ( systemEmailTemplate == null )
            {
                throw new Exception( "The system email template setting is not a valid system email template." );
            }


            var connectionRequestService = new ConnectionRequestService( rockContext );
            var midnightToday = new DateTime( RockDateTime.Now.Year, RockDateTime.Now.Month, RockDateTime.Now.Day );
            var currentDateTime = RockDateTime.Now;
            var openConnectionRequests =
                connectionRequestService.Queryable()
                                        .AsNoTracking()
                                        .Where( cr => cr.ConnectionState == ConnectionState.Active || ( cr.ConnectionState == ConnectionState.FutureFollowUp && cr.FollowupDate.HasValue && cr.FollowupDate.Value < midnightToday ) );

            int totalCriticalCount = openConnectionRequests.Count( cr => cr.ConnectionStatus.IsCritical );
            int totalIdleCount = openConnectionRequests
                                        .Count( cr =>
                                            ( cr.ConnectionRequestActivities.Any() && cr.ConnectionRequestActivities.Max( ra => ra.CreatedDateTime ) < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )
                                            || ( !cr.ConnectionRequestActivities.Any() && cr.CreatedDateTime < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )
                                        );

            var groupedRequests = openConnectionRequests
                .ToList()
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
                                {"IdleCount", connectionRequests.Count(cr => ( cr.ConnectionRequestActivities.Any() && cr.ConnectionRequestActivities.Max( ra => ra.CreatedDateTime ) < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )
                                            || ( !cr.ConnectionRequestActivities.Any() && cr.CreatedDateTime < SqlFunctions.DateAdd( "day", -cr.ConnectionOpportunity.ConnectionType.DaysUntilRequestIdle, currentDateTime ) )) },
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
