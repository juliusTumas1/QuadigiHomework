using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzureDevOpsValidationTests;

public class TestHandler
{
    private readonly List<int> _createdWorkItemIds;
    private readonly string _projectName;
    private readonly TeamContext _teamContext;
    private readonly VssConnection _vssConnection;
    private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

    public readonly IdentityRef UserIdentity;


    public TestHandler()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Constants.AppSettingsFileName, true, true)
            .Build();

        var appSettings = new AppSettings();
        config.GetSection(Constants.AppSettingsSectionName).Bind(appSettings);


        _projectName = appSettings.ProjectName;
        _createdWorkItemIds = new List<int>();
        _teamContext = new TeamContext(_projectName, appSettings.ProjectTeamName);
        _vssConnection = CreateVssConnection(appSettings.BaseUri, appSettings.OrganizationName,
            appSettings.UserPersonalAccessToken);
        _workItemTrackingHttpClient = CreateWorkItemTrackingClient();

        UserIdentity = new IdentityRef { DisplayName = appSettings.UserDisplayName, UniqueName = appSettings.UserId };
    }

    private static VssConnection CreateVssConnection(string baseUri, string organizationName, string pat)
    {
        return new VssConnection(new Uri($"{baseUri}{organizationName}"),
            new VssBasicCredential(string.Empty, pat));
    }

    private WorkItemTrackingHttpClient CreateWorkItemTrackingClient()
    {
        return _vssConnection.GetClient<WorkItemTrackingHttpClient>();
    }

    public async Task<WorkItem> CreateWorkItemOnAzureDevOps(JsonPatchDocument patchDocument, string type)
    {
        var createdWorkItem = await _workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, _projectName, type);
        if (createdWorkItem.Id is not null) _createdWorkItemIds.Add((int)createdWorkItem.Id);

        return createdWorkItem;
    }

    public async Task<WorkItem> UpdateWorkItemOnAzureDevOps(JsonPatchDocument patchDocument, int itemId)
    {
        return await _workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, _projectName, itemId);
    }

    public async Task DeleteWorkItemOnAzureDevOps(int itemId)
    {
        await _workItemTrackingHttpClient.DeleteWorkItemAsync(_projectName, itemId);
    }

    public async void CleanUpWorkItems()
    {
        foreach (var workItemId in _createdWorkItemIds) await DeleteWorkItemOnAzureDevOps(workItemId);
    }
}