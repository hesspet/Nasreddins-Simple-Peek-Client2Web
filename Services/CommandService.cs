using NasreddinsSimplePeekClient2Web.Models;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class CommandService(ApplicationStateStore stateStore, BluetoothPrompterClient bluetoothPrompterClient)
{
    public async Task<bool> SendCommandAsync(string command)
    {
        stateStore.Update(currentState => currentState with { LastCommand = command });

        if (stateStore.State.IsTestModeEnabled)
        {
            var message = $"Testmodus: {command}";
            stateStore.Update(currentState => currentState with { LastResponse = message });
            stateStore.AddLogEntry($"Testmodus gesendet: {command}");
            return true;
        }

        if (!stateStore.State.IsConnected)
        {
            stateStore.Update(currentState => currentState with { LastResponse = GermanText.BluetoothNotConnected });
            stateStore.AddLogEntry(GermanText.BluetoothNotConnected);
            return false;
        }

        return await bluetoothPrompterClient.SendCommandAsync(command);
    }

    public async Task ClearDisplayAsync()
    {
        var wasCleared = await SendCommandAsync("CL");

        if (wasCleared && !stateStore.State.ShouldSkipSleepDisplayAfterClearDisplay)
        {
            await SendCommandAsync("SLEEP DISPLAY");
        }
    }

    public async Task SendCycleSleepCommandAsync()
    {
        await SendCommandAsync($"SLEEP CYCLE {stateStore.State.CycleSleepSeconds} {stateStore.State.CycleListenSeconds}");
    }

    public async Task<bool> SendSleepResetCommandAsync() => await SendCommandAsync("SLEEP RESET");

    public async Task SendCommandAndMaybeCloseOverlayAsync(string command)
    {
        var wasSent = await SendCommandAsync(command);

        if (wasSent && stateStore.State.ShouldCloseButtonViewAfterSend)
        {
            stateStore.Update(currentState => currentState with { AreFullScreenControlsVisible = false });
        }
    }

    public async Task SendCardRankAsync(string rankCode)
    {
        var wasSent = await SendCommandAsync($"C{stateStore.State.SelectedSuitCode}{rankCode}");

        if (!wasSent)
        {
            return;
        }

        if (stateStore.State.WasCardSuitButtonPressed && stateStore.State.ShouldCloseButtonViewAfterSend)
        {
            stateStore.Update(currentState => currentState with { AreFullScreenControlsVisible = false });
        }

        stateStore.Update(currentState => currentState with { WasCardSuitButtonPressed = false });
    }

    public async Task SendSymbolAsync()
    {
        var normalizedSymbolText = stateStore.State.SymbolText.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedSymbolText))
        {
            stateStore.Update(currentState => currentState with { LastResponse = GermanText.SymbolMissing });
            stateStore.AddLogEntry(GermanText.SymbolMissing);
            return;
        }

        var wasSent = await SendCommandAsync($"SYMBOL {normalizedSymbolText[..Math.Min(2, normalizedSymbolText.Length)]}");

        if (!wasSent)
        {
            return;
        }

        stateStore.Update(currentState => currentState with
        {
            SymbolText = "",
            AreFullScreenControlsVisible = currentState.ShouldCloseButtonViewAfterSend ? false : currentState.AreFullScreenControlsVisible
        });
    }
}
