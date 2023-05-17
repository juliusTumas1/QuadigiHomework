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


    [Fact]
    public async Task CanCreateTsr()
    {
        var expectedFields = CreateRequiredTsrFieldDictionary();

        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, "TSR");
        Assert.NotNull(createdWorkItem);
        Assert.True(TestHelper.DictionaryContainsAllKeyValuePairs(createdWorkItem.Fields, expectedFields));
    }

    [Fact]
    public async Task CanUpdateTsrFields()
    {
        var expectedFields = CreateRequiredTsrFieldDictionary();
        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, "TSR");
        Assert.NotNull(createdWorkItem.Id);

        var fieldsForUpdate = new Dictionary<string, object>
        {
            { "System.Title", "Updated TSR" },
            { "System.Description", "Updated Description" },
            { "Custom.Type", "RCM" }
        };

        var updateDocument = TestHelper.CreatePatchDocumentForFields(fieldsForUpdate);
        var updatedWorkItem = await _testHandler.UpdateWorkItemOnAzureDevOps(updateDocument, (int)createdWorkItem.Id);
        Assert.NotNull(updatedWorkItem);
        Assert.NotNull(updatedWorkItem.Id);
        Assert.True(TestHelper.DictionaryContainsAllKeyValuePairs(updatedWorkItem.Fields, fieldsForUpdate));
    }

    [Fact]
    public async Task UpdatingTsrWorkItemId_WillNotHaveAnyEffect()
    {
        var expectedFields = CreateRequiredTsrFieldDictionary();
        var document = TestHelper.CreatePatchDocumentForFields(expectedFields);

        var createdWorkItem = await _testHandler.CreateWorkItemOnAzureDevOps(document, "TSR");
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

    private Dictionary<string, object> CreateRequiredTsrFieldDictionary()
    {
        return new Dictionary<string, object>
        {
            { "System.Title", "Test TSR" },
            { "System.Description", "This is a new TSR work item created using the Azure DevOps .NET SDK." },
            { "System.WorkItemType", "TSR" },
            { "Custom.Type", "Requirement" },
            { "Custom.Verificationmethod", "MethodA" },
            { "Custom.VerificationLevel", "LevelA" },
            { "Custom.ReviewedbyProductManagement", _testHandler.UserIdentity },
            { "Custom.ReviewedbyProjectManagement", _testHandler.UserIdentity },
            { "Custom.ReviewedbySystemEngineering", _testHandler.UserIdentity },
            { "Custom.ReviewedbyLeaderVerification", _testHandler.UserIdentity }
        };
    }

}