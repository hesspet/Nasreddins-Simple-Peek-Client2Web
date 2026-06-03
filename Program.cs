using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NasreddinsSimplePeekClient2Web;
using NasreddinsSimplePeekClient2Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<ApplicationStateStore>();
builder.Services.AddScoped<SettingsStorage>();
builder.Services.AddScoped<BluetoothPrompterClient>();
builder.Services.AddScoped<CommandService>();
builder.Services.AddScoped<VideoCameraService>();

await builder.Build().RunAsync();
