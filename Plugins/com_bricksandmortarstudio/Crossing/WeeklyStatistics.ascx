<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WeeklyStatistics.ascx.cs" Inherits="RockWeb.Plugins.com_bricksandmortarstudio.Crossing.WeeklyStatistics" %>

<style>
    .is-bold {
        font-weight: bold;
    }
</style>

<!-- TODO Remove in v7 -->
<script src="https://cdn.jsdelivr.net/npm/clipboard@1/dist/clipboard.min.js"></script>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
       <asp:HiddenField runat="server" ID="hfUrl" ClientIDMode="Static" />

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
            <asp:Literal runat="server" ID="dLiteral"></asp:Literal>
            <div class="panel-heading">
                <h1 class="panel-title">Weekly Statistics</h1> 

                <button id="btnCopyToClipboard" style="margin-left: 1em;"
                        data-toggle="tooltip" data-placement="top" data-trigger="hover" data-delay="250" title="Copy Report Link to Clipboard"
                        class="btn btn-link padding-all-none btn-copy-to-clipboard"
                        onclick="$(this).attr('data-original-title', 'Copied').tooltip('show').attr('data-original-title', 'Copy Link to Clipboard');return false;">
                     <i class='fa fa-clipboard'></i>
                </button>
            </div>
            <div class="panel-body" style="margin-bottom: 2em">
                <Rock:DatePicker ID="dpWeek" runat="server" Label="Week" OnTextChanged="dpWeek_TextChanged" AutoPostBack="true" />

                <div class="row">
                    <div class="col-md-6">
                        <div class="row">
                            <div style="text-align: center">
                                <div class="row">
                                    <p style="font-size: x-large; display: inline;">This Year</p>
                                    <asp:Literal ID="lThisYearDate" runat="server" />
                                </div>
                                <asp:Literal ID="lThisYearNote" runat="server" />
                            </div>
                        </div>
                        </br></br>
                        <div class="row">
                            <div class="col-md-10 col-md-offset-1">
                                <div class="grid grid-panel">
                                    <Rock:Grid ID="gThisYear" runat="server" AllowPaging="False" ShowActionRow="False" RowItemText="Attendance">
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

                    <div class="col-md-6">
                        <div class="row">
                            <div style="text-align: center">
                                <div class="row">
                                    <p style="font-size: x-large; display: inline;">Last Year</p>
                                    <asp:Literal ID="lLastYearDate" runat="server" />
                                </div>
                                <asp:Literal ID="lLastYearNote" runat="server" />
                            </div>
                        </div>
                        </br></br>
                        <div class="row">
                            <div class="col-md-10 col-md-offset-1">
                                <div class="grid grid-panel">
                                    <Rock:Grid ID="gLastYear" AllowPaging="False" ShowActionRow="False"  runat="server" RowItemText="Attendance">
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
                </div>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>

<script>
    new Clipboard('#btnCopyToClipboard', {
        text: function (trigger) {
            return  $('#hfUrl').val();
        }
    });
</script>