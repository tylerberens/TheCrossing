using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "This Week's Volunteers" )]
    [Category( "Bricks and Mortar Studio" )]
    [Description( "Find the volunteers serving this week" )]

    [BooleanField( "Is External", "Is this block meant for external access", false )]
    public partial class ThisWeeksVolunteers : Rock.Web.UI.RockBlock
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
            hfGroupGuid.Value = PageParameter( "group" );
            if (GetAttributeValue("IsExternal").AsBoolean())
            {
                gFilter.Visible = false;
            }
            if ( !Page.IsPostBack )
            {
                SetFilter();
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
            BindGrid();
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

        protected void gFilter_ApplyFilterClick( object sender, EventArgs e )
        {
            gFilter.SaveUserPreference( "Group", gpGroup.SelectedValue );
            BindGrid();
        }

        protected void gFilter_OnClearFilterClick( object sender, EventArgs e )
        {
            gFilter.SaveUserPreference( "Group", "" );
            gpGroup.SetValue(null);
            BindGrid();
        }

        protected void gFilter_OnDisplayFilterValue( object sender, GridFilter.DisplayFilterValueArgs e )
        {
            switch (e.Key)
            {
                case "Group":
                    string groupName = string.Empty;

                    int? groupId = e.Value.AsIntegerOrNull();
                    if ( groupId.HasValue )
                    {
                        var groupService = new GroupService( new RockContext() );
                        var group = groupService.Get( groupId.Value );
                        if ( group != null )
                        {
                            groupName = group.Name;
                        }
                    }

                    e.Value = groupName;
                    break;
            }
        }

        #endregion

        #region Methods

        private void SetFilter()
        {
            var rockContext = new RockContext();
            int? groupId = gFilter.GetUserPreference( "Consolidator" ).AsIntegerOrNull();
            if ( groupId.HasValue )
            {
                var groupService = new GroupService( rockContext );
                var group = groupService.Get( groupId.Value );
                if ( group != null )
                {
                    gpGroup.SetValue( group );
                }
            }
        }

        private static int CountDays( DayOfWeek day, DateTime start, DateTime end )
        {
            TimeSpan ts = end - start;                       // Total duration
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

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var rockContext = new RockContext();
            // Get the team that should be serving this week
            string weekTeam = CountDays( DayOfWeek.Sunday, new DateTime( 2017, 08, 27 ), RockDateTime.Today ) % 2 == 1 ? "Team 1" : "Team 2";

            // Get the group members who should be serving
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

            // Find the group member information to present
            IEnumerable<GroupMember> query = new GroupMemberService( rockContext ).GetListByIds( groupMemberIds );


            //Filtering to a specific group branch

            var groupService = new GroupService( rockContext );
            var group = hfGroupGuid.Value.AsGuidOrNull() == null
                ? groupService.GetByGuid(hfGroupGuid.Value.AsGuid())
                : groupService.Get(gFilter.GetUserPreference("Group").AsInteger());

            if ( group != null )
            {
                var validGroupIds = new List<int>();
                validGroupIds.Add(@group.Id);
                validGroupIds.AddRange(groupService.GetAllDescendents(@group.Id).Select(g => g.Id));

                query = query.Where(gm => validGroupIds.Contains(gm.GroupId));
            }

            var allScheduledPeople = query.Select( g =>
                         new GroupAndPerson
                         {
                             Name = g.Person.FullName,
                             GroupName = g.Group.Name,
                             ParentGroup = g.Group.ParentGroup
                         } );

            // Sort and bind
            var sortProperty = gList.SortProperty;
            if ( sortProperty == null )
            {
                gList.DataSource = allScheduledPeople.OrderBy( g => g.Name );
            }
            else
            {
                gList.DataSource = allScheduledPeople.AsQueryable().Sort(sortProperty);
            }
            gList.DataBind();
        }

        #endregion


    }

    internal class GroupAndPerson
    {
        public string Name { get; set; }

        public string GroupName { get; set; }

        public string WeekToServe { get; set; }

        public Group ParentGroup { get; set; }
    }
}