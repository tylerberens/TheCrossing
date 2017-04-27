<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VolunteerDashboard.ascx.cs" Inherits="RockWeb.Plugins.com_bricksandmortarstudio.Crossing.VolunteerDashboard" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Serving Dashboard</h1>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-1">
                        <Rock:DatePicker ID="dpStart" runat="server" OnTextChanged="dp_TextChanged" AutoPostBack="true" Label="Start" />
                    </div>
                    <div class="col-md-1">
                        <Rock:DatePicker ID="dpEnd" runat="server" OnTextChanged="dp_TextChanged" AutoPostBack="true" Label="End" />
                    </div>
                    <div class="col-md-10">
                        <div class="row">
                            <div class="col-md-3 col-md-offset-1">
                                <h4>Volunteers</h4>
                                <div class="grid grid-panel">
                                    <Rock:Grid ID="gVolunteers" runat="server" AllowSorting="true" ShowFooter="false" DisplayType="Light">
                                        <Columns>
                                            <Rock:RockBoundField DataField="StartingVolunteers" HeaderText="Begin" SortExpression="StartingVolunteers" />
                                            <Rock:RockBoundField DataField="NewVolunteers" HeaderText="New" SortExpression="NewVolunteers" />
                                            <Rock:RockBoundField DataField="LostVolunteers" HeaderText="Lost" SortExpression="LostVolunteers" />
                                            <Rock:RockBoundField DataField="TotalVolunteers" HeaderText="End" SortExpression="TotalVolunteers" />
                                            <Rock:RockBoundField DataField="VolunteerGoal" HeaderText="Goal" SortExpression="VolunteerGoal" />
                                            <Rock:RockBoundField DataField="VolunteerPercent" HeaderText="Goal %" SortExpression="VolunteerPercent" />
                                        </Columns>
                                    </Rock:Grid>
                                </div>
                            </div>
                            <div class="col-md-3 col-md-offset-1">
                                <h4>Leaders</h4>
                                <div class="grid grid-panel">
                                    <Rock:Grid ID="gLeaders" runat="server" AllowSorting="true" ShowFooter="false" DisplayType="Light">
                                        <Columns>
                                            <Rock:RockBoundField DataField="StartingLeaders" HeaderText="Begin" SortExpression="StartingLeaders" />
                                            <Rock:RockBoundField DataField="NewLeaders" HeaderText="New " SortExpression="NewLeaders" />
                                            <Rock:RockBoundField DataField="LostLeaders" HeaderText="Lost" SortExpression="LostLeaders" />
                                            <Rock:RockBoundField DataField="TotalLeaders" HeaderText="End" SortExpression="TotalLeaders" />
                                            <Rock:RockBoundField DataField="LeaderGoal" HeaderText="Goal" SortExpression="LeaderGoal" />
                                            <Rock:RockBoundField DataField="LeaderPercent" HeaderText="Goal %" SortExpression="LeaderPercent" />
                                        </Columns>
                                    </Rock:Grid>
                                </div>
                            </div>
                            <div class="col-md-2 col-md-offset-1">
                                <div class="grid grid-panel">
                                    <h4>Uniques</h4>
                                    <Rock:Grid ID="gUniques" runat="server" AllowSorting="true" ShowFooter="false" DisplayType="Light">
                                        <Columns>
                                            <Rock:RockBoundField DataField="UniqueVolunteers" HeaderText="Volunteers" SortExpression="Ministry" />
                                            <Rock:RockBoundField DataField="UniqueLeaders" HeaderText="Leaders" SortExpression="Director" />
                                            <Rock:RockBoundField DataField="UniqueTotal" HeaderText="Total" SortExpression="DateRange" />
                                        </Columns>
                                    </Rock:Grid>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                </br></br>
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
