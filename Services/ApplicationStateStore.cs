using System.Globalization;
using NasreddinsSimplePeekClient2Web.Models;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class ApplicationStateStore
{
    private static readonly CultureInfo GermanCulture = CultureInfo.GetCultureInfo("de-DE");

    public ApplicationState State { get; private set; } = new();

    public event Action? StateChanged;

    public void Update(Func<ApplicationState, ApplicationState> updateState)
    {
        State = updateState(State);
        StateChanged?.Invoke();
    }

    public PersistedApplicationSettings GetPersistedSettings() => new()
    {
        CustomCameraButtonOpacity = State.CustomCameraButtonOpacity,
        CycleListenSeconds = State.CycleListenSeconds,
        CycleSleepSeconds = State.CycleSleepSeconds,
        IsDarkModeEnabled = State.IsDarkModeEnabled,
        IsDisplayInverted = State.IsDisplayInverted,
        IsDisplayRotated = State.IsDisplayRotated,
        RememberedBluetoothDevice = State.RememberedBluetoothDevice,
        SelectedCameraButtonAppearance = State.SelectedCameraButtonAppearance,
        SelectedCameraViewStyle = State.SelectedCameraViewStyle,
        ShouldCloseButtonViewAfterSend = State.ShouldCloseButtonViewAfterSend,
        ShouldSkipSleepDisplayAfterClearDisplay = State.ShouldSkipSleepDisplayAfterClearDisplay
    };

    public void ApplyPersistedSettings(PersistedApplicationSettings settings)
    {
        Update(currentState => currentState with
        {
            CustomCameraButtonOpacity = ClampCameraButtonOpacity(settings.CustomCameraButtonOpacity),
            CycleListenSeconds = ClampInteger(settings.CycleListenSeconds, AppConstants.MinimumCycleListenSeconds, AppConstants.MaximumCycleListenSeconds),
            CycleSleepSeconds = ClampInteger(settings.CycleSleepSeconds, AppConstants.MinimumCycleSleepSeconds, AppConstants.MaximumCycleSleepSeconds),
            IsDarkModeEnabled = settings.IsDarkModeEnabled,
            IsDisplayInverted = settings.IsDisplayInverted,
            IsDisplayRotated = settings.IsDisplayRotated,
            RememberedBluetoothDevice = settings.RememberedBluetoothDevice,
            SelectedCameraButtonAppearance = settings.SelectedCameraButtonAppearance,
            SelectedCameraViewStyle = settings.SelectedCameraViewStyle,
            ShouldCloseButtonViewAfterSend = settings.ShouldCloseButtonViewAfterSend,
            ShouldSkipSleepDisplayAfterClearDisplay = settings.ShouldSkipSleepDisplayAfterClearDisplay
        });
    }

    public void AddLogEntry(string message)
    {
        var timestamp = DateTimeOffset.Now.ToString("HH:mm:ss", GermanCulture);
        Update(currentState => currentState with
        {
            MessageLog = new[] { $"{timestamp} {message}" }
                .Concat(currentState.MessageLog)
                .Take(AppConstants.MaximumLogEntries)
                .ToArray()
        });
    }

    public void RememberDevice(BluetoothDeviceInformation bluetoothDevice)
    {
        var rememberedDevice = new RememberedBluetoothDevice(
            bluetoothDevice.Id,
            bluetoothDevice.Name,
            bluetoothDevice.DeviceIdentifier,
            bluetoothDevice.SignalStrength,
            bluetoothDevice.LastSeenText,
            FormatEuropeanDate(DateTimeOffset.Now),
            AppConstants.BluetoothPrompterServiceUuid,
            AppConstants.BluetoothPrompterReceiveCharacteristicUuid,
            AppConstants.BluetoothPrompterTransmitCharacteristicUuid);

        Update(currentState => currentState with { RememberedBluetoothDevice = rememberedDevice });
    }

    public static string FormatEuropeanDate(DateTimeOffset date) => date.ToString("dd.MM.yyyy", GermanCulture);

    public static string FormatEuropeanDateTime(DateTimeOffset date) => date.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss", GermanCulture);

    public static string FormatRecordingDuration(int totalSeconds)
    {
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    public static double ClampCameraZoom(double value) => Math.Max(0, Math.Min(AppConstants.MaximumCameraZoom, value));

    public static double ClampCameraButtonOpacity(double value) =>
        Math.Max(AppConstants.MinimumCameraButtonOpacity, Math.Min(AppConstants.MaximumCameraButtonOpacity, value));

    public static int ClampInteger(int value, int minimumValue, int maximumValue) =>
        Math.Max(minimumValue, Math.Min(maximumValue, value));

    public static double GetCameraButtonOpacity(CameraButtonAppearance selectedCameraButtonAppearance, double customCameraButtonOpacity) =>
        selectedCameraButtonAppearance switch
        {
            CameraButtonAppearance.Normal => 1,
            CameraButtonAppearance.Transparent => 0.72,
            CameraButtonAppearance.Minimal => 0.28,
            CameraButtonAppearance.Custom => ClampCameraButtonOpacity(customCameraButtonOpacity),
            _ => 1
        };

    public static string GetDeviceDisplayName(BluetoothDeviceInformation bluetoothDevice)
    {
        var identifierText = string.IsNullOrWhiteSpace(bluetoothDevice.DeviceIdentifier)
            ? ""
            : $"{bluetoothDevice.DeviceIdentifier} · ";
        var signalText = bluetoothDevice.SignalStrength is null
            ? "RSSI unbekannt"
            : $"RSSI {bluetoothDevice.SignalStrength}";
        return $"{identifierText}{bluetoothDevice.Name} · {signalText} · {bluetoothDevice.LastSeenText}";
    }

    public static string GetScanModeText(BluetoothScanMode scanMode) => scanMode switch
    {
        BluetoothScanMode.AllDevices => GermanText.AllDevicesScan,
        BluetoothScanMode.SleepingDevice => GermanText.SleepingDeviceScan,
        _ => GermanText.FilteredScan
    };
}
