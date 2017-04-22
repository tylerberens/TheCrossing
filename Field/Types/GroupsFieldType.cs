using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using Rock;
using Rock.Data;
using Rock.Field;
using Rock.Field.Types;
using Rock.Model;
using Rock.Reporting;
using Rock.Web.UI.Controls;

namespace com.bricksandmortarstudio.TheCrossing.Field.Types
{
    /// <summary>
    /// Stored as comma-delimited list of Group.Guids
    /// </summary>
    public class GroupsFieldType : GroupFieldType
    {

        #region Formatting

        /// <summary>
        /// Returns the field's current value(s)
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="value">Information about the value</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="condensed">Flag indicating if the value should be condensed (i.e. for use in a grid column)</param>
        /// <returns></returns>
        public override string FormatValue( Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            string formattedValue = string.Empty;

            if ( !string.IsNullOrWhiteSpace( value ) )
            {
                var guids = value.SplitDelimitedValues();
                var groups = new GroupService( new RockContext() ).Queryable().Where( a => guids.Contains( a.Guid.ToString() ) );
                if ( groups.Any() )
                {
                    formattedValue = string.Join( ", ", ( groups.Select( g => g.Name ).ToArray() ) );
                }
            }

            return base.FormatValue( parentControl, formattedValue, null, condensed );
        }

        #endregion

        #region EditControl

        /// <summary>
        /// Creates the control(s) necessary for prompting user for a new value
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id"></param>
        /// <returns>
        /// The control
        /// </returns>
        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            var picker = new GroupPicker { ID = id, AllowMultiSelect = true };
            return picker;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            var picker = control as GroupPicker;
            string result = null;

            if ( picker != null )
            {
                var guids = new List<Guid>();
                var ids = picker.SelectedValuesAsInt();
                var groups = new GroupService( new RockContext() ).Queryable().Where( g => ids.Contains( g.Id ) );

                if ( groups.Any() )
                {
                    guids = groups.Select( g => g.Guid ).ToList();
                }

                result = string.Join( ",", guids );

            }

            return result;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="value">The value.</param>
        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            if ( value != null )
            {
                var picker = control as GroupPicker;
                var guids = new List<Guid>();

                if ( picker != null )
                {
                    var ids = value.Split( new[] { ',' } );

                    foreach ( var id in ids )
                    {
                        Guid guid;

                        if ( Guid.TryParse( id, out guid ) )
                        {
                            guids.Add( guid );
                        }
                    }

                    var groups = new GroupService( new RockContext() ).Queryable().Where( g => guids.Contains( g.Guid ) );
                    picker.SetValues( groups );
                }
            }
        }

        #endregion

        #region Filter Control

        /// <summary>
        /// Gets the type of the filter comparison.
        /// </summary>
        /// <value>
        /// The type of the filter comparison.
        /// </value>
        public override ComparisonType FilterComparisonType
        {
            get
            {
                return ComparisonHelper.ContainsFilterComparisonTypes;
            }
        }

        #endregion

    }
}
