using Microsoft.JSInterop;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class BackExitGuardService(IJSRuntime javaScriptRuntime) : IAsyncDisposable
{
    private DotNetObjectReference<BackExitGuardService>? objectReference;
    private bool isInitialized;

    public event Func<Task>? ExitRequested;

    public async Task InitializeAsync()
    {
        objectReference ??= DotNetObjectReference.Create(this);

        if (isInitialized)
        {
            return;
        }

        await javaScriptRuntime.InvokeVoidAsync("nasreddinBackExitGuard.initialize", objectReference);
        isInitialized = true;
    }

    public async Task CancelExitAsync()
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinBackExitGuard.cancelExit");
    }

    [JSInvokable]
    public async Task RequestExitAsync()
    {
        var handler = ExitRequested;

        if (handler is not null)
        {
            await handler.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (isInitialized)
            {
                await javaScriptRuntime.InvokeVoidAsync("nasreddinBackExitGuard.dispose");
            }
        }
        catch (JSException)
        {
        }

        objectReference?.Dispose();
        objectReference = null;
        isInitialized = false;
    }
}
