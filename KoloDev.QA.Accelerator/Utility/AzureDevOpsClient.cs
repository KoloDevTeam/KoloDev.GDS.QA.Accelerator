using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace KoloDev.GDS.QA.Accelerator.ADO
{
    public static class AzureDevopsClient
    {
        public static WikiHttpClient GetWikiByName(string org, string project, string pat)
        {
            var uri = new Uri(org);
            var personalAccessToken = pat;
            VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, personalAccessToken));
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();
            return wikiClient;
        }

        public static WorkItemTrackingHttpClient CreateConnection(string org, string project, string pat)
        {
            var uri = new Uri(org);
            var personalAccessToken = pat;
            VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, personalAccessToken));
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            return workItemTrackingHttpClient;
        }

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

        public static WorkItem GetWorkItemDetail(VssConnection connection)
        {
            WorkItem item = new WorkItem();
            return item;
        }
    }
}
