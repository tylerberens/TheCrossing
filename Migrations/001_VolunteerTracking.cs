
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

            // WORKFLOWS

            #region EntityTypes
            RockMigrationHelper.UpdateEntityType( "com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.AddOrUpdateVolunteer", "1C6D926A-63CD-4355-B9D1-9662669373C4", false, true );
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "1C6D926A-63CD-4355-B9D1-9662669373C4", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "6A6F81E1-CC97-47A1-9407-1B151D4E7D21" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.AddOrUpdateVolunteer:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "1C6D926A-63CD-4355-B9D1-9662669373C4", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Group", "Group", "Workflow Attribute that contains the group the person is in.", 1, @"", "1F3A0DD1-CE69-4823-9471-445255B42E1E" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.AddOrUpdateVolunteer:Group
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "1C6D926A-63CD-4355-B9D1-9662669373C4", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to add in the volunteer table.", 0, @"", "1AA77A23-3749-4592-B92F-D9E7DB09190E" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.AddOrUpdateVolunteer:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "1C6D926A-63CD-4355-B9D1-9662669373C4", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "629FA651-5E80-49D6-965D-7756F7D7E7FD" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.AddOrUpdateVolunteer:Order      
            #endregion

            #region Categories

            RockMigrationHelper.UpdateCategory( "C9F3C4A5-1526-474D-803F-D6C7A45CBBAE", "Volunteer Tracking", "fa fa-group", "", "DCD6A16C-C9DE-46F4-840A-08671635A149", 0 ); // Volunteer Tracking

            #endregion

            #region Add New Volunteer

            RockMigrationHelper.UpdateWorkflowType( false, true, "Add New Volunteer", "This workflow will add a newly added volunteer to the volunteer membership table, to track when they start and leave their serving group.", "DCD6A16C-C9DE-46F4-840A-08671635A149", "Work", "fa fa-list-ol", 28800, true, 0, "38853F53-09FB-4687-8C86-4AFC20A214C8", 0 ); // Add New Volunteer
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "38853F53-09FB-4687-8C86-4AFC20A214C8", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "Group", "Group", "", 0, @"", "DF3E1809-9C8B-4E9A-95BE-64BE7417A3F0" ); // Add New Volunteer:Group
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "38853F53-09FB-4687-8C86-4AFC20A214C8", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 1, @"", "54CFC3C1-FBC1-40E6-84CF-71F80C61AB03" ); // Add New Volunteer:Person
            RockMigrationHelper.AddAttributeQualifier( "54CFC3C1-FBC1-40E6-84CF-71F80C61AB03", "EnableSelfSelection", @"False", "A5029CF4-82DB-486F-851C-F6559BA5169C" ); // Add New Volunteer:Person:EnableSelfSelection
            RockMigrationHelper.UpdateWorkflowActivityType( "38853F53-09FB-4687-8C86-4AFC20A214C8", true, "Add Volunteer", "", true, 0, "EE0118E4-3F70-4504-852A-31E449349763" ); // Add New Volunteer:Add Volunteer
            RockMigrationHelper.UpdateWorkflowActionType( "EE0118E4-3F70-4504-852A-31E449349763", "Add New Volunteer", 0, "1C6D926A-63CD-4355-B9D1-9662669373C4", true, false, "", "", 1, "", "70D55678-E28F-4A08-9383-38AEE50A825C" ); // Add New Volunteer:Add Volunteer:Add New Volunteer
            RockMigrationHelper.UpdateWorkflowActionType( "EE0118E4-3F70-4504-852A-31E449349763", "Complete Workflow", 1, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "AB47383B-1589-46B8-88F2-E8F573437151" ); // Add New Volunteer:Add Volunteer:Complete Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "70D55678-E28F-4A08-9383-38AEE50A825C", "629FA651-5E80-49D6-965D-7756F7D7E7FD", @"" ); // Add New Volunteer:Add Volunteer:Add New Volunteer:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "70D55678-E28F-4A08-9383-38AEE50A825C", "6A6F81E1-CC97-47A1-9407-1B151D4E7D21", @"False" ); // Add New Volunteer:Add Volunteer:Add New Volunteer:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "70D55678-E28F-4A08-9383-38AEE50A825C", "1AA77A23-3749-4592-B92F-D9E7DB09190E", @"54cfc3c1-fbc1-40e6-84cf-71f80c61ab03" ); // Add New Volunteer:Add Volunteer:Add New Volunteer:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "70D55678-E28F-4A08-9383-38AEE50A825C", "1F3A0DD1-CE69-4823-9471-445255B42E1E", @"df3e1809-9c8b-4e9a-95be-64be7417a3f0" ); // Add New Volunteer:Add Volunteer:Add New Volunteer:Group
            RockMigrationHelper.AddActionTypeAttributeValue( "AB47383B-1589-46B8-88F2-E8F573437151", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // Add New Volunteer:Add Volunteer:Complete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "AB47383B-1589-46B8-88F2-E8F573437151", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // Add New Volunteer:Add Volunteer:Complete Workflow:Order

            #endregion

            #region DefinedValue AttributeType qualifier helper

            Sql( @"
			UPDATE [aq] SET [key] = 'definedtype', [Value] = CAST( [dt].[Id] as varchar(5) )
			FROM [AttributeQualifier] [aq]
			INNER JOIN [Attribute] [a] ON [a].[Id] = [aq].[AttributeId]
			INNER JOIN [FieldType] [ft] ON [ft].[Id] = [a].[FieldTypeId]
			INNER JOIN [DefinedType] [dt] ON CAST([dt].[guid] AS varchar(50) ) = [aq].[value]
			WHERE [ft].[class] = 'Rock.Field.Types.DefinedValueFieldType'
			AND [aq].[key] = 'definedtypeguid'
		" );

            #endregion           

            #region Update Volunteer Group Role

            RockMigrationHelper.UpdateWorkflowType( false, true, "Update Volunteer Group Role", "This workflow will update a group member's role in the volunteer membership table, as well as mark the day their role changed.", "DCD6A16C-C9DE-46F4-840A-08671635A149", "Work", "fa fa-list-ol", 28800, true, 0, "B28027AD-FE46-443D-8B95-190400F891E1", 0 ); // Update Volunteer Group Role
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "B28027AD-FE46-443D-8B95-190400F891E1", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "Group", "Group", "", 0, @"", "B9694F30-C3C6-4F74-97D9-A34066F60955" ); // Update Volunteer Group Role:Group
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "B28027AD-FE46-443D-8B95-190400F891E1", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 1, @"", "5CDCEF02-C140-4510-838B-008295A2496C" ); // Update Volunteer Group Role:Person
            RockMigrationHelper.AddAttributeQualifier( "5CDCEF02-C140-4510-838B-008295A2496C", "EnableSelfSelection", @"False", "CAF980A2-5656-4746-843C-5AC12E46A003" ); // Update Volunteer Group Role:Person:EnableSelfSelection
            RockMigrationHelper.UpdateWorkflowActivityType( "B28027AD-FE46-443D-8B95-190400F891E1", true, "Update Volunteer Group Role", "", true, 0, "D7419067-775B-4137-8E0B-B2662AD2E38E" ); // Update Volunteer Group Role:Update Volunteer Group Role
            RockMigrationHelper.UpdateWorkflowActionType( "D7419067-775B-4137-8E0B-B2662AD2E38E", "Update Volunteer Group Role", 0, "1C6D926A-63CD-4355-B9D1-9662669373C4", true, false, "", "", 1, "", "8CEC699F-C100-4CEC-A3EC-FF9F06F3EBCA" ); // Update Volunteer Group Role:Update Volunteer Group Role:Update Volunteer Group Role
            RockMigrationHelper.UpdateWorkflowActionType( "D7419067-775B-4137-8E0B-B2662AD2E38E", "Complete Workflow", 1, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "C7F27558-5E8B-4BE0-9F4B-06836ED90C82" ); // Update Volunteer Group Role:Update Volunteer Group Role:Complete Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "8CEC699F-C100-4CEC-A3EC-FF9F06F3EBCA", "629FA651-5E80-49D6-965D-7756F7D7E7FD", @"" ); // Update Volunteer Group Role:Update Volunteer Group Role:Update Volunteer Group Role:Order
            RockMigrationHelper.AddActionTypeAttributeValue( "8CEC699F-C100-4CEC-A3EC-FF9F06F3EBCA", "6A6F81E1-CC97-47A1-9407-1B151D4E7D21", @"False" ); // Update Volunteer Group Role:Update Volunteer Group Role:Update Volunteer Group Role:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "8CEC699F-C100-4CEC-A3EC-FF9F06F3EBCA", "1AA77A23-3749-4592-B92F-D9E7DB09190E", @"5cdcef02-c140-4510-838b-008295a2496c" ); // Update Volunteer Group Role:Update Volunteer Group Role:Update Volunteer Group Role:Person
            RockMigrationHelper.AddActionTypeAttributeValue( "8CEC699F-C100-4CEC-A3EC-FF9F06F3EBCA", "1F3A0DD1-CE69-4823-9471-445255B42E1E", @"b9694f30-c3c6-4f74-97d9-a34066f60955" ); // Update Volunteer Group Role:Update Volunteer Group Role:Update Volunteer Group Role:Group
            RockMigrationHelper.AddActionTypeAttributeValue( "C7F27558-5E8B-4BE0-9F4B-06836ED90C82", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // Update Volunteer Group Role:Update Volunteer Group Role:Complete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "C7F27558-5E8B-4BE0-9F4B-06836ED90C82", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // Update Volunteer Group Role:Update Volunteer Group Role:Complete Workflow:Order

            #endregion

            #region EntityTypes
            RockMigrationHelper.UpdateEntityType( "com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.MarkVolunteerAsLeft", "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", false, true );
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "35071825-C5AA-4A5F-9305-DF9D01A4E5D3" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.MarkVolunteerAsLeft:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Group", "Group", "Workflow Attribute that contains the group the person is in.", 1, @"", "CED7B015-A9CA-49A7-8352-1B9B204E0438" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.MarkVolunteerAsLeft:Group
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to add in the volunteer table.", 0, @"", "815ADE1D-34B0-4598-B04D-2F1FEEC7CAB2" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.MarkVolunteerAsLeft:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "4C4F5FCF-AB46-4980-B3B5-099294D738F7" ); // com.bricksandmortarstudio.TheCrossing.Workflow.Action.VolunteerTracking.MarkVolunteerAsLeft:Order
            #endregion

            #region Mark Old Volunteer as 'Left'

            RockMigrationHelper.UpdateWorkflowType( false, true, "Mark Old Volunteer as 'Left'", "This workflow will mark down the date an old volunteer left the serving team.", "DCD6A16C-C9DE-46F4-840A-08671635A149", "Work", "fa fa-list-ol", 28800, true, 0, "406C9CAD-19EB-40A4-A2C8-E578066DD535", 0 ); // Mark Old Volunteer as 'Left'
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "406C9CAD-19EB-40A4-A2C8-E578066DD535", "F4399CEF-827B-48B2-A735-F7806FCFE8E8", "Group", "Group", "", 0, @"", "ADB715F2-E1EA-4124-8970-023D5308211C" ); // Mark Old Volunteer as 'Left':Group
            RockMigrationHelper.UpdateWorkflowTypeAttribute( "406C9CAD-19EB-40A4-A2C8-E578066DD535", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "", 1, @"", "9565BCD4-6215-4CA2-BC9D-CCDE65F40F52" ); // Mark Old Volunteer as 'Left':Person
            RockMigrationHelper.AddAttributeQualifier( "9565BCD4-6215-4CA2-BC9D-CCDE65F40F52", "EnableSelfSelection", @"False", "DBCEEF4E-2644-482E-9DDD-176A46B7C69E" ); // Mark Old Volunteer as 'Left':Person:EnableSelfSelection
            RockMigrationHelper.UpdateWorkflowActivityType( "406C9CAD-19EB-40A4-A2C8-E578066DD535", true, "Mark Old Volunteer as 'Left'", "", true, 0, "F0F3B05A-80E1-4211-88F6-23DC2B6E2DBA" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left'
            RockMigrationHelper.UpdateWorkflowActionType( "F0F3B05A-80E1-4211-88F6-23DC2B6E2DBA", "Mark Old Volunteer as 'Left'", 0, "C50C272C-389B-4EE9-A3CA-B1DCCE801D7B", true, false, "", "", 1, "", "0EF50535-8760-484F-ADA2-F6C7E511640F" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left'
            RockMigrationHelper.UpdateWorkflowActionType( "F0F3B05A-80E1-4211-88F6-23DC2B6E2DBA", "Complete Workflow", 1, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "0A7CAF04-517F-488A-8718-3B25F079F596" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Complete Workflow
            RockMigrationHelper.AddActionTypeAttributeValue( "0EF50535-8760-484F-ADA2-F6C7E511640F", "4C4F5FCF-AB46-4980-B3B5-099294D738F7", @"" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Order
            RockMigrationHelper.AddActionTypeAttributeValue( "0EF50535-8760-484F-ADA2-F6C7E511640F", "35071825-C5AA-4A5F-9305-DF9D01A4E5D3", @"False" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Active
            RockMigrationHelper.AddActionTypeAttributeValue( "0EF50535-8760-484F-ADA2-F6C7E511640F", "815ADE1D-34B0-4598-B04D-2F1FEEC7CAB2", @"9565bcd4-6215-4ca2-bc9d-ccde65f40f52" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Person
            RockMigrationHelper.AddActionTypeAttributeValue( "0EF50535-8760-484F-ADA2-F6C7E511640F", "CED7B015-A9CA-49A7-8352-1B9B204E0438", @"adb715f2-e1ea-4124-8970-023d5308211c" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Group
            RockMigrationHelper.AddActionTypeAttributeValue( "0A7CAF04-517F-488A-8718-3B25F079F596", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Complete Workflow:Active
            RockMigrationHelper.AddActionTypeAttributeValue( "0A7CAF04-517F-488A-8718-3B25F079F596", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // Mark Old Volunteer as 'Left':Mark Old Volunteer as 'Left':Complete Workflow:Order

            #endregion
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
