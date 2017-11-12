<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UpdateVolunteers.ascx.cs" Inherits="RockWeb.Plugins.com_bricksandmortarstudio.Crossing.UpdateVolunteers" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox ID="nbInfo" runat="server" Visible="True" NotificationBoxType="Info">
            If you need to do a first-time import of your volunteers into the Volunteer Membership table, or want to track additional grouptypes and need to add them to the table,
            select the group types from the checkboxlist below and click 'Import'
        </Rock:NotificationBox>
        <Rock:NotificationBox ID="nbResult" runat="server" Visible="false" />

        <Rock:BootstrapButton runat="server" ID="btnImport" CssClass="btn btn-primary" Text="Import Volunteers" OnClick="btnImport_Click" DataLoadingText="Importing" />

    </ContentTemplate>
</asp:UpdatePanel>
