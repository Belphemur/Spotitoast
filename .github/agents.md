# Spotitoast — Agent Guide

## Overview

Spotitoast is a cross-platform desktop application that monitors Spotify playback and provides desktop notifications, global hotkey controls, and live systemd status reporting. It runs on **Windows** (WinForms tray app) and **Linux** (systemd-integrated daemon with D-Bus notifications).

## Repository layout

```
Spotitoast.sln                     # Visual Studio solution (all projects)
├── Spotitoast.Spotify/            # Spotify Web API client, OAuth, models
├── Spotitoast.Configuration/      # JSON-based configuration persistence
├── Spotitoast.Logic/              # Core business logic (shared)
│   ├── Business/
│   │   ├── Action/                # IAction implementations (Like, Dislike, SkipTrack, …)
│   │   ├── Command/               # Command execution pipeline
│   │   └── Player/                # ISpotifyNotifier, SpotifyNotifier, track adapters
│   ├── Dependencies/
│   │   └── Bootstrap.cs           # AddSpotitoastCore() — central DI registrations
│   ├── Framework/
│   │   └── Extensions/
│   │       └── ImageDownloader.cs # IHttpClientFactory + IMemoryCache image fetcher
│   └── Model/                     # Domain models (ITrack, IAlbum, …)
├── Spotitoast/                    # Windows WinForms entry point (net10.0-windows)
├── Spotitoast.Banner/             # Windows notification banner UI (net10.0-windows)
├── Spotitoast.Hotkeys/            # Global hotkey handling (net8.0-windows)
├── Spotitoast.Linux/              # Linux entry point + systemd integration (net10.0)
│   ├── Bootstrap/
│   │   └── BootstrapLinuxModule.cs   # AddSpotitoastLinux() — Linux DI registrations
│   ├── Context/
│   │   ├── ServerContext.cs       # TCP server (first instance)
│   │   └── ClientContext.cs       # TCP client (subsequent instances)
│   ├── Hosting/
│   │   ├── SpotitoastService.cs   # BackgroundService wrapping TCP event loop
│   │   └── SystemdStatusReporter.cs  # STATUS= updates to systemd
│   ├── Notification/              # D-Bus notification handler
│   └── Resources/
│       └── spotitoast.service     # systemd unit file (Type=notify)
├── Notify.Linux/                  # Low-level D-Bus notification client (net10.0)
└── scripts/
    └── install-arch-pacman.sh     # Arch Linux build/install script
```

## Architecture

### Dependency injection

All DI wiring uses `Microsoft.Extensions.DependencyInjection` (MEDI).

- **Core services** are registered via `Spotitoast.Logic/Dependencies/Bootstrap.cs` → `AddSpotitoastCore()` extension method on `IServiceCollection`. This includes configuration, Spotify client, actions, the job scheduler, `IHttpClientFactory`, `IMemoryCache`, and `ImageDownloader`.
- **Linux services** are registered via `Spotitoast.Linux/Bootstrap/BootstrapLinuxModule.cs` → `AddSpotitoastLinux()`. This adds D-Bus notification support.
- **Windows services** are composed directly in `Spotitoast/Program.cs` using `ServiceCollection` + `ServiceProvider`.

When adding a new service, register it in the appropriate `IServiceCollection` extension method.

### Actions and the factory pattern

User-triggerable commands (Like, Dislike, SkipTrack, TogglePlayback, CurrentlyPlaying, Exit) are registered as `IAction` singletons. `ActionFactory` receives `IEnumerable<IAction>` via MEDI multi-registration and builds a keyed dictionary. Register new actions with:

```csharp
services.AddSingleton<IAction, YourAction>();
```

### HTTP clients

Outbound HTTP calls use `IHttpClientFactory` (registered via `services.AddHttpClient(...)`). The `ImageDownloader` service demonstrates this pattern with an `IMemoryCache` layer for album art.

### Linux systemd integration

The Linux app uses the .NET Generic Host (`Host.CreateApplicationBuilder`) with `AddSystemd()`:

- **`SpotitoastService`** (`BackgroundService`): runs the TCP server event loop; wires notification subscriptions before `READY=1`; triggers host shutdown on exit.
- **`SystemdStatusReporter`** (`IHostedService`): subscribes to `ISpotifyNotifier` Rx streams and pushes `STATUS=Playing: Song — Artist` to systemd via `ISystemdNotifier`.
- The systemd unit file uses `Type=notify`, `WatchdogSec=60`, and `Restart=on-failure`.
- SIGTERM handling, `READY=1`, `STOPPING=1`, and watchdog heartbeats are provided automatically by the Generic Host.

### Linux server/client architecture

The Linux app uses a mutex-based single-instance pattern:

1. The first instance acquires a named mutex, starts as a **TCP server**, and listens for commands.
2. Subsequent instances detect the mutex is held, connect as **TCP clients**, send a command, and exit immediately.
3. The port is deterministically derived from the username so each user gets a unique port.

### Event-driven streams

`ISpotifyNotifier` exposes `IObservable<ITrack>` streams (`TrackPlayed`, `TrackLiked`, `TrackDisliked`) using Reactive Extensions. UI layers and the systemd status reporter subscribe to these streams.

## Build & validate

### Prerequisites

- .NET 10 SDK (preview)

### Build Linux project (on Linux or CI)

```bash
dotnet build Spotitoast.Linux/Spotitoast.Linux.csproj -c Release
```

### Build full solution (on Windows)

```bash
dotnet build Spotitoast.sln -c Release
```

The full solution includes Windows-only projects (`net10.0-windows`, `net8.0-windows`) that require the Windows SDK. On Linux, build individual cross-platform projects instead.

### Publish

```bash
# Linux
dotnet publish Spotitoast.Linux/Spotitoast.Linux.csproj -c Release -o publish/linux

# Windows
dotnet publish Spotitoast/Spotitoast.csproj -c Release -o publish/windows
```

## CI/CD

The repository uses GitHub Actions (`.github/workflows/build.yml`):

- **build-linux**: builds and publishes `Spotitoast.Linux` on `ubuntu-latest`
- **build-windows**: builds the full solution and publishes the Windows app on `windows-latest`

Both jobs use .NET 10 preview SDK.

## Conventions

- See `.github/instructions/spotitoast-csharp.instructions.md` for detailed C# coding conventions.
- Block-scoped namespaces, Allman braces, PascalCase for public members, `_camelCase` for private fields.
- Keep changes within existing project boundaries. Don't merge platform-specific code into shared projects.
- Prefer incremental changes over broad refactors.
