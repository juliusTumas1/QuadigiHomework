using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzureDevOpsValidationTests;

public static class TestHelper
{
    public static JsonPatchDocument CreatePatchDocumentForFields(IDictionary<string, object> fields)
    {
        var document = new JsonPatchDocument();
        document.AddRange(fields.Select(field =>
            new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = $"{Constants.PatchOperationFieldPathPrefix}{field.Key}",
                Value = field.Value
            }));
        return document;
    }

    public static bool DictionaryContainsAllKeyValuePairs(IDictionary<string, object> dictionary1,
        IDictionary<string, object> dictionary2)
    {
        foreach (var kvp in dictionary2)
        {
            if (!dictionary1.TryGetValue(kvp.Key, out var value))
                return false;

            if (kvp.Value is IdentityRef identityRef1 && value is IdentityRef identityRef2)
            {
                if (identityRef1.DisplayName != identityRef2.DisplayName ||
                    identityRef1.UniqueName != identityRef2.UniqueName)
                    return false;
            }
            else if (!EqualityComparer<object>.Default.Equals(kvp.Value, value))
            {
                return false;
            }
        }

        return true;
    }
}