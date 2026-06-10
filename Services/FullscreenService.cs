using Microsoft.JSInterop;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class FullscreenService(IJSRuntime javaScriptRuntime)
{
    public async Task<bool> TryFullscreenAsync() =>
        await javaScriptRuntime.InvokeAsync<bool>("peekUi.tryFullscreen");
}
