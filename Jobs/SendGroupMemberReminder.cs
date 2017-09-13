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
using Rock.Web.Cache;

namespace com.bricksandmortarstudio.TheCrossing.Jobs
{
    [SystemEmailField( "Email", "The System Email to send to group members" )]
    [GroupField("Root Group", "This group and its descendent groups will be the only groups emailed", true)]
    public class SendGroupMemberReminder : IJob
    {
        public SendGroupMemberReminder() { }
        public void Execute( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var dataMap = context.JobDetail.JobDataMap;
            var systemEmailGuid = dataMap.GetString( "Email" ).AsGuidOrNull();
            var groupFieldGuid = dataMap.GetString("RootGroup").AsGuidOrNull();

            string appRoot = GlobalAttributesCache.Read().GetValue( "ExternalApplicationRoot" );
            if ( systemEmailGuid == null )
            {
                throw new Exception( "A system email template needs to be set." );
            }
            var systemEmailTemplate = new SystemEmailService( rockContext ).Get( systemEmailGuid.Value );

            if ( systemEmailTemplate == null )
            {
                throw new Exception( "The system email template setting is not a valid system email template." );
            }

            if (groupFieldGuid == null)
            {
                throw new Exception("A group must be specified");
            }

            var groupService = new GroupService(rockContext);
            var rootGroup = groupService.GetByGuid(groupFieldGuid.Value);
            var validGroupIds = new List<int> {rootGroup.Id};
            validGroupIds.AddRange(groupService.GetAllDescendents(rootGroup.Id).Select(g => g.Id));

            // Check to see if we should skip this week
            var definedTypeId = new DefinedTypeService(rockContext).Queryable().FirstOrDefault(dt => dt.Name == "Volunteer Reminder Exclusions" )?.Id;
            if (definedTypeId == null)
            {
                context.Result = "Could not get Volunteer Reminder Exclusions defined type";
                return;
            }
            var datesToSkip = new DefinedValueService(rockContext).GetByDefinedTypeId(definedTypeId.Value);
            foreach (var dateToSkipValue in datesToSkip)
            {
                dateToSkipValue.LoadAttributes();
                var date = dateToSkipValue.GetAttributeValue("Date").AsDateTime();
                if (date != null && DatesAreInTheSameWeek(date.Value, RockDateTime.Today))
                {
                    context.Result = "This week should be skipped because of the exclusion " + date.Value.ToString("o") ;
                    return;
                }
            }


            string weekTeam = CountDays( DayOfWeek.Sunday, new DateTime( 2017, 08, 27 ), RockDateTime.Today ) % 2 == 1 ? "Team 1" : "Team 2";
            var attributeService = new AttributeService( rockContext );
            var attributeIds =
                attributeService.GetByEntityTypeId(
                    EntityTypeCache.Read( Rock.SystemGuid.EntityType.GROUP_MEMBER.AsGuid() ).Id ).Where( a => a.Key == "AssignedTeam" ).Select( a => a.Id );
            var attributeValueService = new AttributeValueService( rockContext );
            var groupMemberIds = new List<int>();

            foreach ( int attributeId in attributeIds )
            {
                var attributeValues = attributeValueService.GetByAttributeId( attributeId ).AsQueryable().AsNoTracking().Where( av => av.Value.Contains( weekTeam ) );
                groupMemberIds.AddRange( attributeValues.Where( av => av.EntityId != null ).Select( av => av.EntityId.Value ) );
            }


            var groupMembers = new GroupMemberService(rockContext).GetListByIds(groupMemberIds).Where(gm => validGroupIds.Contains( gm.GroupId ) ).Distinct();
            int mailedCount = 0;
            foreach ( var groupMember in groupMembers )
            {
                var mergeFields = new Dictionary<string, object>
                            {
                                {"GroupMember", groupMember},
                                {"Person", groupMember.Person},
                                {"Group", groupMember.Group}
                            };

                var recipients = new List<string> { groupMember.Person.Email };

                Email.Send( systemEmailTemplate.From.ResolveMergeFields( mergeFields ), systemEmailTemplate.FromName.ResolveMergeFields( mergeFields ), systemEmailTemplate.Subject.ResolveMergeFields( mergeFields ), recipients, systemEmailTemplate.Body.ResolveMergeFields( mergeFields ), appRoot, null, null );
                mailedCount++;
            }
            context.Result = string.Format( "{0} reminders were sent ", mailedCount );
        }

        private bool DatesAreInTheSameWeek( DateTime date1, DateTime date2 )
        {
            if (System.Globalization.DateTimeFormatInfo.CurrentInfo == null)
            {
                throw new Exception("System.Globalization.DateTimeFormatInfo.CurrentInfo is null");
            }
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1*(int) cal.GetDayOfWeek(date1) - 1);
            var d2 = date2.Date.AddDays(-1*(int) cal.GetDayOfWeek(date2) - 1);

            return d1 == d2;
        }

        private static int CountDays( DayOfWeek day, DateTime start, DateTime end )
        {
            var ts = end - start;                       // Total duration
            int count = ( int ) Math.Floor( ts.TotalDays / 7 );   // Number of whole weeks
            int remainder = ( int ) ( ts.TotalDays % 7 );         // Number of remaining days
            int sinceLastDay = ( int ) ( end.DayOfWeek - day );   // Number of days since last [day]
            if ( sinceLastDay < 0 )
                sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if ( remainder >= sinceLastDay )
                count++;

            return count;
        }
    }
}
