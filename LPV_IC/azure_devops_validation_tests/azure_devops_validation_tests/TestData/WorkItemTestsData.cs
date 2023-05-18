using System.Collections;
using AzureDevOpsValidationTests;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Services.WebApi;

public class WorkItemTypeTestData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new();

    public WorkItemTypeTestData()
    {
        var userIdentity = GetUserIdentityFromConfig();

        _data.Add(new object[]
        {
            "TSR", new Dictionary<string, object>
            {
                { "System.Title", "Test TSR" },
                { "System.Description", "This is a new TSR work item created using the Azure DevOps .NET SDK." },
                { "System.WorkItemType", "TSR" },
                { "Custom.Type", "Requirement" },
                { "Custom.Verificationmethod", "MethodA" },
                { "Custom.VerificationLevel", "LevelA" },
                { "Custom.ReviewedbyProductManagement", userIdentity },
                { "Custom.ReviewedbyProjectManagement", userIdentity },
                { "Custom.ReviewedbySystemEngineering", userIdentity },
                { "Custom.ReviewedbyLeaderVerification", userIdentity }
            }
        });

        _data.Add(new object[]
        {
            "Product Requirement", new Dictionary<string, object>
            {
                { "System.Title", "Test PR" },
                { "System.Description", "This is a new PR work item created using the Azure DevOps .NET SDK." },
                { "System.WorkItemType", "Product Requirement" },
                { "Custom.Requirementclassification", "Claim" },
                { "Custom.Evaluationmethod", "Usability" },
                { "Custom.ReviewedbyProductManagement", userIdentity },
                { "Custom.ReviewedbyProjectManagement", userIdentity },
                { "Custom.ReviewedbySystemEngineering", userIdentity },
                { "Custom.ReviewedbyLeaderVerification", userIdentity }
            }
        });
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static IdentityRef GetUserIdentityFromConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Constants.AppSettingsFileName, true, true)
            .Build();

        var appSettings = new AppSettings();
        config.GetSection(Constants.AppSettingsSectionName).Bind(appSettings);

        return new IdentityRef { DisplayName = appSettings.UserDisplayName, UniqueName = appSettings.UserId };
    }
}