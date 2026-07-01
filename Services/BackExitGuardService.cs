using Microsoft.JSInterop;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class BackExitGuardService(IJSRuntime javaScriptRuntime) : IAsyncDisposable
{
    private DotNetObjectReference<BackExitGuardService>? objectReference;
    private bool isInitialized;
    private Func<bool>? canHandleBack;
    private Func<Task>? handleBack;

    public event Func<Task>? ExitRequested;
    public bool CanGoBack => canHandleBack?.Invoke() == true;

    public IDisposable RegisterBackHandler(Func<bool> nextCanHandleBack, Func<Task> nextHandleBack)
    {
        canHandleBack = nextCanHandleBack;
        handleBack = nextHandleBack;

        return new BackHandlerRegistration(this, nextCanHandleBack, nextHandleBack);
    }

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

    public async Task ConfirmExitAsync()
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinBackExitGuard.confirmExit");
    }

    public async Task HandleBackAsync()
    {
        var handler = handleBack;

        if (handler is not null)
        {
            await handler.Invoke();
        }
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

    private void UnregisterBackHandler(Func<bool> registeredCanHandleBack, Func<Task> registeredHandleBack)
    {
        if (canHandleBack == registeredCanHandleBack && handleBack == registeredHandleBack)
        {
            canHandleBack = null;
            handleBack = null;
        }
    }

    private sealed class BackHandlerRegistration(
        BackExitGuardService service,
        Func<bool> registeredCanHandleBack,
        Func<Task> registeredHandleBack) : IDisposable
    {
        private bool isDisposed;

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            service.UnregisterBackHandler(registeredCanHandleBack, registeredHandleBack);
            isDisposed = true;
        }
    }
}
