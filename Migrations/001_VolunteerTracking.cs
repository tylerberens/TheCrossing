
using Rock.Plugin;

namespace com.bricksandmortarstudio.TheCrossing.Migrations
{
    [MigrationNumber( 1, "1.6.0" )]
    public class VolunteerTracking : Migration
    {
        public override void Up()
        {           
            Sql( @"
                CREATE TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
                    [PersonId] [int] NOT NULL,
	                [GroupId] [int] NOT NULL,
	                [GroupRoleId] [int] NOT NULL,
                    [JoinedGroupDateTime] [datetime] NOT NULL,
                    [LeftGroupDateTime] [datetime] NULL,
	                [Guid] [uniqueidentifier] NOT NULL,
	                [CreatedDateTime] [datetime] NULL,
	                [ModifiedDateTime] [datetime] NULL,
	                [CreatedByPersonAliasId] [int] NULL,
	                [ModifiedByPersonAliasId] [int] NULL,
	                [ForeignKey] [nvarchar](50) NULL,
                    [ForeignGuid] [uniqueidentifier] NULL,
                    [ForeignId] [int] NULL,
                 CONSTRAINT [PK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]  WITH CHECK ADD  CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Person] FOREIGN KEY([PersonId])
                REFERENCES [dbo].[Person] ([Id])

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] CHECK CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Person]

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]  WITH CHECK ADD  CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Group] FOREIGN KEY([GroupId])
                REFERENCES [dbo].[Group] ([Id])

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] CHECK CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Group]

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]  WITH CHECK ADD  CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_GroupRole] FOREIGN KEY([GroupRoleId])
                REFERENCES [dbo].[GroupTypeRole] ([Id])

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] CHECK CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_GroupRole]

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]  WITH CHECK ADD  CONSTRAINT [FK_dbo.VolunteerMembership_dbo.PersonAlias_CreatedByPersonAliasId] FOREIGN KEY([CreatedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] CHECK CONSTRAINT [FK_dbo.VolunteerMembership_dbo.PersonAlias_CreatedByPersonAliasId]

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]  WITH CHECK ADD  CONSTRAINT [FK_dbo.VolunteerMembership_dbo.PersonAlias_ModifiedByPersonAliasId] FOREIGN KEY([ModifiedByPersonAliasId])
                REFERENCES [dbo].[PersonAlias] ([Id])

                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] CHECK CONSTRAINT [FK_dbo.VolunteerMembership_dbo.PersonAlias_ModifiedByPersonAliasId]
" );
           RockMigrationHelper.UpdateEntityType( "com.bricksandmortarstudio.TheCrossing.Model.VolunteerMembership", "70F15075-ACEE-4A33-B052-1E45271A9ADF", true, true );

            RockMigrationHelper.UpdateFieldType( "Groups", "", "com.bricksandmortarstudio.TheCrossing", "com.bricksandmortarstudio.TheCrossing.Field.Types.GroupsFieldType", "87C17D4B-D465-4B6B-A5E0-3B18CCE226BC" );
        }
        public override void Down()
        {
            RockMigrationHelper.DeleteFieldType( "87C17D4B-D465-4B6B-A5E0-3B18CCE226BC" );
            RockMigrationHelper.DeleteEntityType( "70F15075-ACEE-4A33-B052-1E45271A9ADF" );

            Sql( @"               
                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] DROP CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_ModifiedByPersonAliasId]
                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] DROP CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_CreatedByPersonAliasId]
                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] DROP CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_GroupRole]
                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] DROP CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Group]
                ALTER TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership] DROP CONSTRAINT [FK__com_bricksandmortarstudio_TheCrossing_VolunteerMembership_Person]
                DROP TABLE [dbo].[_com_bricksandmortarstudio_TheCrossing_VolunteerMembership]
                " );
        }
    }
}
