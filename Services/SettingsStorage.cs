using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using NasreddinsSimplePeekClient2Web.Models;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class SettingsStorage(IJSRuntime javaScriptRuntime)
{
    private const string SettingsStorageKey = "nasreddins-simple-peek-client2-web:settings:v1";
    private const string ProfileNameStorageKey = "nasreddins-simple-peek-client2-web:settings-profile-name:v1";
    public const string ProfileFileSuffix = ".peekclient.json";
    private static readonly JsonSerializerOptions JsonSerializerOptions = CreateJsonSerializerOptions();
    private static readonly JsonSerializerOptions IndentedJsonSerializerOptions = new(JsonSerializerOptions)
    {
        WriteIndented = true
    };

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

    public async Task<string> LoadCurrentProfileNameAsync()
    {
        return await javaScriptRuntime.InvokeAsync<string?>("localStorage.getItem", ProfileNameStorageKey) ?? "";
    }

    public async Task SaveCurrentProfileNameAsync(string profileName)
    {
        await javaScriptRuntime.InvokeVoidAsync("localStorage.setItem", ProfileNameStorageKey, profileName.Trim());
    }

    public static PersistedApplicationSettings? DeserializeProfile(string profileText) =>
        JsonSerializer.Deserialize<PersistedApplicationSettings>(profileText, JsonSerializerOptions);

    public static string SerializeProfile(PersistedApplicationSettings settings) =>
        JsonSerializer.Serialize(settings, IndentedJsonSerializerOptions);

    public static bool IsAllowedProfileFileName(string fileName) =>
        fileName.EndsWith(ProfileFileSuffix, StringComparison.OrdinalIgnoreCase);

    public static string NormalizeProfileFileName(string profileName)
    {
        var normalizedName = profileName.Trim();

        foreach (var invalidCharacter in new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' })
        {
            normalizedName = normalizedName.Replace(invalidCharacter, '-');
        }

        while (normalizedName.EndsWith(ProfileFileSuffix, StringComparison.OrdinalIgnoreCase))
        {
            normalizedName = normalizedName[..^ProfileFileSuffix.Length].Trim();
        }

        return $"{normalizedName}{ProfileFileSuffix}";
    }

    public static string GetProfileNameFromFileName(string fileName)
    {
        var profileName = Path.GetFileName(fileName.Trim());

        if (profileName.EndsWith(ProfileFileSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return profileName[..^ProfileFileSuffix.Length];
        }
        return profileName;
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter<CameraViewStyle>());
        options.Converters.Add(new JsonStringEnumConverter<CameraButtonAppearance>());
        options.Converters.Add(new JsonStringEnumConverter<VideoSourceType>());
        return options;
    }
}
