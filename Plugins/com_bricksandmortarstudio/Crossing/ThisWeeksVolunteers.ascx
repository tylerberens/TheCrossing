<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ThisWeeksVolunteers.ascx.cs" Inherits="Plugins.com_bricksandmortarstudio.Crossing.ThisWeeksVolunteers" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:HiddenField runat="server" ID="hfGroupGuid"/>
        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
            
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-star"></i> Who's Serving This Week?</h1>
            </div>
            <div class="panel-body">
                <div class="grid grid-panel">
                     <Rock:GridFilter ID="gFilter" runat="server" OnApplyFilterClick="gFilter_ApplyFilterClick"  OnClearFilterClick="gFilter_OnClearFilterClick" OnDisplayFilterValue="gFilter_OnDisplayFilterValue">
                        <Rock:GroupPicker runat="server" Label="Group" ID="gpGroup"/>
                    </Rock:GridFilter>
                    <Rock:Grid ID="gList" runat="server" AllowSorting="true" >
                        <Columns>
                            <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <Rock:RockBoundField DataField="GroupName" HeaderText="Group" SortExpression="GroupName" />
                            <Rock:RockBoundField DataField="ParentGroup.Name" HeaderText="Parent Group" SortExpression="ParentGroup.Name"/>
                        </Columns>
                    </Rock:Grid>
                </div>
            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
