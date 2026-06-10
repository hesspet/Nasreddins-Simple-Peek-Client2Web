namespace NasreddinsSimplePeekClient2Web.Resources;

/// <summary>
/// Zentrale Registratur für SVG-Icons, die plattformunabhängig auf Buttons und
/// Steuerelementen gerendert werden. Löst das Problem, dass Unicode-Pfeile und
/// -Symbole auf iOS/macOS nicht in allen Systemfonts korrekt dargestellt werden.
/// </summary>
/// <remarks>
/// <para><b>Hintergrund:</b> Unicode-Zeichen aus den Blöcken U+2190–U+21FF (Pfeile),
/// U+2600–U+26FF (Symbole) und U+2680–U+2685 (Würfel) haben im iOS-Systemfont
/// (San Francisco) keine oder nur eingeschränkte Glyphen. Sie erscheinen als
/// generische Textzeichen statt als grafische Symbole.</para>
///
/// <para><b>Lösung:</b> Diese Klasse stellt SVG-Markup-Strings bereit, die über
/// <c>MarkupString</c> in Blazor-Komponenten eingebettet werden. SVG wird von
/// allen modernen Browsern identisch gerendert – unabhängig vom Betriebssystem
/// und den installierten Fonts.</para>
///
/// <para><b>Verwendung in Razor:</b></para>
/// <code>
///   // Direkt:
///   @((MarkupString)IconSvg.ArrowNorth)
///
///   // Mit Fallback auf Unicode-String:
///   @((MarkupString)(IconSvg.GetSvg("arrow-n") ?? "↑"))
/// </code>
///
/// <para><b>Erweiterung:</b> Neue Icons als <c>public const string</c>-Felder
/// ergänzen und optional in <see cref="GetSvg"/> registrieren. Jedes Icon muss
/// ein vollständiges <c>&lt;svg&gt;</c>-Element mit <c>viewBox="0 0 24 24"</c>
/// enthalten. Die Attribute <c>stroke="currentColor"</c> und <c>fill="none"</c>
/// sorgen dafür, dass die Farbe vom umgebenden Text (inkl. Dark Mode) geerbt wird.</para>
///
/// <para><b>Größensteuerung:</b> Das <c>&lt;svg&gt;</c>-Element enthält bewusst
/// keine <c>width</c>/<c>height</c>-Attribute. Die Größe wird per CSS über die
/// Klasse <c>.command-icon</c> gesteuert, sodass sie zentral angepasst werden kann.</para>
/// </remarks>
public static class IconSvg
{
    // ──────────────────────────────────────────────
    //  Pfeile – Himmelsrichtungen (8 Stück)
    // ──────────────────────────────────────────────

    /// <summary>Pfeil nach Norden (↑).</summary>
    public const string ArrowNorth =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"12\" y1=\"19\" x2=\"12\" y2=\"5\" />" +
        "<polyline points=\"9,9 12,5 15,9\" />" +
        "</svg>";

    /// <summary>Pfeil nach Nordosten (↗).</summary>
    public const string ArrowNorthEast =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"7\" y1=\"17\" x2=\"17\" y2=\"7\" />" +
        "<polyline points=\"12,8 17,7 16,12\" />" +
        "</svg>";

    /// <summary>Pfeil nach Osten (→).</summary>
    public const string ArrowEast =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"5\" y1=\"12\" x2=\"19\" y2=\"12\" />" +
        "<polyline points=\"15,9 19,12 15,15\" />" +
        "</svg>";

    /// <summary>Pfeil nach Südosten (↘).</summary>
    public const string ArrowSouthEast =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"7\" y1=\"7\" x2=\"17\" y2=\"17\" />" +
        "<polyline points=\"16,12 17,17 12,16\" />" +
        "</svg>";

    /// <summary>Pfeil nach Süden (↓).</summary>
    public const string ArrowSouth =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"12\" y1=\"5\" x2=\"12\" y2=\"19\" />" +
        "<polyline points=\"9,15 12,19 15,15\" />" +
        "</svg>";

    /// <summary>Pfeil nach Südwesten (↙).</summary>
    public const string ArrowSouthWest =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"17\" y1=\"7\" x2=\"7\" y2=\"17\" />" +
        "<polyline points=\"8,12 7,17 12,16\" />" +
        "</svg>";

    /// <summary>Pfeil nach Westen (←).</summary>
    public const string ArrowWest =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"19\" y1=\"12\" x2=\"5\" y2=\"12\" />" +
        "<polyline points=\"9,9 5,12 9,15\" />" +
        "</svg>";

    /// <summary>Pfeil nach Nordwesten (↖).</summary>
    public const string ArrowNorthWest =
        "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
        "<line x1=\"19\" y1=\"19\" x2=\"7\" y2=\"7\" />" +
        "<polyline points=\"12,8 7,7 8,12\" />" +
        "</svg>";

    // ──────────────────────────────────────────────
    //  Schlüssel-basierte Suche
    // ──────────────────────────────────────────────

    private static readonly Dictionary<string, string> IconRegistry = new()
    {
        ["arrow-n"] = ArrowNorth,
        ["arrow-ne"] = ArrowNorthEast,
        ["arrow-e"] = ArrowEast,
        ["arrow-se"] = ArrowSouthEast,
        ["arrow-s"] = ArrowSouth,
        ["arrow-sw"] = ArrowSouthWest,
        ["arrow-w"] = ArrowWest,
        ["arrow-nw"] = ArrowNorthWest,
    };

    /// <summary>
    /// Gibt das SVG-Markup für einen registrierten Icon-Schlüssel zurück,
    /// oder <c>null</c> wenn der Schlüssel nicht existiert.
    /// </summary>
    /// <param name="key">Icon-Schlüssel, z.B. <c>"arrow-n"</c>.</param>
    /// <returns>SVG-Markup-String oder <c>null</c>.</returns>
    public static string? GetSvg(string key) =>
        IconRegistry.TryGetValue(key, out var svg) ? svg : null;
}
