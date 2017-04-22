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

namespace RockWeb.Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Volunteer Dashboard" )]
    [Category( "com_bricksandmortarstudio > Crossing" )]
    [Description( "Dashboard to keep track of volunteers." )]
    [GroupTypeField( "Serving Group Type", "The Group Type to display in the grid", true, Rock.SystemGuid.GroupType.GROUPTYPE_SERVING_TEAM )]
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

        /// <summary>
        /// Handles the SelectedDateRangeChanged event of the drpSlidingDateRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void drpSlidingDateRange_SelectedDateRangeChanged( object sender, EventArgs e )
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
            HistoryService historyService = new HistoryService( rockContext );
            GroupService groupService = new GroupService( rockContext );
            GroupTypeService groupTypeService = new GroupTypeService( rockContext );

            var servingTeamGroupTypeGuid = GetAttributeValue( "ServingGroupType" ).AsGuidOrNull();
            if ( servingTeamGroupTypeGuid.HasValue )
            {
                var servingTeamChildGroups = groupTypeService.GetChildGroupTypes( servingTeamGroupTypeGuid.Value );

                var servingGroups = groupService.Queryable().Where( g => servingTeamChildGroups.Select( gt => gt.Id ).Contains( g.GroupTypeId ) ).ToList();
                var servingGroupIds = servingGroups.Select( g => g.Id ).ToList();

                var joinHistory = historyService.Queryable().Where( h =>
                    h.EntityType.Guid == entityTypePersonGuid &&
                    h.Summary == "Added to group." &&
                    h.RelatedEntityTypeId.HasValue &&
                     h.RelatedEntityType.Guid == entityTypeGroupGuid &&
                     h.RelatedEntityId.HasValue &&
                     servingGroupIds.Contains( h.RelatedEntityId.Value )
                     ).Select( h => new
                     {
                         PersonId = h.EntityId,
                         GroupId = h.RelatedEntityId,
                         JoinDate = h.CreatedDateTime
                     } )
                     .GroupBy( h => new
                     {
                         PersonId = h.PersonId,
                         GroupId = h.GroupId
                     } );

                var leaveHistory = historyService.Queryable().Where( h =>
                    h.EntityType.Guid == entityTypePersonGuid &&
                    h.Summary == "Removed from group." &&
                    h.RelatedEntityTypeId.HasValue &&
                     h.RelatedEntityType.Guid == entityTypeGroupGuid &&
                     h.RelatedEntityId.HasValue &&
                     servingGroupIds.Contains( h.RelatedEntityId.Value )
                     ).Select( h => new
                     {
                         PersonId = h.EntityId,
                         GroupId = h.RelatedEntityId,
                         LeaveDate = h.CreatedDateTime
                     } )
                     .GroupBy( h => new
                     {
                         PersonId = h.PersonId,
                         GroupId = h.GroupId
                     } );

                var personGroupHistory = joinHistory.Join( leaveHistory, j => j.Key, l => l.Key, ( j, l ) => new
                {
                    PersonId = j.Key.PersonId,
                    GroupId = j.Key.GroupId,
                    JoinDates = j.Select( h => h.JoinDate ),
                    LeaveDates = l.Select( h => h.LeaveDate )
                } );

                DateRange dateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( drpSlidingDateRange.DelimitedValues );
                if ( dateRange.Start == null )
                {
                    dateRange.Start = RockDateTime.Now;
                }

                if ( dateRange.End == null )
                {
                    dateRange.End = RockDateTime.Now;
                }

                IEnumerable<DateRange> dateRangeList = SplitDateRangeIntoWeeks( dateRange );

                var gridSource = servingGroups.SelectMany( g => dateRangeList, ( g, d ) => new
                {
                    Ministry = g.ParentGroup != null ? g.ParentGroup.Name : g.Name,
                    Director = "",
                    DateRange = d.Start.Value.ToShortDateString() + " " + d.End.Value.ToShortDateString(),
                    StartingVolunteers = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Any( jd => jd < d.Start ) && !h.LeaveDates.Any( ld => ld >= h.JoinDates.Where( jd => jd < d.Start ).OrderByDescending( jd => jd ).FirstOrDefault() ) ).DistinctBy( h => h.PersonId ).Count(),
                    NewVolunteers = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Where( jd => jd >= d.Start && jd <= d.End ).Count() > h.LeaveDates.Where( ld => ld >= d.Start && ld <= d.End ).Count() ).DistinctBy( h => h.PersonId ).Count(),
                    LostVolunteers = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Where( jd => jd >= d.Start && jd <= d.End ).Count() < h.LeaveDates.Where( ld => ld >= d.Start && ld <= d.End ).Count() ).DistinctBy( h => h.PersonId ).Count(),
                    TotalVolunteers = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Any( jd => jd <= d.End ) && !h.LeaveDates.Any( ld => ld >= h.JoinDates.Where( jd => jd <= d.End ).OrderByDescending( jd => jd ).FirstOrDefault() ) ).DistinctBy( h => h.PersonId ).Count(),
                    VolunteerGoal = "",
                    VolunteerPercent = "",
                    StartingLeaders = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Any( jd => jd < d.Start ) && !h.LeaveDates.Any( ld => ld >= h.JoinDates.Where( jd => jd < d.Start ).OrderByDescending( jd => jd ).FirstOrDefault() ) ).DistinctBy( h => h.PersonId ).Count(),
                    NewLeaders = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Where( jd => jd >= d.Start && jd <= d.End ).Count() > h.LeaveDates.Where( ld => ld >= d.Start && ld <= d.End ).Count() ).DistinctBy( h => h.PersonId ).Count(),
                    LostLeaders = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Where( jd => jd >= d.Start && jd <= d.End ).Count() < h.LeaveDates.Where( ld => ld >= d.Start && ld <= d.End ).Count() ).DistinctBy( h => h.PersonId ).Count(),
                    TotalLeaders = personGroupHistory.Where( h => h.GroupId == g.Id && h.JoinDates.Any( jd => jd <= d.End ) && !h.LeaveDates.Any( ld => ld >= h.JoinDates.Where( jd => jd <= d.End ).OrderByDescending( jd => jd ).FirstOrDefault() ) ).DistinctBy( h => h.PersonId ).Count(),
                    LeaderGoal = "",
                    LeaderPercent = ""
                } ).ToList();

                gList.DataSource = gridSource.ToList();
                gList.DataBind();
            }
        }

        public IEnumerable<DateRange> SplitDateRangeIntoWeeks( DateRange dateRange )
        {
            DateTime rangeEnd;
            while ( ( rangeEnd = dateRange.Start.Value.AddDays( 7 ) ) < dateRange.End.Value )
            {
                yield return new DateRange( dateRange.Start, rangeEnd );
                dateRange.Start = rangeEnd;
            }
            yield return new DateRange( dateRange.Start, dateRange.End );
        }

        #endregion

    }
}