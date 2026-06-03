using Microsoft.JSInterop;
using NasreddinsSimplePeekClient2Web.Models;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class BluetoothPrompterClient : IAsyncDisposable
{
    private readonly IJSRuntime javaScriptRuntime;
    private readonly ApplicationStateStore stateStore;
    private DotNetObjectReference<BluetoothPrompterClient>? objectReference;

    public BluetoothPrompterClient(IJSRuntime javaScriptRuntime, ApplicationStateStore stateStore)
    {
        this.javaScriptRuntime = javaScriptRuntime;
        this.stateStore = stateStore;
    }

    public async Task<bool> CheckSupportAsync() =>
        await javaScriptRuntime.InvokeAsync<bool>("nasreddinBluetooth.checkSupport");

    public async Task<bool> RequestAndConnectAsync(BluetoothScanMode scanMode)
    {
        if (!await CheckSupportAsync())
        {
            stateStore.Update(currentState => currentState with
            {
                ConnectionStatus = ConnectionStatus.PermissionMissing,
                LastResponse = GermanText.BluetoothUnsupported
            });
            stateStore.AddLogEntry(GermanText.BluetoothUnsupported);
            return false;
        }

        objectReference ??= DotNetObjectReference.Create(this);
        stateStore.Update(currentState => currentState with
        {
            ConnectionStatus = scanMode == BluetoothScanMode.SleepingDevice
                ? ConnectionStatus.SearchingSleepingDevice
                : ConnectionStatus.SearchingDevice,
            IsBluetoothOperationRunning = true,
            ScanMode = scanMode,
            DiscoveredDevices = Array.Empty<BluetoothDeviceInformation>(),
            SleepingDeviceSearchText = scanMode == BluetoothScanMode.SleepingDevice
                ? "Geräteauswahl für schlafendes Gerät geöffnet."
                : ""
        });

        try
        {
            var bluetoothDevice = await javaScriptRuntime.InvokeAsync<BluetoothDeviceInformation>(
                "nasreddinBluetooth.requestAndConnect",
                objectReference,
                scanMode.ToString(),
                AppConstants.BluetoothPrompterServiceUuid,
                AppConstants.BluetoothPrompterReceiveCharacteristicUuid,
                AppConstants.BluetoothPrompterTransmitCharacteristicUuid,
                AppConstants.BluetoothPrompterNamePrefix);

            ApplyConnectedDevice(bluetoothDevice);

            if (scanMode == BluetoothScanMode.SleepingDevice)
            {
                await SendCommandAsync("WAKE");
                stateStore.Update(currentState => currentState with
                {
                    SleepingDeviceSearchText = GermanText.DeviceIsAwake,
                    LastResponse = GermanText.DeviceIsAwake
                });
                stateStore.AddLogEntry(GermanText.DeviceIsAwake);
            }

            return true;
        }
        catch (JSException javaScriptException)
        {
            var message = string.IsNullOrWhiteSpace(javaScriptException.Message)
                ? GermanText.BluetoothConnectionFailed
                : MapBluetoothError(javaScriptException.Message);
            stateStore.Update(currentState => currentState with
            {
                ConnectionStatus = ConnectionStatus.Error,
                IsBluetoothOperationRunning = false,
                LastResponse = message,
                SleepingDeviceSearchText = scanMode == BluetoothScanMode.SleepingDevice
                    ? GermanText.SleepingDeviceNotFound
                    : currentState.SleepingDeviceSearchText
            });
            stateStore.AddLogEntry(message);
            return false;
        }
    }

    public async Task<bool> ReconnectRememberedDeviceAsync(RememberedBluetoothDevice rememberedBluetoothDevice)
    {
        if (!await CheckSupportAsync())
        {
            stateStore.Update(currentState => currentState with { LastResponse = GermanText.BluetoothUnsupported });
            return false;
        }

        objectReference ??= DotNetObjectReference.Create(this);
        stateStore.Update(currentState => currentState with
        {
            ConnectionStatus = ConnectionStatus.Connecting,
            IsBluetoothOperationRunning = true,
            LastResponse = $"{GermanText.ConnectAgain}: {rememberedBluetoothDevice.Name}"
        });

        try
        {
            var bluetoothDevice = await javaScriptRuntime.InvokeAsync<BluetoothDeviceInformation>(
                "nasreddinBluetooth.reconnectRememberedDevice",
                objectReference,
                rememberedBluetoothDevice.Id,
                rememberedBluetoothDevice.Name,
                AppConstants.BluetoothPrompterServiceUuid,
                AppConstants.BluetoothPrompterReceiveCharacteristicUuid,
                AppConstants.BluetoothPrompterTransmitCharacteristicUuid);

            ApplyConnectedDevice(bluetoothDevice);
            return true;
        }
        catch (JSException javaScriptException)
        {
            var message = MapBluetoothError(javaScriptException.Message);
            stateStore.Update(currentState => currentState with
            {
                ConnectionStatus = ConnectionStatus.NotConnected,
                IsBluetoothOperationRunning = false,
                LastResponse = message
            });
            stateStore.AddLogEntry(message);
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinBluetooth.disconnect");
        stateStore.Update(currentState => currentState with
        {
            ConnectionStatus = ConnectionStatus.NotConnected,
            IsConnected = false,
            ConnectedDeviceName = "",
            IsBluetoothOperationRunning = false
        });
    }

    public async Task<bool> SendCommandAsync(string command)
    {
        try
        {
            await javaScriptRuntime.InvokeVoidAsync("nasreddinBluetooth.sendCommand", command);
            stateStore.AddLogEntry($"Gesendet: {command}");
            return true;
        }
        catch (JSException javaScriptException)
        {
            var message = MapBluetoothError(javaScriptException.Message);
            stateStore.Update(currentState => currentState with { LastResponse = message });
            stateStore.AddLogEntry(message);
            return false;
        }
    }

    [JSInvokable]
    public void HandleBluetoothResponse(string responseText)
    {
        stateStore.Update(currentState => currentState with { LastResponse = responseText });
        stateStore.AddLogEntry($"Antwort: {responseText}");
    }

    [JSInvokable]
    public void HandleBluetoothDisconnected()
    {
        stateStore.Update(currentState => currentState with
        {
            ConnectionStatus = ConnectionStatus.Disconnected,
            IsConnected = false,
            ConnectedDeviceName = "",
            IsBluetoothOperationRunning = false,
            LastResponse = GermanText.BluetoothDisconnected
        });
        stateStore.AddLogEntry(GermanText.BluetoothDisconnected);
    }

    private void ApplyConnectedDevice(BluetoothDeviceInformation bluetoothDevice)
    {
        stateStore.Update(currentState => currentState with
        {
            ConnectedDeviceName = bluetoothDevice.Name,
            ConnectionStatus = ConnectionStatus.Connected,
            IsBluetoothOperationRunning = false,
            IsConnected = true,
            LastResponse = "-",
            DiscoveredDevices = Array.Empty<BluetoothDeviceInformation>()
        });
        stateStore.RememberDevice(bluetoothDevice);
        stateStore.AddLogEntry($"Verbunden mit {bluetoothDevice.Name}.");
    }

    private static string MapBluetoothError(string errorMessage)
    {
        if (errorMessage.Contains("not supported", StringComparison.OrdinalIgnoreCase))
        {
            return GermanText.BluetoothUnsupported;
        }

        if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("cancel", StringComparison.OrdinalIgnoreCase))
        {
            return GermanText.BluetoothPermissionDenied;
        }

        if (errorMessage.Contains("not connected", StringComparison.OrdinalIgnoreCase))
        {
            return GermanText.BluetoothNotConnected;
        }

        return GermanText.BluetoothConnectionFailed;
    }

    public async ValueTask DisposeAsync()
    {
        objectReference?.Dispose();
        await javaScriptRuntime.InvokeVoidAsync("nasreddinBluetooth.disconnect");
    }
}
