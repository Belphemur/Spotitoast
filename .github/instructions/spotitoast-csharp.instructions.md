---
description: "Use when editing Spotitoast C# source files, adding services, hotkeys, notifications, Spotify integrations, or dependency bindings. Covers project boundaries, DI wiring, platform separation, systemd integration, CLI client, and existing C# naming/style conventions."
name: "Spotitoast C# Conventions"
applyTo: "**/*.cs"
---
# Spotitoast C# Conventions

- Treat these conventions as the default for this repository. Only deviate when the task explicitly requires it, and keep the deviation narrow and local to that change.
- Keep changes inside the existing project boundaries. Put Spotify API code in `Spotitoast.Spotify`, cross-cutting business logic in `Spotitoast.Logic`, configuration persistence in `Spotitoast.Configuration`, hotkey code in `Spotitoast.HotKeys`, Windows UI code in `Spotitoast` or `Spotitoast.Banner`, Linux server code in `Spotitoast.Linux.Server`, Linux CLI client code in `Spotitoast.Linux.Client`, D-Bus notification helpers in `Notify.Linux`, and shared IPC types in `Spotitoast.Shared`.

## Architecture

- The Linux deployment follows a **server / CLI client** split:
  - **`Spotitoast.Linux.Server`** — Long-running `Host`-based background service. Listens on a named pipe, manages Spotify polling, and sends D-Bus notifications. Launched via systemd (`Type=notify`).
  - **`Spotitoast.Linux.Client`** — Lightweight Spectre.Console CLI (`spotitoast`). Connects to the server over the named pipe and forwards a single command. Used by the `.desktop` file actions and by users directly.
  - **`Spotitoast.Shared`** — Contains `IpcConstants` (pipe name, mutex name), `IpcClient` (named-pipe helper), and `PlayerCommand` enum. Referenced by both server and client.
- The named pipe is derived from the current username via `IpcConstants.PipeName` in `Spotitoast.Shared`.

## Dependency Injection

- The repository uses `Microsoft.Extensions.DependencyInjection` (MEDI) for all DI wiring. The older Ninject container has been fully removed.
- Core service registrations live in `Spotitoast.Logic/Dependencies/Bootstrap.cs` via the `AddSpotitoastCore()` extension method on `IServiceCollection`.
- Linux-specific registrations live in `Spotitoast.Linux.Server/Bootstrap/BootstrapLinuxModule.cs` via `AddSpotitoastLinux()`.
- When adding a new service, register it in the appropriate `IServiceCollection` extension method rather than resolving it manually or using service locators.
- The `EquatableFactory<TKey, TImplementation>` pattern resolves all `IAction` implementations via constructor-injected `IEnumerable<IAction>`, building a key-based dictionary. Register new actions as `services.AddSingleton<IAction, YourAction>()` in `AddSpotitoastCore()`.

## HTTP Clients

- Use `IHttpClientFactory` (registered via `services.AddHttpClient(...)`) for all outbound HTTP calls. Do not create static or long-lived `HttpClient` instances.
- The `ImageDownloader` service in `Spotitoast.Logic/Framework/Extensions/ImageDownloader.cs` demonstrates the pattern: it receives `IHttpClientFactory` and `IMemoryCache` via constructor injection.

## Linux & systemd

- The Linux server uses the .NET Generic Host (`Host.CreateApplicationBuilder`) with `AddSystemd()` for full systemd integration.
- `Spotitoast.Linux.Server/Hosting/SpotitoastService.cs` is a `BackgroundService` that runs the named-pipe server event loop and stops the job scheduler on shutdown.
- `Spotitoast.Linux.Server/Hosting/SystemdStatusReporter.cs` subscribes to `ISpotifyNotifier` track events and pushes `STATUS=` updates to systemd via `ISystemdNotifier`.
- The systemd unit file (`Spotitoast.Linux.Server/Resources/spotitoast.service`) uses `Type=notify` so the host sends `READY=1` once all hosted services have started, `STOPPING=1` on shutdown, and periodic `WATCHDOG=1` heartbeats.

## CLI Client

- The CLI is built with `Spectre.Console.Cli` and produces a binary called `spotitoast`.
- It provides shorthand commands (`like`, `dislike`, `toggle`, `skip`, `now`, `exit`) as well as a generic `send <Command>` and a `status` command.
- The `.desktop` file actions invoke the CLI (e.g. `Exec=/opt/spotitoast/spotitoast like`).

## Style

- Follow the repository's existing C# style: block-scoped namespaces, Allman braces, PascalCase for public members and types, and `_camelCase` for private fields.
- Match the existing async patterns. Use `Task`-based async code in library and service layers, and only bridge async work synchronously in application entry points or framework-required boundaries.
- Preserve the current interface and feature-folder patterns. Keep `I`-prefixed interfaces near their related implementations and avoid collapsing platform-specific code into shared projects.