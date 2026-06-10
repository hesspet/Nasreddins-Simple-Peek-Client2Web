using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Models;

public enum MenuItem
{
    Client,
    Einstellungen,
    Log,
    About
}

public enum ModeLabel
{
    Pfeile,
    Karten,
    Text,
    Esp,
    Wuerfel
}

public enum BluetoothScanMode
{
    Filtered,
    AllDevices,
    SleepingDevice
}

public enum ConnectionStatus
{
    NotConnected,
    PermissionMissing,
    BluetoothOff,
    SearchingDevice,
    SearchingSleepingDevice,
    Connecting,
    Connected,
    TestMode,
    Disconnected,
    Error
}

public enum CameraViewStyle
{
    Android,
    Ios,
    Fantasy
}

public enum CameraButtonAppearance
{
    Normal = 0,
    Minimal = 2,
    Custom = 3
}

public enum VideoSourceType
{
    LiveCamera,
    TestVideo
}

public sealed record DirectionCommand(string Label, string Arrow, string Command);

public sealed record SuitOption(string Label, string Mark, string Code, bool IsRed);

public sealed record RankOption(string Label, string Code);

public sealed record SymbolCommand(string Label, string Command, string Symbol);

public sealed record TestVideoFileSelection(string FileName, bool IsRegistered, string ErrorText = "");

public sealed record HelpTopic
{
    public string Id { get; init; } = "";
    public string Title { get; init; } = "";
    public string Scope { get; init; } = "";
    public string Context { get; init; } = "";
    public string File { get; init; } = "";
}

public sealed record BluetoothDeviceInformation(
    string Id,
    string Name,
    string? DeviceIdentifier,
    int? SignalStrength,
    string LastSeenText);

public sealed record RememberedBluetoothDevice(
    string Id,
    string Name,
    string? DeviceIdentifier,
    int? SignalStrength,
    string LastSeenText,
    string LastConnectedDate,
    string ServiceUuid,
    string ReceiveCharacteristicUuid,
    string TransmitCharacteristicUuid);

public sealed record PersistedApplicationSettings
{
    public bool IsDarkModeEnabled { get; init; }
    public bool IsDisplayInverted { get; init; }
    public bool IsDisplayRotated { get; init; }
    public bool IsHelpSystemHidden { get; init; }
    public bool ShouldCloseButtonViewAfterSend { get; init; } = true;
    public bool ShouldSkipSleepDisplayAfterClearDisplay { get; init; }
    public CameraViewStyle SelectedCameraViewStyle { get; init; } = CameraViewStyle.Android;
    public CameraButtonAppearance SelectedCameraButtonAppearance { get; init; } = CameraButtonAppearance.Normal;
    public VideoSourceType SelectedVideoSourceType { get; init; } = VideoSourceType.LiveCamera;
    public double CustomCameraButtonOpacity { get; init; } = 0.18;
    public int CycleSleepSeconds { get; init; } = AppConstants.DefaultCycleSleepSeconds;
    public int CycleListenSeconds { get; init; } = AppConstants.DefaultCycleListenSeconds;
    public RememberedBluetoothDevice? RememberedBluetoothDevice { get; init; }
}

public sealed record ApplicationState
{
    public MenuItem SelectedMenuItem { get; init; } = MenuItem.Client;
    public bool IsMenuOpen { get; init; }
    public bool IsDarkModeEnabled { get; init; }
    public bool IsHelpSystemHidden { get; init; }
    public bool ShouldCloseButtonViewAfterSend { get; init; } = true;
    public bool ShouldSkipSleepDisplayAfterClearDisplay { get; init; }
    public CameraViewStyle SelectedCameraViewStyle { get; init; } = CameraViewStyle.Android;
    public CameraButtonAppearance SelectedCameraButtonAppearance { get; init; } = CameraButtonAppearance.Normal;
    public VideoSourceType SelectedVideoSourceType { get; init; } = VideoSourceType.LiveCamera;
    public double CustomCameraButtonOpacity { get; init; } = 0.18;
    public ConnectionStatus ConnectionStatus { get; init; } = ConnectionStatus.NotConnected;
    public bool IsConnected { get; init; }
    public bool IsTestModeEnabled { get; init; }
    public bool IsBluetoothOperationRunning { get; init; }
    public BluetoothScanMode ScanMode { get; init; } = BluetoothScanMode.Filtered;
    public string ConnectedDeviceName { get; init; } = "";
    public ModeLabel SelectedMode { get; init; } = ModeLabel.Pfeile;
    public ModeLabel? FullScreenMode { get; init; }
    public bool AreFullScreenControlsVisible { get; init; }
    public DateTimeOffset? RecordingStartedAt { get; init; }
    public int RecordingElapsedSeconds { get; init; }
    public double CameraZoom { get; init; }
    public bool IsFakeVideoPaused { get; init; }
    public string SelectedTestVideoFileName { get; init; } = "";
    public bool IsTestVideoFileRegistered { get; init; }
    public string TestVideoSelectionErrorText { get; init; } = "";
    public string SelectedSuitCode { get; init; } = "H";
    public bool WasCardSuitButtonPressed { get; init; }
    public string SymbolText { get; init; } = "";
    public bool IsDisplayInverted { get; init; }
    public bool IsDisplayRotated { get; init; }
    public int CycleSleepSeconds { get; init; } = AppConstants.DefaultCycleSleepSeconds;
    public int CycleListenSeconds { get; init; } = AppConstants.DefaultCycleListenSeconds;
    public string SleepingDeviceSearchText { get; init; } = "";
    public string LastCommand { get; init; } = "-";
    public string LastResponse { get; init; } = "-";
    public IReadOnlyList<string> MessageLog { get; init; } = Array.Empty<string>();
    public IReadOnlyList<BluetoothDeviceInformation> DiscoveredDevices { get; init; } = Array.Empty<BluetoothDeviceInformation>();
    public RememberedBluetoothDevice? RememberedBluetoothDevice { get; init; }
}

public static class AppConstants
{
    public const string HelpOverviewIdentifier = "overview";
    public const string ClientHelpIdentifier = "view-peeker";
    public const string SettingsHelpIdentifier = "view-settings";
    public const string FullScreenHelpIdentifier = "view-fullscreen";
    public const string LogHelpIdentifier = "view-log";
    public const string AboutHelpIdentifier = "view-about";
    public const string ConnectHelpIdentifier = "element-connect";
    public const string ReconnectHelpIdentifier = "element-reconnect";
    public const string SearchSleepingDeviceHelpIdentifier = "element-search-sleeping-device";
    public const string TestModeHelpIdentifier = "element-test-mode";
    public const string ClearDisplayHelpIdentifier = "element-clear-display";
    public const string HideHelpSystemHelpIdentifier = "element-hide-help-system";
    public const string DarkModeHelpIdentifier = "element-dark-mode";
    public const string CloseButtonViewAfterSendHelpIdentifier = "element-close-button-view-after-send";
    public const string DisplayInvertedHelpIdentifier = "element-display-inverted";
    public const string DisplayRotatedHelpIdentifier = "element-display-rotated";
    public const string SkipSleepDisplayAfterClearDisplayHelpIdentifier = "element-skip-sleep-display-after-clear-display";
    public const string CyclicSleepHelpIdentifier = "element-cyclic-sleep";
    public const string CycleSleepDurationHelpIdentifier = "element-cycle-sleep-duration";
    public const string CycleListenDurationHelpIdentifier = "element-cycle-listen-duration";
    public const string CameraViewHelpIdentifier = "element-camera-view";
    public const string VideoSourceHelpIdentifier = "element-video-source";
    public const string CameraControlsHelpIdentifier = "element-camera-controls";
    public const string CameraButtonOpacityHelpIdentifier = "element-camera-button-opacity";
    public const string ForgetRememberedDeviceHelpIdentifier = "element-forget-remembered-device";
    public const string SleepResetHelpIdentifier = "element-sleep-reset";
    public const string BluetoothPrompterServiceUuid = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
    public const string BluetoothPrompterReceiveCharacteristicUuid = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
    public const string BluetoothPrompterTransmitCharacteristicUuid = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
    public const string BluetoothPrompterNamePrefix = "BlePrompter";
    public const int DefaultCycleSleepSeconds = 30;
    public const int DefaultCycleListenSeconds = 10;
    public const int MinimumCycleSleepSeconds = 5;
    public const int MaximumCycleSleepSeconds = 60;
    public const int MinimumCycleListenSeconds = 10;
    public const int MaximumCycleListenSeconds = 120;
    public const int MinimumReconnectWaitSeconds = 30;
    public const int MaximumReconnectWaitSeconds = 150;
    public const int MaximumLogEntries = 8;
    public const double MaximumCameraZoom = 0.7;
    public const double MinimumCameraButtonOpacity = 0.05;
    public const double MaximumCameraButtonOpacity = 1;

    public static readonly IReadOnlyList<MenuItem> MenuItems =
    [
        MenuItem.Client,
        MenuItem.Einstellungen,
        MenuItem.Log,
        MenuItem.About
    ];

    public static readonly IReadOnlyList<ModeLabel> ModeLabels =
    [
        ModeLabel.Pfeile,
        ModeLabel.Karten,
        ModeLabel.Text,
        ModeLabel.Esp,
        ModeLabel.Wuerfel
    ];

    public static readonly IReadOnlyList<CameraButtonAppearance> CameraButtonAppearances =
    [
        CameraButtonAppearance.Normal,
        CameraButtonAppearance.Minimal,
        CameraButtonAppearance.Custom
    ];

    public static readonly IReadOnlyList<VideoSourceType> VideoSourceTypes =
    [
        VideoSourceType.LiveCamera,
        VideoSourceType.TestVideo
    ];

    public static readonly IReadOnlyList<DirectionCommand> DirectionCommands =
    [
        new("NW", IconSvg.ArrowNorthWest, "ANW"),
        new("N",  IconSvg.ArrowNorth,     "AN"),
        new("NO", IconSvg.ArrowNorthEast, "ANE"),
        new("W",  IconSvg.ArrowWest,      "AW"),
        new("O",  IconSvg.ArrowEast,      "AE"),
        new("SW", IconSvg.ArrowSouthWest, "ASW"),
        new("S",  IconSvg.ArrowSouth,     "AS"),
        new("SO", IconSvg.ArrowSouthEast, "ASE")
    ];

    public static readonly IReadOnlyList<SuitOption> SuitOptions =
    [
        new("Herz", "♥", "H", true),
        new("Karo", "♦", "D", true),
        new("Kreuz", "♣", "C", false),
        new("Pik", "♠", "S", false)
    ];

    public static readonly IReadOnlyList<RankOption> RankOptions =
    [
        new("Ass", "1"),
        new("2", "2"),
        new("3", "3"),
        new("4", "4"),
        new("5", "5"),
        new("6", "6"),
        new("7", "7"),
        new("8", "8"),
        new("9", "9"),
        new("10", "X"),
        new("Bube", "J"),
        new("Dame", "Q"),
        new("König", "K")
    ];

    public static readonly IReadOnlyList<SymbolCommand> EspSymbolCommands =
    [
        new("Kreis", "EC", "○"),
        new("Kreuz", "EG", "+"),
        new("Wellen", "EW", "≋"),
        new("Quadrat", "EQ", "□"),
        new("Stern", "ES", "☆")
    ];

    public static readonly IReadOnlyList<SymbolCommand> CubeCommands =
    [
        new("1", "CUBE 1", "⚀"),
        new("2", "CUBE 2", "⚁"),
        new("3", "CUBE 3", "⚂"),
        new("4", "CUBE 4", "⚃"),
        new("5", "CUBE 5", "⚄"),
        new("6", "CUBE 6", "⚅")
    ];
}
