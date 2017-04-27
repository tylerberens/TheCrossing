
using Rock.Data;
using com.bricksandmortarstudio.TheCrossing.Data;

namespace com.bricksandmortarstudio.TheCrossing.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class VolunteerMembershipService : VolunteerTrackingService<VolunteerMembership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VolunteerMembershipService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public VolunteerMembershipService( VolunteerTrackingContext context ) : base( context ) { }
    }
}
