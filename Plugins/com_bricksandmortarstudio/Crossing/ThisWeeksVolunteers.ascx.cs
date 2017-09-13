// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "This Week's Volunteers" )]
    [Category( "Bricks and Mortar Studio" )]
    [Description( "Template block for developers to use to start a new list block." )]
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

        #endregion

        #region Methods


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
            string weekTeam = CountDays(DayOfWeek.Sunday, new DateTime( 2017, 08, 27 ) , RockDateTime.Today) % 2 == 1 ? "Team 1" : "Team 2";
            var attributeService = new AttributeService( rockContext );
            var attributeIds =
                attributeService.GetByEntityTypeId(
                    EntityTypeCache.Read(Rock.SystemGuid.EntityType.GROUP_MEMBER.AsGuid()).Id).Where( a => a.Key == "AssignedTeam" ).Select(a => a.Id);
            var attributeValueService = new AttributeValueService(rockContext);
            var groupMemberIds = new List<int>();

            foreach (int attributeId in attributeIds)
            {
                var attributeValues = attributeValueService.GetByAttributeId(attributeId).AsQueryable().AsNoTracking().Where(av => av.Value.Contains(weekTeam));
                groupMemberIds.AddRange(attributeValues.Where(av => av.EntityId != null).Select(av => av.EntityId.Value));
            }


            
            // sample query to display a few people
            var qry = new GroupMemberService(rockContext).GetListByIds(groupMemberIds).Select( g =>  new GroupAndPerson() {Name = g.Person.FullName, GroupName = g.Group.Name});

            gList.DataSource = qry.ToList();
            gList.DataBind();
        }

        #endregion
    }

    internal class GroupAndPerson
    {
       public string Name { get; set; }

        public string GroupName { get; set; }

        public string WeekToServe { get; set; }
    }
}