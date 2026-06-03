using System.Text.Json;
using Microsoft.JSInterop;
using NasreddinsSimplePeekClient2Web.Models;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class SettingsStorage(IJSRuntime javaScriptRuntime)
{
    private const string SettingsStorageKey = "nasreddins-simple-peek-client2-web:settings:v1";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<PersistedApplicationSettings?> LoadSettingsAsync()
    {
        var settingsText = await javaScriptRuntime.InvokeAsync<string?>("localStorage.getItem", SettingsStorageKey);

        if (string.IsNullOrWhiteSpace(settingsText))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<PersistedApplicationSettings>(settingsText, JsonSerializerOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveSettingsAsync(PersistedApplicationSettings settings)
    {
        var settingsText = JsonSerializer.Serialize(settings, JsonSerializerOptions);
        await javaScriptRuntime.InvokeVoidAsync("localStorage.setItem", SettingsStorageKey, settingsText);
    }
}
