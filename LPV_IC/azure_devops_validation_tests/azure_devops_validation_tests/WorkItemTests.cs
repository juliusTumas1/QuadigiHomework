using Xunit;

namespace AzureDevOpsValidationTests;

public class WorkItemTests : IDisposable
{
    private readonly TestHandler _testHandler;

    public WorkItemTests()
    {
        _testHandler = new TestHandler();
    }

    public void Dispose()
    {
        _testHandler.CleanUpWorkItems();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [ClassData(typeof(WorkItemTypeTestData))]
    public async Task CanCreateWorkItem(string type, Dictionary<string, object> expectedFields)
    {
        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, type);
        Assert.NotNull(createdWorkItem);
        Assert.True(TestHelper.DictionaryContainsAllKeyValuePairs(createdWorkItem.Fields, expectedFields));
    }

    [Theory]
    [ClassData(typeof(WorkItemTypeTestData))]
    public async Task CanUpdateWorkItemFields(string type, Dictionary<string, object> expectedFields)
    {
        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, type);
        Assert.NotNull(createdWorkItem.Id);

        var fieldsForUpdate = new Dictionary<string, object>
        {
            { "System.Title", "Updated " + type },
            { "System.Description", "Updated " + type + " Description" },
            { "Custom.ReviewedbyLeaderVerification", _testHandler.UserIdentity }
        };

        var updateDocument = TestHelper.CreatePatchDocumentForFields(fieldsForUpdate);
        var updatedWorkItem = await _testHandler.UpdateWorkItemOnAzureDevOps(updateDocument, (int)createdWorkItem.Id);
        Assert.NotNull(updatedWorkItem);
        Assert.NotNull(updatedWorkItem.Id);
        Assert.True(TestHelper.DictionaryContainsAllKeyValuePairs(updatedWorkItem.Fields, fieldsForUpdate));
    }

    [Theory]
    [ClassData(typeof(WorkItemTypeTestData))]
    public async Task UpdatingWorkItemId_WillNotHaveAnyEffect(string type, Dictionary<string, object> expectedFields)
    {
        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, type);
        Assert.NotNull(createdWorkItem.Id);

        var fieldsForUpdate = new Dictionary<string, object>
        {
            { "System.Id", "123" }
        };

        var updateDocument = TestHelper.CreatePatchDocumentForFields(fieldsForUpdate);
        var updatedWorkItem = await _testHandler.UpdateWorkItemOnAzureDevOps(updateDocument, (int)createdWorkItem.Id);

        Assert.Equal(createdWorkItem.Id, updatedWorkItem.Id);
        Assert.False(TestHelper.DictionaryContainsAllKeyValuePairs(updatedWorkItem.Fields, fieldsForUpdate));
        Assert.True(TestHelper.DictionaryContainsAllKeyValuePairs(updatedWorkItem.Fields, expectedFields));
    }
}