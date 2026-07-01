# Projektübersicht: Nasreddins Simple Peek Client 2 Web

Stand: 01.07.2026

## Zweck

Dieses Projekt ist die Blazor-WebAssembly-PWA-Portierung von `C:\dev\Nasreddins-Simple-Peek-Client2`. Die Anwendung steuert den BlePrompter über Web Bluetooth und ist für Android Chrome sowie iOS Bluefy gedacht. Die Auslieferung erfolgt als statische PWA, lokal oder über GitHub Pages.

## Aktueller Stand

- Das Zielprojekt wurde als eigenständige Blazor WebAssembly PWA auf .NET 10 erstellt.
- Die React-Native-/Expo-Template-App wurde nicht übernommen; die Web-App ist eine neue C#-basierte Portierung.
- `dotnet build` wurde erfolgreich ausgeführt.
- `dotnet publish -c Release` wurde erfolgreich ausgeführt.
- Die NuGet-Ausgabe meldete nur `NU1900`, weil die Sicherheitsrisiko-Daten von `https://api.nuget.org/v3/index.json` in der Umgebung nicht geladen werden konnten. Das war kein Buildfehler.
- Der lokale Server kann mit `dotnet run --no-launch-profile --urls http://127.0.0.1:5088` gestartet werden.
- Ein zuvor blockierender `dotnet.exe` auf Port `5088` wurde beendet; danach konnte der Server vom Benutzer ohne Probleme gestartet werden.

## Wichtige Dateien

- `Pages/Home.razor`: Hauptseite, globaler UI-Fluss, Menü, Einstellungen, Vollbildmodus und State-Verkabelung.
- `Models/AppModels.cs`: Enums, App-State, persistierte Einstellungen, BLE-Geräteinformationen und Kommandokonstanten.
- `Services/ApplicationStateStore.cs`: Reaktiver App-Zustand und Hilfsformatierungen.
- `Services/BluetoothPrompterClient.cs`: C#-Fassade für Web Bluetooth.
- `Services/CommandService.cs`: Sendelogik für `CL`, `SLEEP DISPLAY`, Karten, Pfeile, Text, ESP, Würfel, Audio Spy und Schlafzyklus.
- `Services/AudioSpyMatcher.cs`: Schlüsselwort-Matching für Audio Spy mit Regex-/Wildcard-Unterstützung und Regex-Timeout.
- `Services/SettingsStorage.cs`: Persistenz über `localStorage` sowie JSON-Profilexport und -import.
- `Services/HelpContentService.cs`: Lädt den Hilfeindex und die Markdown-Hilfetexte aus `wwwroot/help/de`.
- `Services/VideoCameraService.cs`: C#-Fassade für Kamera-, Testvideo- und Zoom-Interop.
- `Services/BackExitGuardService.cs`: C#-Fassade für den globalen Zurück-/Verlassen-Guard.
- `Components/*.razor`: UI-Komponenten für Client, Audio Spy, Einstellungen, Log, About, Command-Controls, Hilfe und Kamera-Vollbild.
- `Components/BackExitGuard.razor`: Globaler Dialog beim ersten Browser-/Hardware-Zurückdruck.
- `Resources/GermanText.cs`: Zentraler deutscher Textkatalog für sichtbare UI-Texte.
- `Resources/Strings.resx`: Platzhalter-/Basis-Ressourcendatei für spätere echte Ressourcenlokalisierung.
- `wwwroot/help/de`: Pflegbare Markdown-Hilfetexte und `help-index.json`.
- `wwwroot/js/webBluetooth.js`: Web-Bluetooth-Interop.
- `wwwroot/js/videoCamera.js`: Kamera-Interop.
- `wwwroot/js/audioSpy.js`: Eingabeüberwachung für Audio Spy ohne State-Binding pro Zeichen.
- `wwwroot/js/settingsProfile.js`: Browser-Download für Einstellungsprofile.
- `wwwroot/js/backExitGuard.js`: History-API-Guard für Browser-/Hardware-Zurück.
- `.github/workflows/deploy-github-pages.yml`: GitHub-Pages-Deployment.
- `wwwroot/.nojekyll`: Stellt sicher, dass GitHub Pages Blazor-Ordner wie `_framework` ausliefert.

## Architektur

- Framework: Blazor WebAssembly Standalone PWA auf .NET 10.
- Hauptsprache: C#.
- Browsernahe APIs:
  - `wwwroot/js/webBluetooth.js` kapselt `navigator.bluetooth`.
  - `wwwroot/js/videoCamera.js` kapselt `navigator.mediaDevices.getUserMedia`, MP4-Testvideo-Dateiauswahl und Video-Zoom.
  - `wwwroot/js/audioSpy.js` erkennt Wortenden im Textfeld und begrenzt die Eingabe auf fünf Zeilen.
- Zustand:
  - `ApplicationStateStore` hält den reaktiven App-Zustand.
  - Komponenten rendern nach `StateChanged` neu.
- Persistenz:
  - `SettingsStorage` speichert Einstellungen in `localStorage` und erzeugt eingerückte Profil-JSON-Dateien.
- Styling:
  - `wwwroot/css/app.css` enthält das komplette App-Design. Bootstrap aus dem Template ist nicht aktiv eingebunden.

## Bluetooth

- Bluetooth-Name/Prefix: `BlePrompter`
- Service-UUID: `6e400001-b5a3-f393-e0a9-e50e24dcca9e`
- Receive/Write-Characteristic: `6e400002-b5a3-f393-e0a9-e50e24dcca9e`
- Transmit/Notify-Characteristic: `6e400003-b5a3-f393-e0a9-e50e24dcca9e`
- Befehle werden als UTF-8-Text ohne Zeilenumbruch geschrieben.
- Notifications werden als UTF-8 gelesen und im Log sowie als letzte Antwort angezeigt.

## Funktionsumfang

- Menü mit `Peeker (Videofake)`, `Einstellungen`, `Log` und `About`.
- Hilfesystem mit Ansichtshilfe, granularer Elementhilfe per `?`-Button und pflegbaren Markdown-Texten.
- In den Einstellungen kann das Hilfesystem direkt oben ausgeblendet werden; standardmäßig ist es eingeblendet.
- Testmodus ohne Bluetooth.
- Web-Bluetooth-Verbindung per Benutzeraktion.
- Reconnect für bereits freigegebene Geräte, wenn der Browser `navigator.bluetooth.getDevices` unterstützt.
- Kommandos für Pfeile, Karten, Text, ESP und Würfel.
- Audio Spy als Untermodus des Peeker: Spracherkennung über die mobile Tastatur, Mapping-Prüfung nach Wortende, sequenzielles Senden der gemappten Befehle und lokaler Verlauf.
- Audio-Spy-Mappings werden in den Einstellungen gepflegt, sind auf 100 begrenzt und unterstützen exakte Wörter, Wildcards `*`/`?` sowie Regex im Format `/.../`.
- Einstellungen können als Profil über den Browser-Download als `{Profilname}.peekclient.json` gespeichert und per Dateiauswahl wieder geladen werden; fehlerhafte Profile werden abgewiesen.
- `Anzeige löschen` sendet `CL` und optional `SLEEP DISPLAY`.
- Displayoptionen senden bei aktiver Verbindung `I0/I1` und `U0/U1`.
- Zyklischer Schlaf sendet `SLEEP CYCLE <Schlafdauer> <Listenzeit>`.
- Kamera-Vollbildansicht mit Livevideo oder sitzungsweise ausgewähltem MP4-Testvideo, REC-Anzeige, Pause, Zoom und ausblendbaren Overlay-Bedienelementen.
- Browser-/Hardware-Zurück zeigt einen zentralen Dialog mit `Zurück`, sofern die aktuelle Ansicht zurück kann, sowie `Abbrechen` und `App verlassen`. Die Zurück-Reihenfolge schließt zuerst Hilfe, Menü, lokale Dialoge/Subviews und Vollbild, danach Audio Spy oder Menüseiten zurück zum Peeker.
- In den Einstellungen kann als Videoquelle `Livekamera` oder `Testvideo` gewählt werden. Testvideos müssen MP4-Dateien sein, werden per Browser-Dateiauswahl nur für die aktuelle App-Sitzung registriert und nicht in das GitHub-Pages-Deployment übernommen.
- Testvideos werden immer per Zoom-to-Fill bildfüllend angezeigt; kleine Auflösungen werden hochskaliert und können im Testmodus unscharf wirken.

## Browsergrenzen

Web Bluetooth funktioniert nur in sicheren Kontexten wie HTTPS oder `localhost` und benötigt eine direkte Benutzeraktion für die Geräteauswahl. Android Chrome unterstützt diesen Ablauf. iOS Safari und Chrome für iOS unterstützen Web Bluetooth nicht nativ; iOS-Zielbrowser ist deshalb Bluefy.

Ein freier Hintergrundscan wie in `react-native-ble-plx` ist im Browser nicht verfügbar. Die App bildet die Suche über den Browser-Geräteauswahldialog ab. Die Funktion `Schlafendes Gerät suchen` ist deshalb ein browserkonformer Verbindungs-/Wake-Ablauf und kein nativer Hintergrundscan.

Lokale Testvideos werden über die Browser-Dateiauswahl freigegeben. Browser erlauben keinen dauerhaften freien Zugriff auf beliebige lokale Verzeichnisse; nach einem Neuladen oder PWA-Neustart muss die MP4-Datei erneut ausgewählt werden.

Profile werden über Browser-Download und Dateiauswahl importiert/exportiert. Ein garantierter direkter Ordnerzugriff ist browserbedingt nicht vorgesehen.

## GitHub Pages

Der Workflow `.github/workflows/deploy-github-pages.yml` baut die App mit:

```powershell
dotnet publish NasreddinsSimplePeekClient2Web.csproj -c Release -o publish
```

Danach wird der Blazor-`base href` auf den Repository-Pfad gesetzt, `index.html` als `404.html` für SPA-Fallback kopiert und `.nojekyll` im Publish-Root angelegt. GitHub Pages muss in den Repository-Einstellungen auf `GitHub Actions` als Quelle gestellt werden.

Der Workflow verwendet Node-24-kompatible Action-Versionen (`actions/checkout@v6`, `actions/setup-dotnet@v5`, `actions/configure-pages@v6`, `actions/upload-pages-artifact@v5`, `actions/deploy-pages@v5`). Ein `Get Pages site failed`/`Not Found` bei `actions/configure-pages` weist darauf hin, dass GitHub Pages im Repository noch nicht aktiviert oder nicht auf `GitHub Actions` als Quelle gestellt ist.

Das Repository wird nach Plan `Nasreddins-Simple-Peek-Client2Web` heißen. Falls der GitHub-Repositoryname abweicht, setzt der Workflow den Base-HREF dynamisch aus `GITHUB_REPOSITORY`.

## Lokale Befehle

```powershell
cd C:\dev\Nasreddins-Simple-Peek-Client2Web
```

```powershell
dotnet build
```

```powershell
dotnet publish -c Release
```

```powershell
dotnet run --no-launch-profile --urls http://127.0.0.1:5088
```

Lokale URL:

```text
http://127.0.0.1:5088
```

Für echte BLE-Tests muss die Anwendung über HTTPS oder `localhost` laufen. `127.0.0.1` ist für lokale Entwicklung ebenfalls ein sicherer Kontext.

## Bekannte Umgebungshinweise

- In einer Codex-Shell trat zeitweise ein `Path`/`PATH`-Duplikatproblem im Prozess-Environment auf. Dadurch konnten PowerShell-Befehle wie `Start-Process` oder `Get-ChildItem Env:` fehlschlagen. Das lag an der Prozessumgebung, nicht am Projekt.
- Der Benutzer konnte den Server danach in einem normalen Terminal ohne Probleme starten.
- Wenn Port `5088` blockiert ist, kann der blockierende Prozess geprüft werden mit:

```powershell
netstat -ano | Select-String -Pattern ':5088'
```

und ein versehentlich laufender `dotnet.exe` gezielt beendet werden.

## Projektregeln

- Alle sichtbaren Texte sind deutsch.
- Deutsche Umlaute werden verwendet.
- Datumsformat ist `DD.MM.YYYY`.
- Variablen- und Methodennamen vermeiden unnötige Abkürzungen.
