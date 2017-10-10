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
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

using com.bricksandmortarstudio.TheCrossing.Data;
using com.bricksandmortarstudio.TheCrossing.Model;

namespace com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking
{
    /// <summary>
    /// Sets an attribute's value to the selected person 
    /// </summary>
    [ActionCategory( "com_bricksandmortarstudio > Volunteer Tracking" )]
    [Description( "Adds a groupmember to the volunteer tracking table, or marks a group role change." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Add or Update Volunteer" )]

    [WorkflowAttribute( "Person", "Workflow attribute that contains the person to add in the volunteer table.", true, "", "", 0, null,
        new string[] { "Rock.Field.Types.PersonFieldType" } )]

    [WorkflowAttribute( "Group", "Workflow Attribute that contains the group the person is in.", true, "", "", 1, null,
        new string[] { "Rock.Field.Types.GroupFieldType" } )]
    public class AddOrUpdateVolunteer : ActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            // Determine the group
            Group group = null;

            var guidGroupAttribute = GetAttributeValue( action, "Group" ).AsGuidOrNull();

            if ( guidGroupAttribute.HasValue )
            {
                var attributeGroup = AttributeCache.Read( guidGroupAttribute.Value, rockContext );
                if ( attributeGroup != null )
                {
                    var groupGuid = action.GetWorklowAttributeValue( guidGroupAttribute.Value ).AsGuidOrNull();

                    if ( groupGuid.HasValue )
                    {
                        group = new GroupService( rockContext ).Get( groupGuid.Value );
                    }
                }
            }

            if ( group == null )
            {
                errorMessages.Add( "No group was provided" );
            }

            // determine the person
            Person person = null;

            // get the Attribute.Guid for this workflow's Person Attribute so that we can lookup the value
            var guidPersonAttribute = GetAttributeValue( action, "Person" ).AsGuidOrNull();

            if ( guidPersonAttribute.HasValue )
            {
                var attributePerson = AttributeCache.Read( guidPersonAttribute.Value, rockContext );
                if ( attributePerson != null )
                {
                    string attributePersonValue = action.GetWorklowAttributeValue( guidPersonAttribute.Value );
                    if ( !string.IsNullOrWhiteSpace( attributePersonValue ) )
                    {
                        if ( attributePerson.FieldType.Class == typeof( Rock.Field.Types.PersonFieldType ).FullName )
                        {
                            Guid personAliasGuid = attributePersonValue.AsGuid();
                            if ( !personAliasGuid.IsEmpty() )
                            {
                                person = new PersonAliasService( rockContext ).Queryable()
                                    .Where( a => a.Guid.Equals( personAliasGuid ) )
                                    .Select( a => a.Person )
                                    .FirstOrDefault();
                            }
                        }
                        else
                        {
                            errorMessages.Add( "The attribute used to provide the person was not of type 'Person'." );
                        }
                    }
                }
            }

            if ( person == null )
            {
                errorMessages.Add( string.Format( "Person could not be found for selected value ('{0}')!", guidPersonAttribute.ToString() ) );
            }

            // Add Person to Volunteer Tracking Table
            if ( !errorMessages.Any() )
            {
                var groupMemberService = new GroupMemberService( rockContext );
                var groupMember = groupMemberService.Queryable().FirstOrDefault(m => m.GroupId == group.Id && m.PersonId == person.Id);

                if ( groupMember != null )
                {
                    var volunteerTrackingContext = new VolunteerTrackingContext();
                    var volunteerMembershipService = new VolunteerMembershipService( volunteerTrackingContext );
                    var oldVolunteerMembership = volunteerMembershipService.Queryable().FirstOrDefault(v => v.GroupId == groupMember.GroupId && v.PersonId == groupMember.PersonId);
                    if( oldVolunteerMembership != null)
                    {
                        oldVolunteerMembership.LeftGroupDateTime = DateTime.Now;
                    }

                    var newVolunteerMembership = new VolunteerMembership
                    {
                        GroupId = groupMember.GroupId,
                        PersonId = groupMember.PersonId,
                        GroupRoleId = groupMember.GroupRoleId,
                        JoinedGroupDateTime = DateTime.Now
                    };

                    volunteerMembershipService.Add(newVolunteerMembership);

                    volunteerTrackingContext.SaveChanges();
                }
                else
                {
                    // the person is not in the group provided
                    errorMessages.Add( "The person is not in the group provided." );
                }
            }

            errorMessages.ForEach( m => action.AddLogEntry( m, true ) );

            return true;
        }

    }
}