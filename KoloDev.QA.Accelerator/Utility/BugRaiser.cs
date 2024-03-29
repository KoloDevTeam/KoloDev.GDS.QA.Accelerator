﻿namespace KoloDev.GDS.QA.Accelerator.Utility
{
    using KoloDev.GDS.QA.Accelerator.Data;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.VisualStudio.Services.WebApi.Patch;
    using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
    using NUnit.Framework;
    using System;
    using static KoloDev.GDS.QA.Accelerator.Data.KoloTestSuite;

    /// <summary>
    /// Automatic Bug Raising for Azure DevOps
    /// </summary>
    public class BugRaiser
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        /// <summary>
        /// Constructor. Manually set values to match your organization. 
        /// </summary>
        public BugRaiser()
        {
            _uri = "";
            _personalAccessToken = "";
            _project = "";
        }

        /// <summary>
        /// Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>    
        public WorkItem CreateBugUsingClientLib(KoloTestCase testCase, BrowserStackBrowsers browser, int priority, int severity, string screenshot, string AssignTo = "")
        {
            Uri uri = new Uri(_uri);
            string personalAccessToken = _personalAccessToken;
            string project = _project;

            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            VssConnection connection = new VssConnection(uri, credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            try
            {
                var att = workItemTrackingHttpClient.CreateAttachmentAsync(screenshot).Result;
                var result = "";
                foreach (KoloTestSteps step in testCase.TestSteps)
                {
                    result += String.Format("<div>Test Step: " + step.StepNumber + " - " + step.Name + " " + step.Description + " - Step Passed: " + step.Passed.ToString() + "<div>");
                }
                result += "<br/>Latest Screenshot in Test. Video of Test is available in Test Output <br/>";
                result += "<img src='" + att.Url + "' width='50%' height='50%'>";

                //add fields and their values to your patch document
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Title",
                        Value = "" + testCase.Id + ""
                    }
                    );

                patchDocument.Add(new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "AttachedFile",
                        url = att.Url,
                        attributes = new { comment = "" }
                    }
                });

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                        Value = "" + result + ""
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.Common.Priority",
                        Value = "" + priority.ToString() + ""
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.TCM.SystemInfo",
                        Value = "" + browser + ""
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.AssignedTo",
                        Value = "" + AssignTo + ""
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/Microsoft.VSTS.Common.Severity",
                        Value = "2 - High"
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                WorkItem defect = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, project, "Bug").Result;
                return defect;
            }
            catch (AggregateException ex)
            {
                return null;
            }
        }
    }
}
