#!/usr/bin/env fish

set -g SCRIPT_DIR (cd (dirname (status filename)); and pwd)
set -g REPO_ROOT (cd "$SCRIPT_DIR/.."; and pwd)
set -g OUTPUT_DIR "$REPO_ROOT/dist/pacman"
set -g PKGNAME spotitoast
set -g PKGREL 1
set -g PKGVER
set -g INSTALL_PACKAGE true
set -g SYNC_DEPS true
set -g KEEP_WORKDIR false
set -g WORK_DIR
set -g CLEANUP_WORKDIR true

function usage
    printf '%s\n' \
        'Build and optionally install Spotitoast as an Arch Linux pacman package.' \
        '' \
        'Usage:' \
        '  ./scripts/install-arch-pacman.sh [options]' \
        '' \
        'Options:' \
        '  --pkgver <version>   Package version (default: derived from git describe)' \
        '  --pkgrel <release>   Package release number (default: 1)' \
        '  --pkgname <name>     Package name (default: spotitoast)' \
        '  --output <dir>       Output directory for built package (default: dist/pacman)' \
        '  --no-install         Build package only, do not install with pacman' \
        '  --no-syncdeps        Do not install missing make dependencies via makepkg' \
        '  --keep-workdir       Keep temporary packaging workdir for troubleshooting' \
        '  -h, --help           Show this help'
end

function die
    printf 'Error: %s\n' "$argv" >&2
    exit 1
end

function require_cmd
    set -l cmd "$argv[1]"
    if not command -sq "$cmd"
        die "Missing required command: $cmd"
    end
end

function cleanup --on-event fish_exit
    if test "$CLEANUP_WORKDIR" = true -a -n "$WORK_DIR"
        rm -rf "$WORK_DIR"
    end
end

function generate_pkgver
    set -l describe (git -C "$REPO_ROOT" describe --tags --long --always 2>/dev/null)
    if test $status -eq 0 -a -n "$describe"
        set describe (string replace -r '^v' '' -- "$describe")
        set describe (string replace -a -- '-' '.' "$describe")
        printf '%s\n' "$describe"
        return 0
    end

    set -l date_part (date +%Y%m%d)
    set -l rev_count 0
    set -l git_rev_count (git -C "$REPO_ROOT" rev-list --count HEAD 2>/dev/null)
    if test $status -eq 0 -a -n "$git_rev_count"
        set rev_count "$git_rev_count"
    end

    printf '0.0.0.%s.%s\n' "$date_part" "$rev_count"
end

while test (count $argv) -gt 0
    switch $argv[1]
        case --pkgver
            if test (count $argv) -lt 2
                die '--pkgver requires a value'
            end
            set PKGVER "$argv[2]"
            set -e argv[1..2]
        case --pkgrel
            if test (count $argv) -lt 2
                die '--pkgrel requires a value'
            end
            set PKGREL "$argv[2]"
            set -e argv[1..2]
        case --pkgname
            if test (count $argv) -lt 2
                die '--pkgname requires a value'
            end
            set PKGNAME "$argv[2]"
            set -e argv[1..2]
        case --output
            if test (count $argv) -lt 2
                die '--output requires a value'
            end
            set OUTPUT_DIR "$argv[2]"
            set -e argv[1..2]
        case --no-install
            set INSTALL_PACKAGE false
            set -e argv[1]
        case --no-syncdeps
            set SYNC_DEPS false
            set -e argv[1]
        case --keep-workdir
            set KEEP_WORKDIR true
            set CLEANUP_WORKDIR false
            set -e argv[1]
        case -h --help
            usage
            exit 0
        case '*'
            die "Unknown option: $argv[1]"
    end
end

if test -z "$PKGVER"
    set PKGVER (generate_pkgver)
end

if not string match -rq '^[A-Za-z0-9.+_]+$' -- "$PKGVER"
    die "Invalid --pkgver '$PKGVER'. Use only letters, numbers, '.', '+', and '_'"
end

if not string match -rq '^[0-9]+$' -- "$PKGREL"
    die "Invalid --pkgrel '$PKGREL'. Use a positive integer"
end

require_cmd git
require_cmd dotnet
require_cmd makepkg
if test "$INSTALL_PACKAGE" = true -o "$SYNC_DEPS" = true
    require_cmd sudo
end

if not test -f "$REPO_ROOT/Spotitoast.Linux/Spotitoast.Linux.csproj"
    die 'Run this script from inside the Spotitoast repository'
end

set -l tmp_base /tmp
if set -q TMPDIR
    set tmp_base "$TMPDIR"
end

set WORK_DIR (mktemp -d "$tmp_base/spotitoast-pkg.XXXXXX")
or die 'Unable to create temporary work directory'

mkdir -p "$OUTPUT_DIR"
or die "Unable to create output directory: $OUTPUT_DIR"

printf '==> Using packaging workdir: %s\n' "$WORK_DIR"
printf '==> Package: %s %s-%s\n' "$PKGNAME" "$PKGVER" "$PKGREL"

printf '==> Creating source tarball\n'
git -C "$REPO_ROOT" archive --format=tar.gz --prefix=spotitoast/ HEAD >"$WORK_DIR/spotitoast.tar.gz"
or die 'Failed to create source tarball'

begin
    printf '%s\n' \
        '[Unit]' \
        'Description=Spotitoast user background service' \
        'After=graphical-session.target network-online.target' \
        'Wants=graphical-session.target network-online.target' \
        'PartOf=graphical-session.target' \
        '' \
        '[Service]' \
        'Type=simple' \
        'WorkingDirectory=/opt/spotitoast' \
        'ExecStart=/opt/spotitoast/Spotitoast.Linux' \
        'Environment=XDG_DATA_DIRS=/usr/local/share:/usr/share' \
        'Restart=on-failure' \
        'RestartSec=2' \
        '' \
        '[Install]' \
        'WantedBy=default.target'
end >"$WORK_DIR/spotitoast.service"
or die 'Failed to write systemd service file'

begin
    printf '%s\n' \
        '[Desktop Entry]' \
        'Type=Application' \
        'Name=Spotitoast' \
        'Comment=Control Spotify and notifications from your desktop' \
        'Exec=/opt/spotitoast/Spotitoast.Linux' \
        'Icon=Spotitoast' \
        'Terminal=false' \
        'Categories=AudioVideo;Player;' \
        'Actions=TogglePlayback;Like;Dislike;Skip;CurrentlyPlaying;' \
        '' \
        '[Desktop Action TogglePlayback]' \
        'Name=Toggle Playback' \
        'Exec=/opt/spotitoast/Spotitoast.Linux TogglePlayback' \
        'X-KDE-Shortcuts=Ctrl+Home' \
        '' \
        '[Desktop Action Like]' \
        'Name=Like Current Track' \
        'Exec=/opt/spotitoast/Spotitoast.Linux Like' \
        'X-KDE-Shortcuts=Ctrl+PgUp' \
        '' \
        '[Desktop Action Dislike]' \
        'Name=Dislike Current Track' \
        'Exec=/opt/spotitoast/Spotitoast.Linux Dislike' \
        'X-KDE-Shortcuts=Ctrl+PgDown' \
        '' \
        '[Desktop Action Skip]' \
        'Name=Skip Track' \
        'Exec=/opt/spotitoast/Spotitoast.Linux Skip' \
        'X-KDE-Shortcuts=Ctrl+Right' \
        '' \
        '[Desktop Action CurrentlyPlaying]' \
        'Name=Show Current Track' \
        'Exec=/opt/spotitoast/Spotitoast.Linux CurrentlyPlaying' \
        'X-KDE-Shortcuts=Ctrl+End'
end >"$WORK_DIR/spotitoast.desktop"
or die 'Failed to write desktop entry'

begin
    echo "pkgname=$PKGNAME"
    echo "pkgver=$PKGVER"
    echo "pkgrel=$PKGREL"
    echo "pkgdesc='Spotitoast Linux background Spotify controller with notification actions'"
    echo "arch=('x86_64')"
    echo "url='https://github.com/Belphemur/Spotitoast'"
    echo "license=('custom')"
    echo "depends=('glibc')"
    echo "makedepends=('dotnet-sdk')"
    echo "optdepends=('plasma-workspace: KDE shortcut integration for desktop actions')"
    echo "options=('!strip')"
    echo "source=('spotitoast.tar.gz' 'spotitoast.service' 'spotitoast.desktop')"
    echo "sha256sums=('SKIP' 'SKIP' 'SKIP')"
    echo
    echo 'build() {'
    echo '  cd "$srcdir/spotitoast"'
    echo
    echo "  dotnet publish Spotitoast.Linux/Spotitoast.Linux.csproj \\" 
    echo "    -c Release \\" 
    echo "    -r linux-x64 \\" 
    echo "    --self-contained false \\" 
    echo "    -p:DebugType=embedded \\" 
    echo '    -o "$srcdir/publish"'
    echo '}'
    echo
    echo 'package() {'
    echo '  cd "$srcdir"'
    echo
    echo '  mkdir -p "$pkgdir/opt/spotitoast"'
    echo '  cp -a publish/. "$pkgdir/opt/spotitoast/"'
    echo
    echo '  # Preserve desktop autostart compatibility and add KDE actions in system menu metadata.'
    echo '  install -Dm644 spotitoast/Spotitoast.Linux/Resources/Spotitoast.desktop "$pkgdir/opt/spotitoast/Spotitoast.desktop"'
    echo '  install -Dm644 spotitoast.desktop "$pkgdir/usr/share/applications/Spotitoast.desktop"'
    echo '  install -Dm644 spotitoast/Spotitoast.Linux/Resources/Spotitoast.svg "$pkgdir/usr/share/icons/hicolor/scalable/apps/Spotitoast.svg"'
    echo
    echo '  install -Dm644 spotitoast.service "$pkgdir/usr/lib/systemd/user/spotitoast.service"'
    echo '}'
end >"$WORK_DIR/PKGBUILD"
or die 'Failed to write PKGBUILD'

set -l makepkg_args --force --clean
if test "$SYNC_DEPS" = true
    set -a makepkg_args --syncdeps --noconfirm
end

printf '==> Building pacman package with makepkg\n'
pushd "$WORK_DIR" >/dev/null
or die "Unable to enter packaging workdir: $WORK_DIR"
makepkg $makepkg_args
or die 'makepkg failed'
popd >/dev/null

set -l package_candidates (find "$WORK_DIR" -maxdepth 1 -type f -name "$PKGNAME-*.pkg.tar.*" | sort)
if test (count $package_candidates) -eq 0
    die 'Package build finished, but no package file was found'
end

set -l PACKAGE_FILE $package_candidates[-1]
cp -f "$PACKAGE_FILE" "$OUTPUT_DIR/"
or die 'Failed to copy built package to output directory'

set -l OUTPUT_PACKAGE "$OUTPUT_DIR/"(basename "$PACKAGE_FILE")
printf '==> Package ready: %s\n' "$OUTPUT_PACKAGE"

if test "$INSTALL_PACKAGE" = true
    printf '==> Installing package with pacman\n'
    sudo pacman -U --noconfirm "$OUTPUT_PACKAGE"
    or die 'pacman package installation failed'

    printf '==> Importing desktop session environment into user systemd\n'
    if command -sq dbus-update-activation-environment
        dbus-update-activation-environment --systemd DISPLAY WAYLAND_DISPLAY XAUTHORITY DBUS_SESSION_BUS_ADDRESS XDG_CURRENT_DESKTOP XDG_RUNTIME_DIR XDG_SESSION_TYPE >/dev/null 2>&1
    end
    systemctl --user import-environment DISPLAY WAYLAND_DISPLAY XAUTHORITY DBUS_SESSION_BUS_ADDRESS XDG_CURRENT_DESKTOP XDG_RUNTIME_DIR XDG_SESSION_TYPE

    printf '==> Enabling Spotitoast user service\n'
    systemctl --user daemon-reload
    or die 'systemctl --user daemon-reload failed'
    systemctl --user enable spotitoast.service >/dev/null
    systemctl --user restart spotitoast.service
    or die 'Failed to enable Spotitoast user service'

    printf '==> Refreshing KDE desktop metadata\n'
    if command -sq kbuildsycoca6
        kbuildsycoca6 >/dev/null 2>&1
    else if command -sq kbuildsycoca5
        kbuildsycoca5 >/dev/null 2>&1
    end

    if command -sq qdbus6
        qdbus6 org.kde.kglobalaccel /kglobalaccel org.kde.KGlobalAccel.reloadConfig >/dev/null 2>&1
    else if command -sq qdbus
        qdbus org.kde.kglobalaccel /kglobalaccel org.kde.KGlobalAccel.reloadConfig >/dev/null 2>&1
    end

    printf '%s\n' '==> Installed. Verify shortcuts in: System Settings -> Shortcuts -> Spotitoast'
else
    printf '%s\n' '==> Build-only mode: package not installed'
end

if test "$KEEP_WORKDIR" = true
    printf '==> Workdir preserved at: %s\n' "$WORK_DIR"
end
