// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

using com.bricksandmortarstudio.TheCrossing.Model;
using com.bricksandmortarstudio.TheCrossing.Data;

namespace RockWeb.Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Volunteer Dashboard" )]
    [Category( "com_bricksandmortarstudio > Crossing" )]
    [Description( "Dashboard to keep track of volunteers." )]
    [GroupTypeField( "Serving Group Type", "The Group Type to display in the grid", true, Rock.SystemGuid.GroupType.GROUPTYPE_SERVING_TEAM )]
    [TextField( "Director Attribute Key", "The key for the Director group attribute" )]
    [TextField( "Volunteer Goal Attribute Key", "The key for the Volunteer Goal group attribute" )]
    [TextField( "Leader Goal Attribute Key", "The key for the Leader Goal group attribute" )]

    public partial class VolunteerDashboard : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        // used for public / protected properties

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            gList.GridRebind += gList_GridRebind;
            gVolunteers.GridRebind += gVolunteers_GridRebind;
            gLeaders.GridRebind += gLeaders_GridRebind;
            gUniques.GridRebind += gUniques_GridRebind;

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                BindGrid();
            }
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Handles the GridRebind event of the gPledges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gList_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        private void gVolunteers_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        private void gLeaders_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        private void gUniques_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        protected void dp_TextChanged( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var entityTypeGroupGuid = Rock.SystemGuid.EntityType.GROUP.AsGuid();
            var entityTypePersonGuid = Rock.SystemGuid.EntityType.PERSON.AsGuid();

            RockContext rockContext = new RockContext();
            PersonService personService = new PersonService( rockContext );
            GroupService groupService = new GroupService( rockContext );
            GroupTypeService groupTypeService = new GroupTypeService( rockContext );
            VolunteerMembershipService volunteerMembershipService = new VolunteerMembershipService( new VolunteerTrackingContext() );

            var servingTeamGroupTypeGuid = GetAttributeValue( "ServingGroupType" ).AsGuidOrNull();
            if ( servingTeamGroupTypeGuid.HasValue )
            {
                var servingTeamChildGroupTypes = groupTypeService.GetChildGroupTypes( servingTeamGroupTypeGuid.Value );

                var servingGroupQry = groupService.Queryable().Where( g => servingTeamChildGroupTypes.Select( gt => gt.Id ).Contains( g.GroupTypeId ) );

                var servingGroupIds = servingGroupQry.Select( g => g.Id ).ToList();

                var servingGroups = new List<GroupSummary>();

                foreach ( var servingGroup in servingGroupQry.ToList() )
                {
                    servingGroup.LoadAttributes();
                    servingGroups.Add( new GroupSummary
                    {
                        GroupId = servingGroup.Id,
                        ParentGroupId = servingGroup.ParentGroupId,
                        GroupName = servingGroup.Name,
                        ParentGroupName = servingGroup.ParentGroupId != null ? servingGroup.ParentGroup.Name : string.Empty,
                        Director = servingGroup.GetAttributeValue( GetAttributeValue( "DirectorAttributeKey" ) ),
                        VolunteerGoal = servingGroup.GetAttributeValue( GetAttributeValue( "VolunteerGoalAttributeKey" ) ).AsIntegerOrNull(),
                        LeaderGoal = servingGroup.GetAttributeValue( GetAttributeValue( "LeaderGoalAttributeKey" ) ).AsIntegerOrNull()
                    } );
                }

                var volunteerQry = volunteerMembershipService.Queryable().Where( vm => servingGroupIds.Contains( vm.GroupId ) );

                var volunteerSummary = volunteerQry.Select( vm => new
                {
                    GroupId = vm.GroupId,
                    Volunteer = vm
                } )
                .GroupBy( vm => vm.GroupId )
                .Select( vmg => new
                {
                    GroupId = vmg.Key,
                    Volunteers = vmg.Select( v => v.Volunteer ).ToList()
                } );

                var servingGroupSummary = from g in servingGroups
                                          join vmg in volunteerSummary
                                          on g.GroupId equals vmg.GroupId into joinResult
                                          from x in joinResult.DefaultIfEmpty( new { GroupId = g.GroupId, Volunteers = new List<VolunteerMembership>() } )
                                          select new
                                          {
                                              Group = g,
                                              Volunteers = x.Volunteers
                                          };

                DateRange dateRange = new DateRange( dpStart.SelectedDate, dpEnd.SelectedDate );
                if ( dpStart.SelectedDate == null )
                {
                    dateRange.Start = RockDateTime.Now;
                }

                if ( dpStart.SelectedDate == null )
                {
                    dateRange.End = RockDateTime.Now;
                }

                IEnumerable<DateRange> dateRangeList = SplitDateRangeIntoWeeks( dateRange );

                var gListSource = servingGroupSummary.SelectMany( g => dateRangeList, ( g, d ) => new
                {
                    Ministry = g.Group.ParentGroupId != null ? g.Group.ParentGroupName : g.Group.GroupName,
                    Director = g.Group.Director,
                    DateRange = d.Start.Value.ToShortDateString() + " - " + d.End.Value.ToShortDateString(),
                    StartingVolunteers = g.Volunteers.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.Start.Value ) &&
                                                                vm.JoinedGroupDateTime < d.Start.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    NewVolunteers = g.Volunteers.Where( vm => !vm.GroupRole.IsLeader &&
                                                            ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                            ( vm.JoinedGroupDateTime >= d.Start.Value && vm.JoinedGroupDateTime < d.End.Value ) )
                                                .DistinctBy( vm => vm.PersonId ).Count(),
                    LostVolunteers = g.Volunteers.Where( vm => !vm.GroupRole.IsLeader )
                                                .GroupBy( vm => vm.PersonId )
                                                .Where( vmg => vmg.Where( v => v.LeftGroupDateTime >= d.Start.Value && v.LeftGroupDateTime < d.End.Value ).Count() > vmg.Where( v => v.JoinedGroupDateTime >= d.Start.Value && v.JoinedGroupDateTime < d.End.Value ).Count() )
                                                .Count(),
                    TotalVolunteers = g.Volunteers.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                                vm.JoinedGroupDateTime < d.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    VolunteerGoal = g.Group.VolunteerGoal ?? null,
                    VolunteerPercent = g.Group.VolunteerGoal != null ? g.Volunteers.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                                vm.JoinedGroupDateTime < d.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count() / g.Group.VolunteerGoal : null,
                    StartingLeaders = g.Volunteers.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.Start.Value ) &&
                                                                vm.JoinedGroupDateTime < d.Start.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    NewLeaders = g.Volunteers.Where( vm => vm.GroupRole.IsLeader &&
                                                            ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                            ( vm.JoinedGroupDateTime >= d.Start.Value && vm.JoinedGroupDateTime < d.End.Value ) )
                                                .DistinctBy( vm => vm.PersonId ).Count(),
                    LostLeaders = g.Volunteers.Where( vm => vm.GroupRole.IsLeader )
                                                .GroupBy( vm => vm.PersonId )
                                                .Where( vmg => vmg.Where( v => v.LeftGroupDateTime >= d.Start.Value && v.LeftGroupDateTime < d.End.Value ).Count() > vmg.Where( v => v.JoinedGroupDateTime >= d.Start.Value && v.JoinedGroupDateTime < d.End.Value ).Count() )
                                                .Count(),
                    TotalLeaders = g.Volunteers.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                                vm.JoinedGroupDateTime < d.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    LeaderGoal = g.Group.LeaderGoal ?? null,
                    LeaderPercent = g.Group.LeaderGoal != null ? g.Volunteers.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > d.End.Value ) &&
                                                                vm.JoinedGroupDateTime < d.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count() / g.Group.LeaderGoal : null
                } ).ToList();

                gList.DataSource = gListSource.ToList();
                gList.DataBind();


                var gVolunteersSource = new[] { new {
                    StartingVolunteers = volunteerQry.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.Start.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.Start.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    NewVolunteers = volunteerQry.Where( vm => !vm.GroupRole.IsLeader &&
                                                            ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                            ( vm.JoinedGroupDateTime >= dateRange.Start.Value && vm.JoinedGroupDateTime < dateRange.End.Value ) )
                                                .DistinctBy( vm => vm.PersonId ).Count(),
                    LostVolunteers = volunteerQry.Where( vm => !vm.GroupRole.IsLeader )
                                                .GroupBy( vm => vm.PersonId )
                                                .Where( vmg => vmg.Where( v => v.LeftGroupDateTime >= dateRange.Start.Value && v.LeftGroupDateTime < dateRange.End.Value ).Count() > vmg.Where( v => v.JoinedGroupDateTime >= dateRange.Start.Value && v.JoinedGroupDateTime < dateRange.End.Value ).Count() )
                                                .Count(),
                    TotalVolunteers = volunteerQry.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    VolunteerGoal = servingGroups.Sum(g=> g.VolunteerGoal ) ?? null,
                    VolunteerPercent = servingGroups.Sum(g=> g.VolunteerGoal ) > 0 ? volunteerQry.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count() /servingGroups.Sum(g=> g.VolunteerGoal ) : null,

                } };

                gVolunteers.DataSource = gVolunteersSource.ToList();
                gVolunteers.DataBind();
                gVolunteers.ShowFooter = false;

                var gLeadersSource = new[] { new {
                    StartingLeaders = volunteerQry.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.Start.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.Start.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    NewLeaders = volunteerQry.Where( vm => vm.GroupRole.IsLeader &&
                                                            ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                            ( vm.JoinedGroupDateTime >= dateRange.Start.Value && vm.JoinedGroupDateTime < dateRange.End.Value ) )
                                                .DistinctBy( vm => vm.PersonId ).Count(),
                    LostLeaders = volunteerQry.Where( vm => vm.GroupRole.IsLeader )
                                                .GroupBy( vm => vm.PersonId )
                                                .Where( vmg => vmg.Where( v => v.LeftGroupDateTime >= dateRange.Start.Value && v.LeftGroupDateTime < dateRange.End.Value ).Count() > vmg.Where( v => v.JoinedGroupDateTime >= dateRange.Start.Value && v.JoinedGroupDateTime < dateRange.End.Value ).Count() )
                                                .Count(),
                    TotalLeaders = volunteerQry.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    LeaderGoal = servingGroups.Sum(g=> g.LeaderGoal ) ?? null,
                    LeaderPercent = servingGroups.Sum(g=> g.LeaderGoal ) > 0 ? volunteerQry.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count() /servingGroups.Sum(g=> g.LeaderGoal ) : null,

                } };

                gLeaders.DataSource = gLeadersSource.ToList();
                gLeaders.DataBind();

                var gUniquesSource = new[] { new {
                    UniqueVolunteers = volunteerQry.Where( vm => !vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    UniqueLeaders = volunteerQry.Where( vm => vm.GroupRole.IsLeader &&
                                                                ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                                vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count(),
                    UniqueTotal = volunteerQry.Where( vm =>
                                                        ( !vm.LeftGroupDateTime.HasValue || vm.LeftGroupDateTime > dateRange.End.Value ) &&
                                                         vm.JoinedGroupDateTime < dateRange.End.Value )
                                                    .DistinctBy( vm => vm.PersonId )
                                                    .Count()

                } };

                gUniques.DataSource = gUniquesSource.ToList();
                gUniques.DataBind();
            }
        }

        public IEnumerable<DateRange> SplitDateRangeIntoWeeks( DateRange dateRange )
        {
            DateTime rangeEnd;
            while ( ( rangeEnd = dateRange.Start.Value.AddDays( 7 ) ) < dateRange.End.Value )
            {
                yield return new DateRange( dateRange.Start, rangeEnd.AddDays( 1 ).AddMilliseconds( -2 ) );
                dateRange.Start = rangeEnd;
            }
            yield return new DateRange( dateRange.Start, dateRange.End );
        }

        #endregion


        #region Helper Classes

        public class GroupSummary
        {
            public int GroupId { get; set; }

            public int? ParentGroupId { get; set; }

            public String ParentGroupName { get; set; }

            public String GroupName { get; set; }

            public String Director { get; set; }

            public int? VolunteerGoal { get; set; }

            public int? LeaderGoal { get; set; }
        }

        #endregion
    }
}