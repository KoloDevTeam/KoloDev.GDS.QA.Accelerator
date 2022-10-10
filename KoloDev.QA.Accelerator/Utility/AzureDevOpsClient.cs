// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 02-07-2022
//
// Last Modified By : KoloDev
// Last Modified On : 06-27-2022
// ***********************************************************************
// <copyright file="AzureDevOpsClient.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace KoloDev.GDS.QA.Accelerator.ADO
{
    /// <summary>
    /// Class AzureDevopsClient.
    /// </summary>
    public static class AzureDevopsClient
    {
        /// <summary>
        /// Gets the name of the wiki by.
        /// </summary>
        /// <param name="org">The org.</param>
        /// <param name="project">The project.</param>
        /// <param name="pat">The pat.</param>
        /// <returns>WikiHttpClient.</returns>
        public static WikiHttpClient GetWikiByName(string org, string project, string pat)
        {
            var uri = new Uri(org);
            var personalAccessToken = pat;
            VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, personalAccessToken));
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();
            return wikiClient;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="org">The org.</param>
        /// <param name="project">The project.</param>
        /// <param name="pat">The pat.</param>
        /// <returns>WorkItemTrackingHttpClient.</returns>
        public static WorkItemTrackingHttpClient CreateConnection(string org, string project, string pat)
        {
            var uri = new Uri(org);
            var personalAccessToken = pat;
            VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, personalAccessToken));
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            return workItemTrackingHttpClient;
        }

        /// <summary>
        /// Gets all work items ready to release.
        /// </summary>
        /// <param name="workItemTrackingHttpClient">The work item tracking HTTP client.</param>
        /// <param name="project">The project.</param>
        /// <returns>WorkItemQueryResult.</returns>
        public static WorkItemQueryResult GetAllWorkItemsReadyToRelease(WorkItemTrackingHttpClient workItemTrackingHttpClient, string project)
        {
            // wiql - Work Item Query Language
            var wiql = new Wiql
            {
                Query = $@"Select [System.Id], [System.WorkItemType], [System.Title], [System.State] From WorkItems Where [System.BoardColumn] = 'Ready For Release' and [System.TeamProject] = '{project}'"
            };

            WorkItemQueryResult workItemIds = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;
            return workItemIds;
        }

        /// <summary>
        /// Gets the work item detail.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>WorkItem.</returns>
        public static WorkItem GetWorkItemDetail(VssConnection connection)
        {
            WorkItem item = new WorkItem();
            return item;
        }
    }
}
