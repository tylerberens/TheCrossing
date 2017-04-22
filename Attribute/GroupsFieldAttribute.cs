using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Field;
using Rock.Field.Types;
using Rock.Model;
using Rock.Reporting;
using Rock.Web.UI.Controls;

namespace com.bricksandmortarstudio.TheCrossing.Attribute
{
    /// <summary>
    /// Field Attribute to select multiple Groups
    /// Stored as a comma-delimited list of Category.Guids
    /// </summary>
    public class GroupsFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsFieldAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="defaultGroupGuid">The default group unique identifier.</param>
        /// <param name="category">The category.</param>
        /// <param name="order">The order.</param>
        /// <param name="key">The key.</param>
        public GroupsFieldAttribute( string name, string description = "", bool required = true, string defaultGroupGuids = "", string category = "", int order = 0, string key = null, string fieldTypeAssembly = "com.bricksandmortarstudio.TheCrossing" )
            : base( name, description, required, defaultGroupGuids, category, order, key, typeof( com.bricksandmortarstudio.TheCrossing.Field.Types.GroupsFieldType ).FullName, fieldTypeAssembly )
        {
        }
    }
}