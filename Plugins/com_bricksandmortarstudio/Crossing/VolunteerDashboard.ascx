<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VolunteerDashboard.ascx.cs" Inherits="RockWeb.Plugins.com_bricksandmortarstudio.Crossing.VolunteerDashboard" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-star"></i>Blank List Block</h1>
            </div>
            <div class="panel-body">
                <Rock:SlidingDateRangePicker ID="drpSlidingDateRange" runat="server" Label="Sunday Date Range"
                    EnabledSlidingDateRangeTypes="DateRange" OnSelectedDateRangeChanged="drpSlidingDateRange_SelectedDateRangeChanged"/>

                <div class="grid grid-panel">
                    <Rock:Grid ID="gList" runat="server" AllowSorting="true">
                        <Columns>
                            <Rock:RockBoundField DataField="Ministry" HeaderText="Ministry" SortExpression="Ministry" />
                            <Rock:RockBoundField DataField="Director" HeaderText="Reports To" SortExpression="Director" />
                            <Rock:RockBoundField DataField="DateRange" HeaderText="Time Period" SortExpression="DateRange" />
                            <Rock:RockBoundField DataField="StartingVolunteers" HeaderText="Starting Volunteers" SortExpression="StartingVolunteers" />
                            <Rock:RockBoundField DataField="NewVolunteers" HeaderText="New Volunteers" SortExpression="NewVolunteers" />
                            <Rock:RockBoundField DataField="LostVolunteers" HeaderText="Lost Volunteers" SortExpression="LostVolunteers" />
                            <Rock:RockBoundField DataField="TotalVolunteers" HeaderText="Total Volunteers" SortExpression="TotalVolunteers" />
                            <Rock:RockBoundField DataField="VolunteerGoal" HeaderText="Goal" SortExpression="VolunteerGoal" />
                            <Rock:RockBoundField DataField="VolunteerPercent" HeaderText="Goal %" SortExpression="VolunteerPercent" />
                            <Rock:RockBoundField DataField="StartingLeaders" HeaderText="Starting Leaders" SortExpression="StartingLeaders" />
                            <Rock:RockBoundField DataField="NewLeaders" HeaderText="New Leaders" SortExpression="NewLeaders" />
                            <Rock:RockBoundField DataField="LostLeaders" HeaderText="Leaders Lost" SortExpression="LostLeaders" />
                            <Rock:RockBoundField DataField="TotalLeaders" HeaderText="Final Leaders" SortExpression="TotalLeaders" />
                            <Rock:RockBoundField DataField="LeaderGoal" HeaderText="Goal Leaders" SortExpression="LeaderGoal" />
                            <Rock:RockBoundField DataField="LeaderPercent" HeaderText="% Goal" SortExpression="LeaderPercent" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
