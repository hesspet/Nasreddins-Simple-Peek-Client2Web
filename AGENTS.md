# AGENTS.md

## Localization & Int.

- Alle User-facing Strings müssen lokalisiert oder zentral über `Resources/GermanText.cs` beziehungsweise Ressourcen geführt werden.
- Benutze deutsche Umlaute.
- Datumsformate müssen der EU-Norm entsprechen: `DD.MM.YYYY`.

## Project Context

- Dieses Projekt muss die Datei `PROJEKTUEBERSICHT.md` behalten.
- Vor Arbeiten am Projekt zuerst `PROJEKTUEBERSICHT.md` lesen.
- Das Quellprojekt für die Portierung liegt unter `C:\dev\Nasreddins-Simple-Peek-Client2`.

## Code Style

- Vermeide Abkürzungen in Variablennamen und Methodennamen.
- C# ist die Hauptsprache.
- Browser-APIs nur über klar gekapselte JavaScript-Interop-Dateien in `wwwroot/js`.
- UI-Zustand zentral über `ApplicationStateStore` führen.

## Lokale Entwicklung

- Standardstart:

```powershell
dotnet run --no-launch-profile --urls http://127.0.0.1:5088
```

- Buildprüfung:

```powershell
dotnet build
dotnet publish -c Release
```

## GitHub Pages

- Deployment läuft über `.github/workflows/deploy-github-pages.yml`.
- GitHub Pages muss als Quelle `GitHub Actions` verwenden.
- `.nojekyll` ist notwendig, damit `_framework` ausgeliefert wird.
