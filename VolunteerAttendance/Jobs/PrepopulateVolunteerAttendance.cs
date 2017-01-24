using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace com.bricksandmortarstudio.TheCrossing.VolunteerAttendance.Jobs
{
    [GroupField("Root Group", "The root serving team group to generate attendance for the children of")]
    [IntegerField("Days Ahead", "The number of days ahead to find groups")]
    [TextField("Assigned Service Attribute Key", "The group member attribute key that corresponds to an assigned service. Used to find members who will be serving.", defaultValue:"AssignedService")]
    [TextField( "Assigned Team Attribute Key", "The group member attribute key that corresponds to an assigned Team. Used to find members who will be serving.", defaultValue: "AssignedTeam" )]
    public class PrepopulateVolunteerAttendance : IJob
    {
        private readonly HashSet<GroupMember> _groupMembers = new HashSet<GroupMember>();
        private HashSet<int> _seenGroupIds = new HashSet<int>();
        private HashSet<Schedule> _schedules = new HashSet<Schedule>();
        private DateTime _scheduleCutOffDateTime;
        private HashSet<string> _validScheduleNames = new HashSet<string>();

        public void Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var rockContext = new RockContext();
            var groupService = new GroupService(rockContext);
            var rootGroup = groupService.Get(dataMap.GetString("RootGroup").AsGuid());
            _scheduleCutOffDateTime = RockDateTime.Now.AddDays( dataMap.GetIntValue( "DaysAhead" ) );

            GetChildren(rootGroup);

            string serviceAttributeKey = dataMap.GetString("AssignedServiceAttributeKey");
            string teamAttributeKey = dataMap.GetString("AssignedTeamAttributeKey");

            var attendanceService = new AttendanceService(rockContext);
            foreach (var groupMember in _groupMembers)
            {
                groupMember.LoadAttributes();

                string groupMemberSchedule = groupMember.AttributeValues[teamAttributeKey] + " - " +
                                             groupMember.AttributeValues[serviceAttributeKey];

                // Check if the group members schedule attributes match a schedule within the window
                if (_validScheduleNames.Contains(groupMemberSchedule))
                {
                    // Add an attendance record with RSVP 'Yes' to indicate it was added by this job and they were scheduled
                    // The start date will be overwritten by Rock.Workflow.Action.CheckIn.SaveAttendance
                    var attendance = new Attendance
                    {
                        Group = groupMember.Group,
                        GroupId = groupMember.GroupId,
                        DidAttend = false,
                        Location = groupMember.Group.GroupLocations.FirstOrDefault()?.Location,
                        Schedule = groupMember.Group.Schedule,
                        PersonAlias = groupMember.Person.PrimaryAlias,
                        StartDateTime = DateTime.MinValue,
                        RSVP = RSVP.Yes
                    };
                    attendanceService.Add(attendance);
                }
            }
            rockContext.SaveChanges();

        }

        private void GetChildren(Group group)
        {
            _seenGroupIds.Add( group.Id );
            if (group.Groups.Count != 0)
            {
                foreach (var childGroup in group.Groups.Where( g => !_seenGroupIds.Contains(g.Id)))
                {
                    GetChildren(childGroup);
                }
            }

            CollectScheduleAndGroupMembers(group);
        }

        private void CollectScheduleAndGroupMembers(Group @group)
        {
            bool shouldContinue = false;
            foreach (var groupLocation in group.GroupLocations)
            {
                foreach (var schedule in groupLocation.Schedules)
                {
                    if (schedule.NextStartDateTime.HasValue && schedule.NextStartDateTime.Value <= _scheduleCutOffDateTime)
                    {
                        _validScheduleNames.Add(schedule.Name);
                        shouldContinue = true;
                    }
                }
            }

            if (shouldContinue)
            {
                foreach ( var groupMember in @group.Members.Where( g => g.GroupMemberStatus != GroupMemberStatus.Inactive) )
                {
                    _groupMembers.Add( groupMember );
                }
            }
        }
    }
}
