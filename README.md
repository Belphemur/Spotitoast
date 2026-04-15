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
- **Linux**: Command-line interface with D-Bus notifications

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

Run on Linux:
```bash
dotnet run --project Spotitoast.Linux/Spotitoast.Linux.csproj
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

- **Spotitoast.Spotify**: Spotify Web API client with authentication
- **Spotitoast.Logic**: Core business logic for track control and notifications
- **Spotitoast.Configuration**: Configuration persistence and management
- **Spotitoast.HotKeys**: Global hotkey registration and handling
- **Spotitoast** (Windows): WinForms UI with system tray integration
- **Spotitoast.Banner** (Windows): Custom notification banner display
- **Spotitoast.Linux**: Linux daemon and CLI interface
- **Notify.Linux**: D-Bus integration for Linux notifications

The application uses:
- **Reactive Extensions (Rx)** for event-driven architecture
- **Microsoft.Extensions.DependencyInjection** for dependency injection
- **Job.Scheduler** for periodic polling tasks
- **IHttpClientFactory** for HTTP client management
- **.NET Generic Host** with **systemd** integration on Linux

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
