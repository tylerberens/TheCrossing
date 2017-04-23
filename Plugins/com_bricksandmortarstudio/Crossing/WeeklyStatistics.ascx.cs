// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using System.Text;

using com.bricksandmortarstudio.TheCrossing.Attribute;

namespace RockWeb.Plugins.com_bricksandmortarstudio.Crossing
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Weekly Statistics" )]
    [Category( "com_bricksandmortarstudio > Crossing" )]
    [Description( "Dashboard to keep track of volunteers." )]
    [MetricCategoriesField( "Metric Categories", "The metrics in these metric categories will be displayed as 'Areas', with 'Headcount' listed as the subarea.", false )]
    [GroupsField( "Groups", "The groups listed here will be displayed as 'Areas', with their child groups being listed as the subareas'", false )]
    public partial class WeeklyStatistics : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        // used for public / protected properties

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            gThisYear.GridRebind += gThisYear_GridRebind;
            gLastYear.GridRebind += gLastYear_GridRebind;

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                dpWeek.SelectedDate = DateTime.Now.Date;
                BindGrids();
            }
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Handles the GridRebind event of the gThisYear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gThisYear_GridRebind( object sender, EventArgs e )
        {
            BindGrids();
        }

        /// <summary>
        /// Handles the GridRebind event of the gLastYear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gLastYear_GridRebind( object sender, EventArgs e )
        {
            BindGrids();
        }

        protected void dpWeek_TextChanged( object sender, EventArgs e )
        {
            BindGrids();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrids()
        {
            DateTime pickerDate = dpWeek.SelectedDate.Value;
            var endDate = pickerDate.SundayDate();
            var startDate = endDate.AddDays( -6 );
            var lastYearEndDate = endDate.AddDays( -( 52 * 7 ) );
            var lastYearStartDate = lastYearEndDate.AddDays( -6 );

            List<StatisticRow> thisYearDataSource = GetDataForDateRange( startDate, endDate.AddDays( 1 ) );
            List<StatisticRow> lastYearDataSource = GetDataForDateRange( lastYearStartDate, lastYearEndDate.AddDays( 1 ) );

            if ( startDate.Year == endDate.Year )
            {
                if ( startDate.Month == endDate.Month )
                {
                    lThisYearDate.Text = String.Format( "{0} {1}/{2}, {3}", startDate.ToString( "MMM" ), startDate.Day, endDate.Day, startDate.ToString( "yyyy" ) );
                }
                else
                {
                    lThisYearDate.Text = String.Format( "{0} {1} - {2} {3}, {4}", startDate.ToString( "MMM" ), startDate.Day, endDate.ToString( "MMM" ), endDate.Day, startDate.ToString( "yyyy" ) );
                }
            }
            else
            {
                lThisYearDate.Text = String.Format( "{0} {1}, {3} - {4} {5}, {6}", startDate.ToString( "MMM" ), startDate.Day, startDate.ToString( "yyyy" ), endDate.ToString( "MMM" ), endDate.Day, endDate.ToString( "yyyy" ) );
            }

            if ( lastYearStartDate.Year == lastYearEndDate.Year )
            {
                if ( lastYearStartDate.Month == lastYearEndDate.Month )
                {
                    lLastYearDate.Text = String.Format( "{0} {1}/{2}, {3}", lastYearStartDate.ToString( "MMM" ), lastYearStartDate.Day, lastYearEndDate.Day, lastYearStartDate.ToString( "yyyy" ) );
                }
                else
                {
                    lLastYearDate.Text = String.Format( "{0} {1} - {2} {3}, {4}", lastYearStartDate.ToString( "MMM" ), lastYearStartDate.Day, lastYearEndDate.ToString( "MMM" ), lastYearEndDate.Day, lastYearStartDate.ToString( "yyyy" ) );
                }
            }
            else
            {
                lLastYearDate.Text = String.Format( "{0} {1}, {3} - {4} {5}, {6}", lastYearStartDate.ToString( "MMM" ), lastYearStartDate.Day, lastYearStartDate.ToString( "yyyy" ), lastYearEndDate.ToString( "MMM" ), lastYearEndDate.Day, lastYearEndDate.ToString( "yyyy" ) );
            }

            StringBuilder sb = new StringBuilder();
            var noteList = thisYearDataSource.Max( sr => sr.MetricNote ).SplitDelimitedValues();
            for ( int i = 0; i < noteList.Count(); i++ )
            {
                if ( i % 2 == 0 )
                {
                    sb.AppendFormat( "<b> {0}:</b>", noteList[i] );
                }
                else
                {
                    sb.AppendFormat( ", {0}", noteList[i] );
                }
            }

            lThisYearNote.Text = sb.ToString();

            sb = new StringBuilder();
            noteList = lastYearDataSource.Max( sr => sr.MetricNote ).SplitDelimitedValues();
            for ( int i = 0; i < noteList.Count(); i++ )
            {
                if ( i % 2 == 0 )
                {
                    sb.AppendFormat( "<b> {0}:</b>", noteList[i] );
                }
                else
                {
                    sb.AppendFormat( ", {0}", noteList[i] );
                }
            }

            lLastYearNote.Text = sb.ToString();

            gThisYear.DataSource = thisYearDataSource
                .OrderByDescending( sr => lastYearDataSource.Select( ly => ly.RowId ).Contains( sr.RowId ) )
                .ThenBy( sr => sr.SortValue )
                .ThenBy( sr => sr.Area )
                .ThenBy( sr => sr.Subarea )
                .ThenBy( sr => sr.IsTotal )
                .ThenBy( sr => sr.Service )
                .ToList();
            gThisYear.DataBind();

            gLastYear.DataSource = lastYearDataSource
                .OrderByDescending( sr => thisYearDataSource.Select( ly => ly.RowId ).Contains( sr.RowId ) )
                .ThenBy( sr => sr.SortValue )
                .ThenBy( sr => sr.Area )
                .ThenBy( sr => sr.Subarea )
                .ThenBy( sr => sr.IsTotal )
                .ThenBy( sr => sr.Service )
                .ToList();
            gLastYear.DataBind();

        }

        private List<StatisticRow> GetDataForDateRange( DateTime startDate, DateTime endDate )
        {
            var entityTypeGroupGuid = Rock.SystemGuid.EntityType.GROUP.AsGuid();
            var entityTypePersonGuid = Rock.SystemGuid.EntityType.PERSON.AsGuid();
            var entityTypeScheduleGuid = Rock.SystemGuid.EntityType.SCHEDULE.AsGuid();

            RockContext rockContext = new RockContext();
            MetricCategoryService metricCategoryService = new MetricCategoryService( rockContext );
            MetricValueService metricValueService = new MetricValueService( rockContext );
            ScheduleService scheduleService = new ScheduleService( rockContext );
            GroupService groupService = new GroupService( rockContext );
            AttendanceService attendanceService = new AttendanceService( rockContext );

            var metricCategoryGuidList = GetAttributeValue( "MetricCategories" ).SplitDelimitedValues().AsGuidList();
            var groupGuidList = GetAttributeValue( "Groups" ).SplitDelimitedValues().AsGuidList();

            var groups = groupService.GetByGuids( groupGuidList );
            var metrics = metricCategoryService.GetByGuids( metricCategoryGuidList ).Select( mc => mc.Metric ).Distinct().ToList();

            var datasource = new List<StatisticRow>().AsEnumerable();

            foreach ( var metric in metrics )
            {
                var metricData = metricValueService.Queryable().Where( mv =>
                    mv.MetricValueDateTime >= startDate &&
                    mv.MetricValueDateTime <= endDate &&
                    mv.MetricId == metric.Id &&
                    mv.MetricValuePartitions.Where( mvp => mvp.MetricPartition.EntityType.Guid == entityTypeScheduleGuid ).FirstOrDefault().EntityId.HasValue
                    )
                 .GroupBy( mv => mv.MetricValuePartitions.Where( mvp => mvp.MetricPartition.EntityType.Guid == entityTypeScheduleGuid ).FirstOrDefault().EntityId.Value )
                 .Select( mv => new StatisticRow
                 {
                     RowId = metric.Id + "-" + mv.Key,
                     SortValue = 0,
                     IsTotal = false,
                     Area = metric.Title,
                     Subarea = "HeadCount",
                     Service = scheduleService.Get( mv.Key ).Name,
                     iCalendarContent = scheduleService.Get( mv.Key ).iCalendarContent,
                     Count = mv.Sum( a => a.YValue ).HasValue ? Int32.Parse( mv.Sum( a => a.YValue ).Value.ToString() ) : 0,
                     Volunteers = 0,
                     Total = mv.Sum( a => a.YValue ).HasValue ? Int32.Parse( mv.Sum( a => a.YValue ).Value.ToString() ) : 0,
                     MetricNote = mv.Max( a => a.Note )
                 } );

                if ( metricData.Any() )
                {
                    var subTotalRow = metricData.Concat( new List<StatisticRow>{ new StatisticRow
                    {
                        RowId = metric.Id.ToString(),
                        SortValue = 0,
                        IsTotal = true,
                        Area = metric.Title,
                        Subarea = "HeadCount",
                        Service = "Sub-Total",
                        Count = metricData.Sum(mv => mv.Count),
                        Volunteers = metricData.Sum(mv => mv.Volunteers),
                        Total = metricData.Sum(mv => mv.Total)
                    } }.AsQueryable() );

                    datasource = datasource.Concat( subTotalRow );
                }

            }

            foreach ( var group in groups )
            {
                if ( !group.Groups.Any() )
                {
                    var childDescendantGroupIds = groupService.GetAllDescendents( group.Id ).Select( g => g.Id ).ToList();
                    var childGroupData = attendanceService.Queryable().Where( a =>
                        a.StartDateTime >= startDate &&
                        a.StartDateTime <= endDate &&
                         a.GroupId == group.Id )
                    .GroupBy( a => a.ScheduleId )
                      .Select( a => new StatisticRow
                      {
                          RowId = group.Id + "-" + a.Key,
                          SortValue = 1,
                          IsTotal = false,
                          Area = group.Name,
                          Subarea = group.Name,
                          Service = a.FirstOrDefault().Schedule.Name,
                          iCalendarContent = a.FirstOrDefault().Schedule.iCalendarContent,
                          Count = a.Count(),
                          Volunteers = 0,
                          Total = a.Count()
                      } );

                    if ( childGroupData.Any() )
                    {
                        var subTotalRow = childGroupData.ToList().Concat( new List<StatisticRow>{ new StatisticRow
                        {
                            RowId = group.Id.ToString(),
                            SortValue = 1,
                            IsTotal = true,
                            Area = group.Name,
                            Subarea = group.Name,
                            Service = "Sub-Total",
                            Count = childGroupData.Sum(cg => cg.Count),
                            Volunteers = childGroupData.Sum(cg => cg.Volunteers),
                            Total = childGroupData.Sum(cg => cg.Total)
                        } } );

                        datasource = datasource.Concat( subTotalRow );
                    }
                }

                foreach ( var childGroup in group.Groups )
                {
                    var childDescendantGroupIds = groupService.GetAllDescendents( childGroup.Id ).Select( g => g.Id ).ToList();
                    var childGroupData = attendanceService.Queryable().Where( a =>
                        a.GroupId.HasValue &&
                        a.StartDateTime >= startDate &&
                        a.StartDateTime <= endDate &&
                        ( a.GroupId == childGroup.Id ||
                            childDescendantGroupIds.Contains( a.GroupId.Value ) ) )
                        .GroupBy( a => a.ScheduleId )
                        .Select( a => new StatisticRow
                        {
                            RowId = childGroup.Id + "-" + a.Key,
                            SortValue = 1,
                            IsTotal = false,
                            Area = group.Name,
                            Subarea = childGroup.Name,
                            Service = a.FirstOrDefault().Schedule.Name,
                            iCalendarContent = a.FirstOrDefault().Schedule.iCalendarContent,
                            Count = a.Count(),
                            Volunteers = 0,
                            Total = a.Count()
                        } );

                    if ( childGroupData.Any() )
                    {
                        var subTotalRow = childGroupData.ToList().Concat( new List<StatisticRow>{ new StatisticRow
                        {
                            RowId = childGroup.Id.ToString(),
                            SortValue = 1,
                            IsTotal = true,
                            Area = group.Name,
                            Subarea = childGroup.Name,
                            Service = "Sub-Total",
                            Count = childGroupData.Sum(cg => cg.Count),
                            Volunteers = childGroupData.Sum(cg => cg.Volunteers),
                            Total = childGroupData.Sum(cg => cg.Total)
                        } }.AsQueryable() );

                        datasource = datasource.Concat( subTotalRow );
                    }

                }
            }

            datasource = datasource.Concat( new List<StatisticRow>{ new StatisticRow
                {
                    RowId = "Total",
                    SortValue = 2,
                    IsTotal = true,
                    Area = "Grand Total",
                    Subarea = "Total",
                    Service = "Total",
                    Count = datasource.Where(ds=> ds.IsTotal).Sum(cg => cg.Count),
                    Volunteers = datasource.Where(ds=> ds.IsTotal).Sum(cg => cg.Volunteers),
                    Total = datasource.Where(ds=> ds.IsTotal).Sum(cg => cg.Total)
                } }.AsQueryable() );

            return datasource.ToList();
        }

        #endregion

        #region Helper Classes

        public class StatisticRow
        {
            public String RowId { get; set; }

            public int SortValue { get; set; }

            public bool IsTotal { get; set; }

            public String Area { get; set; }

            public String Subarea { get; set; }

            public String Service { get; set; }

            public String iCalendarContent { get; set; }

            public int Count { get; set; }

            public int Volunteers { get; set; }

            public int Total { get; set; }

            public String MetricNote { get; set; }
        }

        #endregion
    }
}