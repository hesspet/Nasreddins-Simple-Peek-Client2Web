using NasreddinsSimplePeekClient2Web.Models;

namespace NasreddinsSimplePeekClient2Web.Resources;

public static class GermanText
{
    public const string ApplicationTitle = "Nasreddins Simple Peek Client 2";
    public const string ApplicationLoading = "Anwendung wird geladen";
    public const string ApplicationError = "Ein unerwarteter Fehler ist aufgetreten.";
    public const string Reload = "Neu laden";
    public const string Close = "Schließen";
    public const string Menu = "Menü";
    public const string OpenMenu = "Menü öffnen";
    public const string CloseMenu = "Menü schließen";
    public const string ApplicationKind = "PWA";
    public const string Build = "Build";
    public const string Client = "Peeker (Videofake)";
    public const string Settings = "Einstellungen";
    public const string Log = "Log";
    public const string About = "About";
    public const string Help = "Hilfe";
    public const string OpenHelp = "Hilfe öffnen";
    public const string CloseHelp = "Hilfe schließen";
    public const string ElementHelp = "Hilfe zu diesem Element";
    public const string MoreHelpTopics = "Weitere Hilfethemen";
    public const string HelpOverview = "Hilfeübersicht";
    public const string HelpTextMissing = "Der Hilfetext konnte nicht geladen werden.";
    public const string SelectHelpTopic = "Wähle ein Hilfethema aus der Liste.";
    public const string HelpSystemHidden = "Hilfesystem ausblenden";
    public const string Connect = "Verbinden";
    public const string ConnectAgain = "Zuletzt genutztes Gerät verbinden";
    public const string Disconnect = "Trennen";
    public const string CancelConnection = "Verbindung abbrechen";
    public const string CancelSearch = "Suche abbrechen";
    public const string SearchSleepingDevice = "Schlafendes Gerät suchen";
    public const string ClearDisplay = "Anzeige löschen";
    public const string TestMode = "Testmodus";
    public const string FoundDevices = "Gefundene Geräte";
    public const string ScanEnded = "Scan beendet";
    public const string SearchIsRunning = "Geräteauswahl läuft. BlePrompter bitte eingeschaltet lassen.";
    public const string FilteredScan = "Gefiltert nach BlePrompter und Nordic UART.";
    public const string AllDevicesScan = "Alle sichtbaren BLE-Geräte werden im Browserdialog angeboten.";
    public const string SleepingDeviceScan = "Lange Suche nach zyklischem Wake-Fenster.";
    public const string CyclicSleep = "Zyklischer Schlaf";
    public const string ConnectedWith = "Verbunden mit";
    public const string LastCommand = "Letzter Befehl";
    public const string Response = "Antwort";
    public const string DarkMode = "Dunkelmodus";
    public const string CloseButtonViewAfterSend = "Schließe Buttonansicht nach Senden";
    public const string Display = "Display";
    public const string DisplayInverted = "Displayanzeige invertiert";
    public const string DisplayRotated = "Displayanzeige drehen";
    public const string SkipSleepDisplayAfterClearDisplay = "Kein SLEEP-DISPLAY bei Anzeige löschen";
    public const string SleepDuration = "Schlafdauer";
    public const string ListenDuration = "Aktivzeit";
    public const string SendSleepCommand = "Gerät schlafen legen";
    public const string CameraView = "Kameraansicht";
    public const string CameraControls = "Kamera-Bedienung";
    public const string Opacity = "Deckkraft";
    public const string Normal = "Normal";
    public const string Minimal = "Unauffällig";
    public const string Custom = "Anpassbar";
    public const string Back = "Zurück";
    public const string HideControls = "Bedienung ausblenden";
    public const string ShowControls = "Bedienung einblenden";
    public const string CameraAccessMissing = "Kamerazugriff fehlt.";
    public const string AllowCameraAccess = "Erlaube den Kamerazugriff, damit die Liveansicht angezeigt werden kann.";
    public const string AllowCamera = "Kamera erlauben";
    public const string Pause = "PAUSE";
    public const string ContinueVideo = "Video fortsetzen";
    public const string PauseVideo = "Video pausieren";
    public const string Zoom = "Zoom";
    public const string Send = "Senden";
    public const string Text = "Text";
    public const string ClearText = "Text löschen";
    public const string NoLogEntries = "Noch keine Logeinträge.";
    public const string BluetoothUnsupported = "Dieser Browser unterstützt Web Bluetooth nicht. Nutze Android Chrome oder iOS Bluefy.";
    public const string CameraUnsupported = "Dieser Browser stellt keine Kamera-API bereit.";
    public const string BluetoothPermissionDenied = "Bluetooth-Geräteauswahl wurde abgebrochen oder nicht erlaubt.";
    public const string BluetoothConnectionFailed = "Die BLE-Verbindung ist fehlgeschlagen.";
    public const string BluetoothDisconnected = "Verbindung getrennt.";
    public const string BluetoothNotConnected = "BlePrompter ist nicht verbunden.";
    public const string TestModeEnabled = "Testmodus aktiv. Bluetooth wird nicht verwendet.";
    public const string TestModeDisabled = "Testmodus beendet.";
    public const string DeviceIsAwake = "Gerät ist wach.";
    public const string SleepingDeviceNotFound = "Schlafendes Gerät wurde nicht gefunden.";
    public const string SymbolMissing = "Symbol fehlt.";
    public const string SetupHint = "GitHub Pages liefert die PWA über HTTPS aus. Für BLE muss die Geräteauswahl direkt durch Antippen gestartet werden.";
    public const string ReconnectHint = "Web Bluetooth erlaubt Auto-Reconnect nur für Geräte, die dieser Webseite bereits freigegeben wurden.";

    public static string GetMetadataLine(string buildTimestamp) => $"{ApplicationKind} · {Build} {buildTimestamp}";

    public static string GetMenuLabel(MenuItem menuItem) => menuItem switch
    {
        MenuItem.Client => Client,
        MenuItem.Einstellungen => Settings,
        MenuItem.Log => Log,
        MenuItem.About => About,
        _ => Client
    };

    public static string GetHelpButtonLabel(string title) => $"{OpenHelp}: {title}";

    public static string GetModeLabel(ModeLabel modeLabel) => modeLabel switch
    {
        ModeLabel.Pfeile => "Pfeile",
        ModeLabel.Karten => "Karten",
        ModeLabel.Text => Text,
        ModeLabel.Esp => "ESP",
        ModeLabel.Wuerfel => "Würfel",
        _ => "Pfeile"
    };

    public static string GetConnectionStatusText(ConnectionStatus status) => status switch
    {
        ConnectionStatus.NotConnected => "Nicht verbunden",
        ConnectionStatus.PermissionMissing => "Berechtigung fehlt",
        ConnectionStatus.BluetoothOff => "Bluetooth aus",
        ConnectionStatus.SearchingDevice => "Suche Gerät",
        ConnectionStatus.SearchingSleepingDevice => "Suche schlafendes Gerät",
        ConnectionStatus.Connecting => "Verbinde",
        ConnectionStatus.Connected => "Verbunden",
        ConnectionStatus.TestMode => TestMode,
        ConnectionStatus.Disconnected => "Getrennt",
        ConnectionStatus.Error => "Fehler",
        _ => "Nicht verbunden"
    };

    public static string GetCameraViewStyleLabel(CameraViewStyle cameraViewStyle) => cameraViewStyle switch
    {
        CameraViewStyle.Android => "Android",
        CameraViewStyle.Ios => "iOS",
        CameraViewStyle.Fantasy => "Fantasie",
        _ => "Android"
    };

    public static string GetCameraButtonAppearanceLabel(CameraButtonAppearance cameraButtonAppearance) =>
        cameraButtonAppearance switch
        {
            CameraButtonAppearance.Normal => Normal,
            CameraButtonAppearance.Minimal => Minimal,
            CameraButtonAppearance.Custom => Custom,
            _ => Normal
        };
}
