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
using System.Text;

using com.bricksandmortarstudio.TheCrossing.Model;
using com.bricksandmortarstudio.TheCrossing.Data;

using com.bricksandmortarstudio.TheCrossing.Attribute;

namespace RockWeb.Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Update Volunteers" )]
    [Category( "com_bricksandmortarstudio > Crossing" )]
    [Description( "Block to import existing group members into the VolunteerMembership table." )]
    [GroupTypesField("Group Types", "The group types to import", true)]
    public partial class UpdateVolunteers : Rock.Web.UI.RockBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

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
                ShowDetail();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ShowDetail();
        }

        protected void btnImport_Click( object sender, EventArgs e )
        {
            btnImport.Enabled = false;
            var rockContext = new RockContext();
            var volunteerTrackingContext = new VolunteerTrackingContext();
            var volunteerMembershipService = new VolunteerMembershipService( volunteerTrackingContext );
            var groupService = new GroupService( rockContext );

            var groupTypeGuids = GetAttributeValue("GroupTypes").SplitDelimitedValues().AsGuidList();
            var groupTypeIds = new GroupTypeService(rockContext).GetByGuids(groupTypeGuids).Select(gt => gt.Id);
            var groups = groupService.Queryable().Where( g => groupTypeIds.Contains( g.GroupTypeId ) ).ToList();

            foreach ( var group in groups )
            {
                foreach ( var groupMember in group.Members )
                {
                    var volunteerMember = volunteerMembershipService.Queryable().FirstOrDefault( v => v.GroupId == groupMember.GroupId && v.PersonId == groupMember.PersonId && v.LeftGroupDateTime == null );
                    if ( volunteerMember == null )
                    {
                        volunteerMember = new VolunteerMembership
                        {
                            GroupId = groupMember.GroupId,
                            PersonId = groupMember.PersonId,
                            GroupRoleId = groupMember.GroupRoleId,
                            JoinedGroupDateTime = DateTime.Now
                        };

                        volunteerMembershipService.Add( volunteerMember );
                    }
                }
            }

            volunteerTrackingContext.SaveChanges();
            btnImport.Enabled = true;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected void ShowDetail()
        {

        }

        #endregion
    }


}
