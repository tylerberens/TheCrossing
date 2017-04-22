<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WeeklyStatistics.ascx.cs" Inherits="RockWeb.Plugins.com_bricksandmortarstudio.Crossing.WeeklyStatistics" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Weekly Statistics</h1>
            </div>
            <div class="panel-body">
                <Rock:DatePicker ID="dpWeek" runat="server" Label="Week" />

                <div class="row">
                    <div class="col-md-6">
                        <div style="text-align: center">
                            <div class="row">
                                <h3>This Year</h3>
                                <asp:Literal ID="lThisYearDate" runat="server" />
                            </div>
                            <asp:Literal ID="lThisYearNote" runat="server" />
                        </div>
                        <div class="grid grid-panel">
                            <Rock:Grid ID="gThisYear" runat="server" AllowSorting="true">
                                <Columns>
                                    <Rock:RockBoundField DataField="Area" HeaderText="Area" SortExpression="Area" />
                                    <Rock:RockBoundField DataField="Subarea" HeaderText="Subarea" SortExpression="Subarea" />
                                    <Rock:RockBoundField DataField="Service" HeaderText="Service" SortExpression="Service" />
                                    <Rock:RockBoundField DataField="Count" HeaderText="Count" SortExpression="Count" />
                                    <Rock:RockBoundField DataField="Volunteers" HeaderText="Volunteers" SortExpression="Volunteers" />
                                    <Rock:RockBoundField DataField="Total" HeaderText="Total" SortExpression="Total" />
                                </Columns>
                            </Rock:Grid>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div style="text-align: center">
                            <div class="row">
                                <h3>Last Year</h3>
                                <asp:Literal ID="lLastYearDate" runat="server" />
                            </div>
                            <asp:Literal ID="lLastYearNote" runat="server" />
                        </div>
                        <div class="grid grid-panel">
                            <Rock:Grid ID="gLastYear" runat="server" AllowSorting="true">
                                <Columns>
                                    <Rock:RockBoundField DataField="Area" HeaderText="Area" SortExpression="Area" />
                                    <Rock:RockBoundField DataField="Subarea" HeaderText="Subarea" SortExpression="Subarea" />
                                    <Rock:RockBoundField DataField="Service" HeaderText="Service" SortExpression="Service" />
                                    <Rock:RockBoundField DataField="Count" HeaderText="Count" SortExpression="Count" />
                                    <Rock:RockBoundField DataField="Volunteers" HeaderText="Volunteers" SortExpression="Volunteers" />
                                    <Rock:RockBoundField DataField="Total" HeaderText="Total" SortExpression="Total" />
                                </Columns>
                            </Rock:Grid>
                        </div>
                    </div>
                </div>


            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
