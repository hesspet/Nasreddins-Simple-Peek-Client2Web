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

    public async Task<string> SelectTestVideoFileAsync(string fileInputElementIdentifier)
    {
        try
        {
            return await javaScriptRuntime.InvokeAsync<string>("nasreddinCamera.selectTestVideoFile", fileInputElementIdentifier);
        }
        catch (JSException exception)
        {
            throw new InvalidOperationException(GermanText.TestVideoUnsupported, exception);
        }
    }

    public async Task RemoveTestVideoFileAsync(string fileInputElementIdentifier)
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.removeTestVideoFile", fileInputElementIdentifier);
    }

    public async Task StartTestVideoAsync(string videoElementIdentifier, double zoom)
    {
        try
        {
            await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.startTestVideo", videoElementIdentifier, zoom);
        }
        catch (JSException exception)
        {
            throw new InvalidOperationException(GermanText.TestVideoMissing, exception);
        }
    }

    public async Task SetPausedAsync(string videoElementIdentifier, bool isPaused)
    {
        await javaScriptRuntime.InvokeVoidAsync("nasreddinCamera.setPaused", videoElementIdentifier, isPaused);
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
