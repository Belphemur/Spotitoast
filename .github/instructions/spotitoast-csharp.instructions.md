---
description: "Use when editing Spotitoast C# source files, adding services, hotkeys, notifications, Spotify integrations, or dependency bindings. Covers project boundaries, Ninject wiring, platform separation, and existing C# naming/style conventions."
name: "Spotitoast C# Conventions"
applyTo: "**/*.cs"
---
# Spotitoast C# Conventions

- Treat these conventions as the default for this repository. Only deviate when the task explicitly requires it, and keep the deviation narrow and local to that change.
- Keep changes inside the existing project boundaries. Put Spotify API code in `Spotitoast.Spotify`, cross-cutting business logic in `Spotitoast.Logic`, configuration persistence in `Spotitoast.Configuration`, hotkey code in `Spotitoast.HotKeys`, Windows UI code in `Spotitoast` or `Spotitoast.Banner`, and Linux-specific notification code in `Spotitoast.Linux` or `Notify.Linux`.
- Extend dependency injection through the existing Ninject setup. Prefer adding or updating modules and bootstrap wiring instead of introducing a new container or ad hoc service locators.
- Follow the repository's existing C# style: block-scoped namespaces, Allman braces, PascalCase for public members and types, and `_camelCase` for private fields.
- Match the existing async patterns. Use `Task`-based async code in library and service layers, and only bridge async work synchronously in application entry points or framework-required boundaries.
- Preserve the current interface and feature-folder patterns. Keep `I`-prefixed interfaces near their related implementations and avoid collapsing platform-specific code into shared projects.
- Prefer incremental changes that fit the current architecture over broad refactors. Do not replace Ninject, rename major folders, or merge Linux and Windows notification paths unless the task explicitly requires it.