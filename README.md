# Nasreddins Simple Peek Client 2 Web

Blazor-WebAssembly-PWA zur Steuerung des BlePrompter per Web Bluetooth.

## Start

```powershell
cd C:\dev\Nasreddins-Simple-Peek-Client2Web
dotnet run --no-launch-profile --urls http://127.0.0.1:5088
```

Danach öffnen:

```text
http://127.0.0.1:5088
```

## Prüfen

```powershell
dotnet build
dotnet publish -c Release
```

## Zielbrowser

- Android: Chrome
- iOS: Bluefy

Safari und Chrome auf iOS unterstützen Web Bluetooth nicht nativ.

## Projektkontext

Die ausführliche Übergabe steht in `PROJEKTUEBERSICHT.md`.
