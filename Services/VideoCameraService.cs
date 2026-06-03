using Microsoft.JSInterop;
using NasreddinsSimplePeekClient2Web.Resources;

namespace NasreddinsSimplePeekClient2Web.Services;

public sealed class VideoCameraService(IJSRuntime javaScriptRuntime)
{
    public async Task<bool> CheckSupportAsync() =>
        await javaScriptRuntime.InvokeAsync<bool>("nasreddinCamera.checkSupport");

    public async Task StartCameraAsync(string videoElementIdentifier, double zoom)
    {
        if (!await CheckSupportAsync())
        {
            throw new InvalidOperationException(GermanText.CameraUnsupported);
        }

        await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.start", videoElementIdentifier, zoom);
    }

    public async Task StopCameraAsync(string videoElementIdentifier)
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.stop", videoElementIdentifier);
    }

    public async Task SetZoomAsync(string videoElementIdentifier, double zoom)
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.setZoom", videoElementIdentifier, zoom);
    }
}
