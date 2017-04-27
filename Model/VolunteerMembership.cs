
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Model;
namespace com.bricksandmortarstudio.TheCrossing.Model
{
    /// <summary>
    /// A VolunteerMembership
    /// </summary>
    [Table( "_com_bricksandmortarstudio_TheCrossing_VolunteerMembership" )]
    [DataContract]
    public class VolunteerMembership : Rock.Data.Model<VolunteerMembership>
    {

        #region Entity Properties

        [DataMember]
        public int PersonId { get; set; }

        [DataMember]
        public int GroupId { get; set; }

        [DataMember]
        public int GroupRoleId { get; set; }

        [DataMember]
        public DateTime JoinedGroupDateTime { get; set; }

        [DataMember]
        public DateTime? LeftGroupDateTime { get; set; }

        #endregion

        #region Virtual Properties

        [DataMember]
        public virtual Person Person { get; set; }

        [LavaInclude]
        public virtual Group Group { get; set; }

        [DataMember]
        public virtual GroupTypeRole GroupRole { get; set; }

        #endregion

    }

    #region Entity Configuration


    public partial class VolunteerMembershipConfiguration : EntityTypeConfiguration<VolunteerMembership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VolunteerMembershipConfiguration"/> class.
        /// </summary>
        public VolunteerMembershipConfiguration()
        {
            this.HasRequired( p => p.Person ).WithMany().HasForeignKey( p => p.PersonId ).WillCascadeOnDelete( false );
            this.HasRequired( p => p.Group ).WithMany().HasForeignKey( p => p.GroupId ).WillCascadeOnDelete( false );
            this.HasRequired( p => p.GroupRole ).WithMany().HasForeignKey( p => p.GroupRoleId ).WillCascadeOnDelete( false );
        }
    }

    #endregion

}
