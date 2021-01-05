using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;
using Rock.Web.Cache;

namespace com.bricksandmortarstudio.TheCrossing.Jobs
{
    /// <summary>
    /// Job to run quick SQL queries on a schedule
    /// </summary>
    [CodeEditorField( "SQL Query", "SQL query to run", CodeEditorMode.Sql, CodeEditorTheme.Rock, 200, true, "", "General", 0, "SQLQuery" )]
    [WorkflowTypeField("Workflow", "The workflow to launch for each row of the SQL results", false, true)]
    [KeyValueListField( "SQL Column to Workflow Attribute Mapping", "Used to match the SQL column values to the keys of the launched workflow. The new workflow will inherit the attribute values of the keys provided.", true, keyPrompt: "SQL Column Name", valuePrompt: "Target Attribute", key:"sqlToWorkflow" )]
    [DisallowConcurrentExecution]
    public class RunSqlLaunchWorkflow : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            // Get job settings
            var dataMap = context.JobDetail.JobDataMap;
            string query = dataMap.GetString( "SQLQuery" );
            var workflowTypeGuid = dataMap.GetString("Workflow").AsGuidOrNull();
            var sqlToWorkflowAttributeMapping = dataMap.GetString("sqlToWorkflow").AsDictionaryOrNull();

            // Ensure job settings aren't null
            if (string.IsNullOrEmpty(query) || workflowTypeGuid == null || sqlToWorkflowAttributeMapping == null)
            {
                throw new Exception("Missing one or more job settings.");
            }

            var rockContext = new RockContext();
            var workflowType = WorkflowTypeCache.Get( workflowTypeGuid.Value );
            if (workflowType == null)
            {
                throw new Exception("Unable to find matching Workflow Type");
            }

            if (workflowType.IsActive == false)
            {
                throw new Exception( String.Format( "{0} is not an active workflow type", workflowType.Name ) );
            }

            // Run SQL and launch workflows
            try
            {
                var table = DbService.GetDataTable( query, CommandType.Text, null );
                if (table == null)
                {
                    throw new Exception("No data returned from SQL query");
                }

                var workflowService = new WorkflowService(rockContext);

                foreach (DataRow row in table.Rows)
                {
                    var workflow = Workflow.Activate( workflowType, workflowType.Name );
                    workflow.LoadAttributes( rockContext );

                    foreach (var keyValuePair in sqlToWorkflowAttributeMapping)
                    {
                        string workflowKey = keyValuePair.Value.Replace("|", "");
                        if (row[keyValuePair.Key] != null && workflow.Attributes.ContainsKey(workflowKey))
                        {
                            string value = row[keyValuePair.Key].ToString();
                            workflow.SetAttributeValue(workflowKey, value);
                        }
                        else
                        {
                            throw new Exception("Bad match on SQL column to Workflow Attribute");
                        }
                    }
                    List<string> errorMessages;
                    workflowService.Process(workflow, out errorMessages);
                }

            }
            catch ( Exception ex )
            {
                var httpContext = HttpContext.Current;
                ExceptionLogService.LogException( ex, httpContext );
                throw;
            }
        }
    }
}
