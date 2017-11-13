using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Attribute;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using com.bricksandmortarstudio.checkinextensions.Utils;

using com.bricksandmortarstudio.TheCrossing.Attribute;

namespace RockWeb.Plugins.com_bricksandmortarstudio.Crossing
{
    [DisplayName( "Weekly Statistics" )]
    [Category( "com_bricksandmortarstudio > Crossing" )]
    [Description( "Dashboard to keep track of attendance and volunteers." )]
    [MetricCategoriesField( "Metrics", "The metrics listed here will be displayed as 'Areas', with 'Headcount' listed as the subarea.", false )]
    [GroupsField( "Attendance Groups", "The groups listed here will be displayed as 'Areas', with their child groups being listed as the subareas'", false )]
    [TextField( "Volunteer Group Attribute Key", "The key for the group attribute that ties attendance and volunteer teams together", false )]
    [GroupsField( "Service Volunteer Groups", "Attendance in the groups listed here and their child groups will be used towards the volunteer count for metric attendance", false )]
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
            gLastYear.RowDataBound += gLastYear_OnRowDataBound;
            gThisYear.RowDataBound += gThisYear_OnRowDataBound;
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
                var week = DateTime.Now.Date;
                if (Page.ClientQueryString.Contains("Week"))
                {
                    week = DateTime.Parse(Page.ClientQueryString);
                }
                dpWeek.SelectedDate = week;


                hfUrl.Value = GetCurrentUrl();

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
            hfUrl.Value = GetCurrentUrl();
            BindGrids();
        }

        private void gThisYear_OnRowDataBound( object sender, GridViewRowEventArgs e )
        {
            if ( e.Row.RowType == DataControlRowType.DataRow )
            {
                var statisticsRows = e.Row.DataItem as StatisticRow;

                if ( statisticsRows != null && statisticsRows.IsTotal )
                {
                    e.Row.AddCssClass( "is-bold" );
                }
            }
        }

        private void gLastYear_OnRowDataBound( object sender, GridViewRowEventArgs e )
        {
            if ( e.Row.RowType == DataControlRowType.DataRow )
            {
                var statisticsRows = e.Row.DataItem as StatisticRow;

                if ( statisticsRows != null && statisticsRows.IsTotal )
                {
                    e.Row.AddCssClass( "is-bold" );
                }
            }
        }

        #endregion

        #region Methods

        private String GetCurrentUrl()
        {
            var queryString = HttpUtility.ParseQueryString( Request.QueryString.ToString() );
            queryString.Set( "Week", dpWeek.SelectedDate.Value.ToShortDateString() );
            return Request.Url.AbsoluteUri + "?" + queryString;
        }

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
            var noteList = thisYearDataSource.Max( sr => sr.MetricNote ).SplitDelimitedValues( false );
            for ( int i = 0; i < noteList.Count(); i++ )
            {
                if ( i % 2 == 0 )
                {
                    if ( i != 0 )
                    {
                        sb.Append( ", " );
                    }
                    sb.AppendFormat( "<b> {0}:</b>", noteList[i] );
                }
                else
                {
                    sb.AppendFormat( " {0}", noteList[i] );
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
                .OrderBy( sr => sr.SortValue )
                .ThenBy( sr => sr.Area )
                .ThenBy( sr => sr.Subarea )
                .ThenBy( sr => sr.IsTotal )
                .ThenBy( sr => ( ( int ) sr.DayOfWeek + 6 ) % 7 )
                .ThenBy( sr => sr.StartTime )
                .ToList();
            gThisYear.DataBind();

            gLastYear.DataSource = lastYearDataSource
                .OrderBy( sr => sr.SortValue )
                .ThenBy( sr => sr.Area )
                .ThenBy( sr => sr.Subarea )
                .ThenBy( sr => sr.IsTotal )
                .ThenBy( sr => ( ( int ) sr.DayOfWeek + 6 ) % 7 )
                .ThenBy( sr => sr.StartTime )
                .ToList();
            gLastYear.DataBind();

        }

        private List<StatisticRow> GetDataForDateRange( DateTime startDate, DateTime endDate )
        {
            var entityTypeGroupGuid = Rock.SystemGuid.EntityType.GROUP.AsGuid();
            var groupEntityType = EntityTypeCache.Read( entityTypeGroupGuid );
            int entityTypeScheduleEntityId = EntityTypeCache.Read( Rock.SystemGuid.EntityType.SCHEDULE.AsGuid() ).Id;

            var rockContext = new RockContext();
            rockContext.Database.CommandTimeout = 2600;
            var metricService = new MetricService( rockContext );
            var metricValueService = new MetricValueService( rockContext );
            var scheduleService = new ScheduleService( rockContext );
            var groupService = new GroupService( rockContext );
            var attendanceService = new AttendanceService( rockContext );
            var attributeService = new AttributeService( rockContext );
            var attributeValueService = new AttributeValueService( rockContext );

            var metricCategoryGuidList = GetAttributeValue( "Metrics" ).SplitDelimitedValues().AsGuidList();
            var attendanceGroupGuidList = GetAttributeValue( "AttendanceGroups" ).SplitDelimitedValues().AsGuidList();
            var parentMetricVolunteerGroupGuids = GetAttributeValue( "ServiceVolunteerGroups" ).SplitDelimitedValues().AsGuidList();


            var attendanceGroups = groupService.GetByGuids( attendanceGroupGuidList );
            var parentMetricVolunteerGroups = groupService.GetByGuids( parentMetricVolunteerGroupGuids );
            var metrics = metricService.GetByGuids( metricCategoryGuidList ).Distinct().ToList();

            var datasource = new List<StatisticRow>();

            var metricVolunteerGroups = new List<Group>();
            foreach ( var parentMetricVolunteerGroup in parentMetricVolunteerGroups )
            {
                metricVolunteerGroups.Add( parentMetricVolunteerGroup );
                metricVolunteerGroups.AddRange( groupService.GetAllDescendents( parentMetricVolunteerGroup.Id ) );
            }

            var metricVolunteerGroupIds = metricVolunteerGroups.Select( g => g.Id ).ToList();

            var metricVolunteerAttendanceData = attendanceService.Queryable().Where( a =>
                                a.GroupId.HasValue &&
                                metricVolunteerGroupIds.Contains( a.GroupId.Value ) && a.StartDateTime >= startDate && a.StartDateTime <= endDate );

            foreach ( var metric in metrics )
            {
                var metricData = metricValueService.Queryable( "MetricValuePartitions" ).Where( mv =>
                                                              mv.MetricValueDateTime >= startDate &&
                                                              mv.MetricValueDateTime <= endDate &&
                                                              mv.MetricId == metric.Id &&
                                                              mv.MetricValuePartitions.FirstOrDefault(
                                                                    mvp =>
                                                                        mvp.MetricPartition.EntityTypeId ==
                                                                        entityTypeScheduleEntityId ).EntityId.HasValue
                                                   )
                                                   .GroupBy(
                                                       mv =>
                                                           mv.MetricValuePartitions.FirstOrDefault(
                                                                 mvp =>
                                                                     mvp.MetricPartition.EntityTypeId ==
                                                                     entityTypeScheduleEntityId ).EntityId.Value )
                                                   .ToList()
                                                   .Select( mv =>
                                                    {
                                                        var service = scheduleService.Get( mv.Key );
                                                        return new StatisticRow
                                                        {
                                                            ScheduleDateRanges =
                                                                GetScheduleDateRanges( service,
                                                                    startDate, endDate ),
                                                            RowId = metric.Id + "-" + mv.Key,
                                                            SortValue = 0,
                                                            IsTotal = false,
                                                            Area = metric.Title,
                                                            Subarea = "Head Count",
                                                            StartTime = service.WeeklyTimeOfDay ?? service.StartTimeOfDay,
                                                            DayOfWeek = service.WeeklyDayOfWeek ?? GetLastDayOfWeek( service, startDate, endDate ),
                                                            Service = service.Name,
                                                            Count =
                                                                mv.Sum( a => a.YValue ).HasValue
                                                                    ? decimal.ToInt32( mv.Sum( a => a.YValue ).Value )
                                                                    : 0,
                                                            MetricNote = mv.Max( a => a.Note ),
                                                            Value = mv
                                                        };
                                                    } )
                                                   .ToList();

                foreach ( var row in metricData )
                {
                    int volunteers = 0;
                    int total = row.Value.Sum( a => a.YValue ).HasValue ? decimal.ToInt32( row.Value.Sum( a => a.YValue ).Value ) : 0;

                    if ( metricVolunteerAttendanceData.Any() )
                    {
                        volunteers += row.ScheduleDateRanges.Sum( dateRange => metricVolunteerAttendanceData.Count( a => ( a.DidAttend == null || a.DidAttend.Value ) && a.StartDateTime >= dateRange.Start && a.StartDateTime <= dateRange.End ) );
                        row.Total = total + volunteers;
                        row.Volunteers = volunteers;
                    }
                }


                datasource.AddRange( metricData );

                if ( metricData.Count > 1 )
                {
                    var subTotalRow = new StatisticRow
                    {
                        RowId = metric.Id.ToString(),
                        SortValue = 0,
                        IsTotal = true,
                        Area = metric.Title,
                        Subarea = "Head Count",
                        Service = "Sub-Total",
                        Count = metricData.Sum( mv => mv.Count ),
                        Volunteers = metricData.Sum( mv => mv.Volunteers ),
                        Total = metricData.Sum( mv => mv.Total )
                    };

                    datasource.Add( subTotalRow );
                }

            }

            var totalRow = new StatisticRow
            {
                RowId = "HeadcountTotal",
                SortValue = 1,
                IsTotal = true,
                Area = "Head Count Total",
                Subarea = "Head Count",
                Service = "Total",
                Count = datasource.Where( row => !row.IsTotal ).Sum( row => row.Count ),
                Volunteers = datasource.Where( row => !row.IsTotal ).Sum( mv => mv.Volunteers ),
                Total = datasource.Where( row => !row.IsTotal ).Sum( mv => mv.Total )
            };

            datasource.Add( totalRow );

            string attributeKeyString = GetAttributeValue( "VolunteerGroupAttributeKey" );
            var volunteerGroupAttributeIdList = attributeService.Queryable()
                .Where( a => a.Key == attributeKeyString && a.EntityTypeQualifierColumn == "GroupTypeId" && a.EntityTypeId == groupEntityType.Id ).Select( a => a.Id );
            if ( volunteerGroupAttributeIdList.Any() )
            {
                // Find the groups that attribute values that have the maaping between group (the entityId) and the place they should be grouped with attending (value)
                var volunteerGroupMappingList = attributeValueService.Queryable().Where( av => volunteerGroupAttributeIdList.Contains( av.AttributeId ) && av.Value != null )
                    .ToList()
                    .Select( av => new
                    {
                        VolunteerAttendanceGroupGuid = av.Value.AsGuid(),
                        VolunteerGroupId = av.EntityId
                    } ).ToList();

                foreach ( var attendanceGroup in attendanceGroups )
                {
                    foreach ( var attendanceChildGroup in attendanceGroup.Groups )
                    {
                        var attendanceChildDescendantGroups = groupService.GetAllDescendents( attendanceChildGroup.Id ).ToList();
                        // Include child group in for cases where attendance needs to be mapped to an area not a specific group (production team isn't for a specific children's group -- it's associated with TC kids as a whole)
                        attendanceChildDescendantGroups.Add( attendanceChildGroup );
                        var attendanceChildDescendantGroupIds = attendanceChildDescendantGroups.Select( g => g.Id );

                        var volunteerGroupIds = volunteerGroupMappingList
                            .Where( vgm => attendanceChildDescendantGroups.Any( g => g.Guid == vgm.VolunteerAttendanceGroupGuid ) )
                            .Select( vgm => vgm.VolunteerGroupId ).ToList();
                        var volunteerGroupAttendance = attendanceService.Queryable()
                            .Where( a => volunteerGroupIds.Any( id => id != null && id == a.Group.Id ) && a.StartDateTime >= startDate && a.StartDateTime <= endDate )
                            .ToList();

                        var acg = attendanceChildGroup;
                        var childGroupAttendance = attendanceService.Queryable().Where( a =>
                            a.GroupId != null &&
                            a.StartDateTime >= startDate &&
                            a.StartDateTime <= endDate &&
                            ( a.GroupId == acg.Id ||
                                attendanceChildDescendantGroupIds.Any( id => id == a.GroupId ) )
                                && ( a.DidAttend == null || a.DidAttend.Value ) )
                            .GroupBy( a => a.ScheduleId )
                            .ToList();

                        // ag is created to prevent a warn "Access to foreach variable in closure."
                        var ag = attendanceGroup;
                        var statisticRows = childGroupAttendance.Select( a =>
                        {

                            var attendance = a.FirstOrDefault();
                            var scheduleDateRanges = GetScheduleDateRanges( attendance.Schedule, startDate,
                                endDate );
                            var row = new StatisticRow();
                            row.RowId = acg.Id + "-" + a.Key;
                            row.SortValue = 2;
                            row.IsTotal = false;
                            row.Area = ag.Name;
                            row.Subarea = acg.Name;
                            row.Service = attendance.Schedule.Name;
                            row.StartTime = attendance.Schedule.WeeklyTimeOfDay ?? attendance.Schedule.StartTimeOfDay;
                            row.DayOfWeek = attendance.Schedule.WeeklyDayOfWeek ?? GetLastDayOfWeek( attendance.Schedule, startDate, endDate );
                            row.Count = a.Count();
                            row.Volunteers = volunteerGroupAttendance.Count( b => scheduleDateRanges.Any( s => b.StartDateTime >= s.Start && b.StartDateTime <= s.End ) );
                            row.Total = a.Count() + volunteerGroupAttendance.Count( b => scheduleDateRanges.Any( s => b.StartDateTime >= s.Start && b.StartDateTime <= s.End ) );
                            return row;
                        } ).ToList();

                        datasource.AddRange( statisticRows );

                        if ( statisticRows.Count > 1 )
                        {
                            var subTotalRow = new StatisticRow
                            {
                                RowId = attendanceChildGroup.Id.ToString(),
                                SortValue = 2,
                                IsTotal = true,
                                Area = attendanceGroup.Name,
                                Subarea = attendanceChildGroup.Name,
                                Service = "Sub-Total",
                                Count = statisticRows.Sum( cg => cg.Count ),
                                Volunteers = statisticRows.Sum( cg => cg.Volunteers ),
                                Total = statisticRows.Sum( cg => cg.Total )
                            };

                            datasource.Add( subTotalRow );
                        }

                    }
                }
            }

            datasource.Add( new StatisticRow
            {
                RowId = "Total",
                SortValue = 3,
                IsTotal = true,
                Area = "Grand Total",
                Subarea = "Total",
                Service = "Total",
                Count = datasource.Where( ds => ds.IsTotal ).Sum( cg => cg.Count ),
                Volunteers = datasource.Where( ds => ds.IsTotal ).Sum( cg => cg.Volunteers ),
                Total = datasource.Where( ds => ds.IsTotal ).Sum( cg => cg.Total )
            } );

            return datasource;
        }

        public virtual List<DateRange> GetScheduleDateRanges( Schedule schedule, DateTime beginDateTime, DateTime endDateTime )
        {
            if ( schedule == null )
            {
                return new List<DateRange>();
            }

            var result = new List<DateRange>();

            DDay.iCal.Event calEvent = schedule.GetCalenderEvent();
            if ( calEvent != null && calEvent.DTStart != null )
            {
                var occurrences = ScheduleICalHelper.GetOccurrences( calEvent, beginDateTime, endDateTime );
                result = occurrences
                    .Where( a =>
                         a.Period != null &&
                         a.Period.StartTime != null &&
                         a.Period.EndTime != null )
                    .Select( a => new DateRange
                    {
                        Start = DateTime.SpecifyKind( a.Period.StartTime.Value, DateTimeKind.Local ).AddMinutes( -schedule.CheckInStartOffsetMinutes ?? 0 ),
                        End = DateTime.SpecifyKind( a.Period.EndTime.Value, DateTimeKind.Local )
                    } )
                    .ToList();
                // ensure the the datetime is DateTimeKind.Local since iCal returns DateTimeKind.UTC
            }
            return result;
        }

        public virtual DayOfWeek GetLastDayOfWeek( Schedule schedule, DateTime beginDateTime, DateTime endDateTime )
        {
            if ( schedule == null )
            {
                return DayOfWeek.Sunday;
            }

            DDay.iCal.Event calEvent = schedule.GetCalenderEvent();
            if ( calEvent != null && calEvent.DTStart != null )
            {
                var occurrences = ScheduleICalHelper.GetOccurrences( calEvent, beginDateTime, endDateTime );
                return occurrences
                    .FirstOrDefault( a =>
                         a.Period != null &&
                         a.Period.StartTime != null &&
                         a.Period.EndTime != null )
                    .Period.StartTime.DayOfWeek;
                // ensure the the datetime is DateTimeKind.Local since iCal returns DateTimeKind.UTC
            }
            return DayOfWeek.Sunday;
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

            public DayOfWeek DayOfWeek { get; set; }
            public TimeSpan StartTime { get; set; }


            public int Count { get; set; }

            public int Volunteers { get; set; }

            public int Total { get; set; }

            public String MetricNote { get; set; }

            public List<DateRange> ScheduleDateRanges { get; set; }

            public System.Linq.IGrouping<int, MetricValue> Value { get; set; }
        }

        #endregion
    }
}