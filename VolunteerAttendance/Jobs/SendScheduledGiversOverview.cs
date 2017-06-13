using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;

namespace com.bricksandmortarstudio.TheCrossing.VolunteerAttendance.Jobs
{
    /// <summary>
    /// Job to run quick SQL queries on a schedule
    /// </summary>
    [SystemEmailField("Email", "The email to send for each found ScheduledTransaction", true )]
    [IntegerField("Previous Minutes", "The number of minutes prior to the current time to search for new ScheduledTransactions", true, 60)]
    [GroupField("Group", "The group to receive the email", true)]
    [DisallowConcurrentExecution]
    class SendScheduledGiversOverview :IJob
    {
        public void Execute( IJobExecutionContext context )
        {
            // Get job settings
            var dataMap = context.JobDetail.JobDataMap;
            var systemEmailGuid = dataMap.GetString( "Email" ).AsGuidOrNull();
            var groupGuid = dataMap.GetString( "Group" ).AsGuidOrNull();
            var previousMinutes = dataMap.GetString( "PreviousMinutes" ).AsIntegerOrNull();

            // Ensure job settings aren't null
            if ( systemEmailGuid == null || previousMinutes == null || groupGuid == null)
            {
                throw new Exception( "Missing one or more job settings." );
            }

            var rockContext = new RockContext();

            var systemEmail = new SystemEmailService(rockContext).Get(systemEmailGuid.Value);
            var group = new GroupService(rockContext).Get(groupGuid.Value);

            if (systemEmail == null || group == null)
            {
                throw new Exception( "One or more job settings incorrect." );
            }

            string appRoot = Rock.Web.Cache.GlobalAttributesCache.Read().GetValue( "ExternalApplicationRoot" );
            var cutOffBegins = RockDateTime.Now.AddMinutes(- previousMinutes.Value);

            var scheduledTransactions = new FinancialScheduledTransactionService(rockContext)
                .Queryable("FinancialPaymentDetail")
                .Where(s => s.IsActive && s.CreatedDateTime >= cutOffBegins)
                .ToList();

            if (scheduledTransactions.Count > 1)
            {
                foreach ( var groupMember in group.Members )
                {
                    var mergeFields = new Dictionary<string, object> { { "Transactions", scheduledTransactions } };

                    var recipients = new List<string>() { groupMember.Person.Email };

                    Email.Send( systemEmail.From.ResolveMergeFields( mergeFields ), systemEmail.FromName.ResolveMergeFields( mergeFields ), systemEmail.Subject.ResolveMergeFields( mergeFields ), recipients, systemEmail.Body.ResolveMergeFields( mergeFields ), appRoot, null );
                }
                context.Result = string.Format( "{0} transactions were sent ", scheduledTransactions.Count() );
            }
            else
            {
                context.Result = "No new transactions to send.";
            }
        }
    }
}
