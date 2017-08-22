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
    [SystemEmailField( "Email", "The System Email to send for each person with new connection requests" )]
    [IntegerField( "Include Hours", "Only connection requests created within the past this many hours will be sent if we cannot determine when the last job was run. This ensures only new requests are sent as a reminder.", true, 18 )]
    public class ConnectionRequestAlert : IJob
    {
        public ConnectionRequestAlert() { }
        public void Execute( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var dataMap = context.JobDetail.JobDataMap;
            DateTime? lastRun = null;
            if ( context.PreviousFireTimeUtc.HasValue )
            {
                lastRun = RockDateTime.ConvertLocalDateTimeToRockDateTime( context.PreviousFireTimeUtc.Value.LocalDateTime );
            }

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

            var cutOffHours = dataMap.GetString( "IncludeHours" ).AsIntegerOrNull();
            if ( !cutOffHours.HasValue )
            {
                throw new Exception( "A cutoff period needs to be set." );
            }

            if ( lastRun == null )
            {
                lastRun = RockDateTime.Now.AddHours( -1 * cutOffHours.Value );
            }

            var connectionRequestService = new ConnectionRequestService( rockContext );
            var openConnectionRequests =
                connectionRequestService.Queryable()
                                        .AsNoTracking()
                                        .Where( cr => cr.CreatedDateTime >= lastRun && cr.ConnectionState != ConnectionState.Connected );

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
                                {"Person", connectionRequestGrouping.Key.Person}
                            };

                var recipients = new List<string> { connectionRequestGrouping.Key.Person.Email };

                Email.Send( systemEmailTemplate.From.ResolveMergeFields( mergeFields ), systemEmailTemplate.FromName.ResolveMergeFields( mergeFields ), systemEmailTemplate.Subject.ResolveMergeFields( mergeFields ), recipients, systemEmailTemplate.Body.ResolveMergeFields( mergeFields ), appRoot, null, null );
                mailedCount++;
            }
            context.Result = string.Format( "{0} reminders were sent ", mailedCount );
        }
    }
}
