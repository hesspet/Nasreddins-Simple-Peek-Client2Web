using System.Text;
using System.Text.RegularExpressions;
using NasreddinsSimplePeekClient2Web.Models;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class AudioSpyMatcher
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(100);
    private const RegexOptions MatchOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    public AudioSpyMatch? FindFirstMatch(string word, IEnumerable<AudioSpyMapping> mappings)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        var normalizedWord = word.Trim();

        foreach (var mapping in mappings)
        {
            if (mapping.Commands.Count == 0)
            {
                continue;
            }

            foreach (var keyword in mapping.Keywords)
            {
                if (IsMatch(normalizedWord, keyword))
                {
                    return new AudioSpyMatch(mapping, keyword);
                }
            }
        }

        return null;
    }

    public bool TryValidateKeyword(string keyword, out string errorText)
    {
        errorText = "";

        if (string.IsNullOrWhiteSpace(keyword))
        {
            errorText = GermanText.AudioSpyEmptyKeyword;
            return false;
        }

        try
        {
            _ = Regex.IsMatch("test", GetPattern(keyword.Trim()), MatchOptions, RegexTimeout);
            return true;
        }
        catch (ArgumentException)
        {
            errorText = GermanText.AudioSpyInvalidRegex;
            return false;
        }
        catch (RegexMatchTimeoutException)
        {
            errorText = GermanText.AudioSpyRegexTimeout;
            return false;
        }
    }

    private static bool IsMatch(string word, string keyword)
    {
        try
        {
            return Regex.IsMatch(word, GetPattern(keyword), MatchOptions, RegexTimeout);
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static string GetPattern(string keyword)
    {
        var trimmedKeyword = keyword.Trim();

        if (trimmedKeyword.Length >= 2 && trimmedKeyword[0] == '/' && trimmedKeyword[^1] == '/')
        {
            return trimmedKeyword[1..^1];
        }

        var builder = new StringBuilder("^");

        foreach (var character in trimmedKeyword)
        {
            builder.Append(character switch
            {
                '*' => ".*",
                '?' => ".",
                _ => Regex.Escape(character.ToString())
            });
        }

        builder.Append('$');
        return builder.ToString();
    }
}
