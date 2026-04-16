# Spotitoast

A lightweight desktop application that monitors Spotify playback and provides desktop notifications and global hotkey controls for Spotify management.

## Features

### 🎵 Smart Notifications
- **Desktop Banner**: Displays a sleek notification banner when tracks play, showing track name, artist, album art, and release year
- **System Tray Alerts**: Shows popup notifications when you like 💖 or dislike 🖤 tracks
- **Real-time Tracking**: Continuously monitors Spotify for track changes and automatically displays notifications

### ⌨️ Global Hotkeys
Control Spotify from anywhere on your desktop without switching windows:
- **`Ctrl+Home`** - Play / Pause
- **`Ctrl+PageUp`** - Like current track ❤️
- **`Ctrl+PageDown`** - Dislike current track
- **`Ctrl+End`** - Show currently playing track

All hotkeys are fully remappable through configuration.

### 🔗 Spotify Integration
- Seamless integration with Spotify Web API
- OAuth token authentication with automatic refresh
- Sync likes and dislikes back to your Spotify library
- Monitor playback status in real-time

## Supported Platforms

- **Windows**: Full-featured with WinForms UI and system tray integration
- **Linux**: Server/CLI architecture with D-Bus notifications and systemd integration

## Installation

### Arch Linux

Spotitoast provides an automated build and installation script for Arch Linux using `makepkg` and `pacman`:

#### Prerequisites
```bash
pacman -S base-devel git dotnet-sdk
```

#### Build and Install
```bash
cd /path/to/Spotitoast
./scripts/install-arch-pacman.sh
```

The script will:
- Build the Arch Linux pacman package
- Install missing build dependencies automatically
- Install the built package via pacman

#### Build Options
The install script supports several options:

```bash
./scripts/install-arch-pacman.sh [options]

Options:
  --pkgver <version>   Package version (default: auto-detected from git)
  --pkgrel <release>   Package release number (default: 1)
  --no-install         Build package only, do not install
  --no-syncdeps        Do not auto-install build dependencies
  --keep-workdir       Keep temporary build directory for debugging
  -h, --help           Show help message
```

Example: Build without installing
```bash
./scripts/install-arch-pacman.sh --no-install
```

### Other Platforms

#### Build from Source
Requirements:
- .NET 10 SDK or later
- Git

```bash
git clone https://github.com/Belphemur/Spotitoast.git
cd Spotitoast
dotnet build -c Release
```

Run on Windows:
```bash
dotnet run --project Spotitoast/Spotitoast.csproj
```

Run on Linux (server):
```bash
dotnet run --project Spotitoast.Linux.Server/Spotitoast.Linux.Server.csproj
```

Run on Linux (CLI client):
```bash
dotnet run --project Spotitoast.Linux.Client/Spotitoast.Linux.Client.csproj -- like
```

## Configuration

Spotitoast stores its configuration in a JSON file. Key settings include:

- **Spotify API Credentials**: OAuth tokens and client ID
- **Hotkey Bindings**: Customize keyboard shortcuts to your preference
- **Notification Preferences**: Control notification behavior
- **Polling Interval**: Adjust how frequently Spotitoast checks for track changes

Configuration is typically stored in:
- **Windows**: `%AppData%/Spotitoast/`
- **Linux**: `~/.config/spotitoast/`

## Architecture

Spotitoast is built with a modular, platform-aware architecture:

### Projects

| Project | Purpose |
|---------|---------|
| **Spotitoast.Spotify** | Spotify Web API client with OAuth authentication and token refresh |
| **Spotitoast.Logic** | Core business logic — track control, notifications, image downloading |
| **Spotitoast.Configuration** | Configuration persistence and management |
| **Spotitoast.HotKeys** | Global hotkey registration and handling |
| **Spotitoast** | Windows WinForms UI with system tray integration |
| **Spotitoast.Banner** | Windows custom notification banner display |
| **Spotitoast.Linux.Server** | Linux long-running background service (systemd `Type=notify`) |
| **Spotitoast.Linux.Client** | Linux lightweight CLI client (`spotitoast`) |
| **Spotitoast.Shared** | Shared IPC types, constants, and named-pipe helpers |
| **Notify.Linux** | D-Bus integration for Linux desktop notifications |

### Linux: Server / CLI Client Split

On Linux, Spotitoast uses a **server + CLI client** architecture communicating over a named pipe:

```
┌──────────────────────┐         named pipe          ┌─────────────────────┐
│  Spotitoast Server   │◄───────────────────────────►│  spotitoast CLI     │
│  (background service)│   "Like" → "Success"        │  (one-shot command) │
│                      │                              │                     │
│  • Spotify polling   │                              │  spotitoast like    │
│  • D-Bus notifs      │                              │  spotitoast skip    │
│  • Library mgmt      │                              │  spotitoast toggle  │
│  • systemd notify    │                              │  spotitoast now     │
└──────────────────────┘                              └─────────────────────┘
```

**Server** (`Spotitoast.Linux.Server`) — A .NET Generic Host background service that:
- Polls Spotify for track changes via `Job.Scheduler`
- Sends D-Bus desktop notifications on track change, like, and dislike
- Listens on a per-user named pipe (`spotitoast-{username}`) for commands
- Integrates with systemd: sends `READY=1` on start, `STOPPING=1` on shutdown, `WATCHDOG=1` heartbeats, and live `STATUS=Playing: Song — Artist` updates
- Uses a mutex (`Global\Spotitoast-{username}`) to prevent duplicate instances

**CLI Client** (`Spotitoast.Linux.Client`) — A Spectre.Console CLI that connects to the server pipe and sends a single command:

| Command | Action |
|---------|--------|
| `spotitoast like` | Like current track |
| `spotitoast dislike` | Dislike and skip current track |
| `spotitoast toggle` | Toggle play/pause |
| `spotitoast skip` | Skip to next track |
| `spotitoast now` | Show currently playing track |
| `spotitoast exit` | Stop the server |
| `spotitoast status` | Check if the server is running |
| `spotitoast send <Cmd>` | Send any `PlayerCommand` by name |

The `.desktop` file actions invoke the CLI (e.g. `Exec=/opt/spotitoast/spotitoast like`).

**IPC Protocol** — The client sends a `PlayerCommand` enum name as an ASCII string over the named pipe. The server parses it, executes the action, and responds with an `ActionResult` enum name.

### Key Libraries

- **Reactive Extensions (Rx)** for event-driven track/like/dislike streams
- **Microsoft.Extensions.DependencyInjection** for DI wiring
- **Job.Scheduler** for periodic Spotify polling
- **IHttpClientFactory** for HTTP client management
- **.NET Generic Host** with **systemd** integration on Linux
- **Spectre.Console.Cli** for the Linux CLI client

## Building for Development

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests (if available)
dotnet test

# Build release
dotnet build -c Release
```

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Community & Support

For issues, feature requests, or contributions, please visit the [GitHub repository](https://github.com/Belphemur/Spotitoast).

Contributions are welcome! Whether you're fixing bugs, adding features, or improving documentation, we appreciate your help in making Spotitoast better.
