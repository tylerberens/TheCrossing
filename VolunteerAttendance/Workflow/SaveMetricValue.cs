using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;

namespace com.bricksandmortarstudio.TheCrossing.VolunteerAttendance.Workflow
{
    [ActionCategory( "Metrics" )]
    [Description( "Save a Metric Value to the MetricValues table." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save MetricValue" )]

    [WorkflowTextOrAttribute( "Metric Guid", "Metric Attribute", "<span class='tip tip-lava'></span> The metric entity attribute or Guid to use.", true, "", "", 1, "MetricId" )]
    [WorkflowTextOrAttribute( "MetricValue Value", "Metric Value Attribute", "<span class='tip tip-lava'></span> The value to save.", true, "", "", 2, "MetricValue" )]
    [WorkflowAttribute( "MetricValue DateTime Attribute", "DateTime to save with the Metric Value (attribute can be a DateTime or Date Entity).", true, "", "", 3, "MetricDate" )]
    [WorkflowTextOrAttribute( "MetricValue Entity Id", "MetricValue Entity Id Attribute", "<span class='tip tip-lava'></span> Entity (eg. campus) Id to save with the metric value.", false, "", "", 4, "EntityId" )]
    [WorkflowTextOrAttribute( "MetricValue Notes", "Notes Attribute", "Note to save with the metric", false, "", "", 5, "Notes" )]
    [WorkflowTextOrAttribute( "MetricValue Type", "MetricValue Type Attribute", "Int of the Type of MetricValue to Save", true, "0", "", 6, "MetricType" )]

    class SaveMetricValue : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //Get the Attribute Values
            string metricId = GetAttributeValue( action, "MetricId" );
            string metricValue = GetAttributeValue( action, "MetricValue" );
            string metricDate = GetAttributeValue( action, "MetricDate" );
            string metricEntityId = GetAttributeValue( action, "EntityId" );
            string metricNotes = GetAttributeValue( action, "Notes" );
            string metricType = GetAttributeValue( action, "MetricType" );

            //Get Metric Id
            string metricIdContent = metricId;
            var metricIdGuid = metricId.AsGuid();
            if ( metricIdGuid.IsEmpty() )
            {
                metricIdContent = metricIdContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricIdGuid );

                if ( attributeValue != null )
                {
                    metricIdContent = attributeValue;
                }
            }


            //Get Metric Value
            string metricValueContent = metricValue;
            Guid metricValueGuid = metricValue.AsGuid();
            if ( metricValueGuid.IsEmpty() )
            {
                metricValueContent = metricValueContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricValueGuid );

                if ( attributeValue != null )
                {
                    metricValueContent = attributeValue;
                }
            }

            //Get Metric Date
            string metricDateContent = metricDate;
            var metricDateGuid = metricDate.AsGuid();
            if ( metricDateGuid.IsEmpty() )
            {
                metricDateContent = metricDateContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricDateGuid );

                if ( attributeValue != null )
                {
                    metricDateContent = attributeValue;
                }
            }

            //Get Metric Notes
            string metricNotesContent = metricNotes;
            var metricNotesGuid = metricNotes.AsGuid();
            if ( metricNotesGuid.IsEmpty() )
            {
                metricNotesContent = metricNotesContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricNotesGuid );

                if ( attributeValue != null )
                {
                    metricNotesContent = attributeValue;
                }
            }

            //Get Metric EntityId
            string metricEntityIdContent = metricEntityId;
            var metricEntityIdGuid = metricEntityId.AsGuid();
            if ( metricEntityIdGuid.IsEmpty() )
            {
                metricEntityIdContent = metricEntityIdContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricEntityIdGuid );

                if ( attributeValue != null )
                {
                    metricEntityIdContent = attributeValue;
                }
            }


            //Get Metric Type
            string metricTypeContent = metricType;
            var metricTypeGuid = metricType.AsGuid();
            if ( metricTypeGuid.IsEmpty() )
            {
                metricTypeContent = metricTypeContent.ResolveMergeFields( GetMergeFields( action ) );
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue( metricTypeGuid );

                if ( attributeValue != null )
                {
                    metricTypeContent = attributeValue;
                }
            }

            //Convert the Attribute values to approprate types
            var metricDateAsDate = Convert.ToDateTime( metricDateContent );
            var metricValueAsDecimal = Decimal.Parse( metricValueContent );
            int metricEntityIdAsInt = metricEntityIdContent.AsInteger();
            int metricTypeAsInt = metricTypeContent.AsInteger();

            //Convert Metric Attribute to Id
            var metricIdAsGuidAsValues = metricIdContent.Split( '|' );
            var metricIdAsGuid = Guid.Parse( metricIdAsGuidAsValues[0] );
            var metricService = new MetricService( rockContext );
            var selectedMetric = metricService.Queryable().FirstOrDefault( m => m.Guid == metricIdAsGuid );
            if ( selectedMetric != null )
            {
                int metricIdAsInt = selectedMetric.Id;

                //Save the Metric
                SaveMetric( metricDateAsDate, metricIdAsInt, metricValueAsDecimal, metricEntityIdAsInt, metricNotesContent, metricTypeAsInt );
            }

            return true;
        }

        public void SaveMetric( DateTime dt, int metric, Decimal value, int entityId, string notes, int type )
        {
            int metricValueId = 0;
            var rockContext = new RockContext();
            MetricValue metricValue;
            var metricValueService = new MetricValueService( rockContext );

            //Does this metric already exist?
            var existingMetric = metricValueService
                .Queryable(
                    ).FirstOrDefault( a => a.MetricId == metric && a.MetricValueDateTime == dt && a.EntityId == entityId );

            if ( existingMetric != null )
            {
                metricValueId = existingMetric.Id;
            }


            if ( metricValueId == 0 )
            {
                metricValue = new MetricValue();
                metricValueService.Add( metricValue );
                metricValue.MetricId = metric;
                metricValue.Metric = metricValue.Metric ?? new MetricService( rockContext ).Get( metricValue.MetricId );
            }
            else
            {
                metricValue = metricValueService.Get( metricValueId );
            }

            metricValue.MetricValueType = ( MetricValueType ) type;
            metricValue.XValue = null;
            metricValue.YValue = value;
            metricValue.Note = notes;
            metricValue.MetricValueDateTime = dt;
            metricValue.EntityId = entityId;

            rockContext.SaveChanges();

        }
    }
}