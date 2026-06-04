using System.Net.Http.Json;
using NasreddinsSimplePeekClient2Web.Models;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class HelpContentService(HttpClient httpClient)
{
    private const string HelpBasePath = "help/de/";
    private IReadOnlyList<HelpTopic>? cachedHelpTopics;

    public async Task<IReadOnlyList<HelpTopic>> GetHelpTopicsAsync()
    {
        if (cachedHelpTopics is not null)
        {
            return cachedHelpTopics;
        }

        try
        {
            cachedHelpTopics = await httpClient.GetFromJsonAsync<IReadOnlyList<HelpTopic>>($"{HelpBasePath}help-index.json")
                ?? Array.Empty<HelpTopic>();
        }
        catch
        {
            cachedHelpTopics = Array.Empty<HelpTopic>();
        }

        return cachedHelpTopics;
    }

    public async Task<HelpTopic?> GetHelpTopicAsync(string helpIdentifier)
    {
        var helpTopics = await GetHelpTopicsAsync();
        return helpTopics.FirstOrDefault(helpTopic => string.Equals(helpTopic.Id, helpIdentifier, StringComparison.Ordinal));
    }

    public async Task<string> GetHelpMarkdownAsync(HelpTopic helpTopic)
    {
        if (string.IsNullOrWhiteSpace(helpTopic.File))
        {
            return GermanText.HelpTextMissing;
        }

        try
        {
            return await httpClient.GetStringAsync($"{HelpBasePath}{helpTopic.File}");
        }
        catch
        {
            return GermanText.HelpTextMissing;
        }
    }
}
