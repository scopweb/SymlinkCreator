# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Symlink Creator** is a modern WPF desktop application for creating Windows symlinks (symbolic links) via the `mklink` command. This is a fork with added configuration support for default paths and a professional dark mode UI.

- **Framework**: .NET 10 (`net10.0-windows` SDK-style)
- **UI**: WPF with professional dark theme (Catppuccin Mocha palette)
- **Build System**: .NET CLI (`dotnet` commands)
- **Testing**: MSTest unit tests
- **Dependencies**: Minimal (System.Configuration.ConfigurationManager for App.config support)

## Architecture

### Folder Structure
- **`SymlinkCreator/`**: Main WPF application
  - `core/`: Business logic
    - `SymlinkAgent.cs`: Creates symlinks and manages script execution
    - `ScriptExecutor.cs`: Executes batch scripts with admin privileges
    - `ApplicationConfiguration.cs`: Loads config from App.config
  - `ui/`: User interface (WPF/XAML)
    - `mainWindow/`: Main application window with MVVM ViewModel
    - `aboutWindow/`: About dialog with fork attribution
    - `themes/`: Dark theme resources
      - `DarkTheme.xaml`: ResourceDictionary with Catppuccin Mocha palette (13 colors, 10+ styled controls)
    - `utility/`: UI helpers (admin shield icon, path handling, long path support)
  - `resources/`: Icons and media
  - `App.config`: Configuration file with `DefaultSourceFolderPath` and `DefaultDestinationFolderPath` settings
  - `app.manifest`: Requires admin privileges for symlink creation
  - `SymlinkCreator.csproj`: SDK-style project file for .NET 10

- **`SymlinkCreatorTests/`**: Unit tests
  - Uses MSTest framework
  - Tests for symlink creation logic

- **Root Solution**: `SymlinkCreator/SymlinkCreator.sln`

### Design Pattern

The app uses **MVVM (Model-View-ViewModel)**:
- `MainWindowViewModel.cs`: Handles UI state and properties
- `MainWindow.xaml/cs`: View bindings
- `SymlinkAgent`: Core business logic for symlink creation

## Building and Running

### Build
```bash
# Build Debug
dotnet build SymlinkCreator/SymlinkCreator.sln

# Build Release
dotnet build SymlinkCreator/SymlinkCreator.sln -c Release

# Output: SymlinkCreator/bin/Debug(Release)/SymlinkCreator.exe
```

### Run
The application requires **administrator privileges** (enforced by `app.manifest`).

```bash
# Run the built executable directly (will prompt for admin)
./SymlinkCreator/bin/Debug/SymlinkCreator.exe

# Or from the project root after building
dotnet run --project SymlinkCreator/SymlinkCreator.csproj
```

### Tests
```bash
# Run all tests
dotnet test SymlinkCreator/SymlinkCreator.sln

# Run with verbose output
dotnet test SymlinkCreator/SymlinkCreator.sln -v normal
```

### Self-Contained Distribution
To generate a standalone EXE without .NET runtime dependency:
```bash
dotnet publish SymlinkCreator/SymlinkCreator.csproj \
  -c Release -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true

# Output: SymlinkCreator/bin/Release/net10.0-windows/win-x64/publish/SymlinkCreator.exe
```

## Key Implementation Details

### Symlink Creation Process
1. **SymlinkAgent** collects source files/folders and destination path
2. **ScriptExecutor** generates a batch script with `mklink` commands
3. Script is executed with elevated privileges (admin mode)
4. Results are returned to the UI

### Dark Theme System
The UI uses a **ResourceDictionary-based theme** (DarkTheme.xaml) with the **Catppuccin Mocha** color palette:
- **AppBackground**: `#1E1E2E` (main window background)
- **AppSurface**: `#181825` (input fields, surface elements)
- **AppBorder**: `#45475A` (borders, dividers)
- **AppText**: `#CDD6F4` (primary text)
- **AppSubtext**: `#BAC2DE` (secondary text, labels)
- **AppAccent**: `#89B4FA` (buttons, highlights)
- **AppGreen**, **AppRed**, **AppYellow**: Status indicators

All controls (Button, TextBox, CheckBox, ListView, etc.) inherit from this palette via DynamicResource bindings. Modify `ui/themes/DarkTheme.xaml` to adjust colors globally.

### Configuration
Settings are stored in `App.config`:
```xml
<add key="DefaultSourceFolderPath" value="C:\MCPs\SKILLS" />
<add key="DefaultDestinationFolderPath" value="C:\MCPs\clone" />
```
Load via `ApplicationConfiguration.cs` at startup.

### Windows-Specific Features
- **Long Path Support**: Uses `longPathAware` registry setting (Windows 10+)
- **Unicode Support**: Handles Unicode folder names correctly
- **Relative Paths**: Option to use relative symlinks if both source/dest are on same drive
- **Admin Shield Icon**: Visual indicator for admin-required operations
- **Modern File Dialog**: Uses native `Microsoft.Win32.OpenFolderDialog` (no WindowsAPICodePack dependency)

## Important Notes

### Never Modify
- Do not change the target framework without updating all project references (currently `net10.0-windows`)
- Do not remove the `app.manifest` - it's required for admin privileges
- Do not bypass admin checks
- Do not modify `DarkTheme.xaml` colors without ensuring sufficient contrast (WCAG AA minimum)

### When Adding Features
- **Keep MVVM pattern**: Logic in ViewModel/Agent, UI in XAML
- **Use DynamicResource bindings**: Reference colors from `DarkTheme.xaml` (e.g., `Foreground="{DynamicResource AppTextBrush}"`)
- **Test both scenarios**: Relative and absolute path scenarios
- **Windows compatibility**: Handle paths with spaces, Unicode, and long paths (use `LongPathAware.cs` utilities)
- **Config changes**: Update `App.config` for any new configurable settings
- **Tests**: Add tests to `SymlinkCreatorTests/` for new core logic
- **Assembly metadata**: Version and company info is in `SymlinkCreator.csproj` (SDK-style), not AssemblyInfo.cs

### SDK-Style Project Notes
- This project uses **SDK-style** `.csproj` format (modern .NET)
- Assembly version and metadata are auto-generated from `.csproj` properties
- `AssemblyInfo.cs` should contain only ComVisible and ThemeInfo attributes
- NuGet dependencies are declared as `<PackageReference>` in the .csproj

## Git Workflow

This is a fork of [arnobpl/SymlinkCreator](https://github.com/arnobpl/SymlinkCreator). Licensed under MIT.

- **Fork Location**: https://github.com/scopweb/SymlinkCreator
- **Upstream**: https://github.com/arnobpl/SymlinkCreator.git
- Push changes to the fork's `main` branch

## Migration History (v1.4.0)

### Framework Migration: .NET Framework 4.8 → .NET 10
- Migrated both .csproj files to SDK-style format
- Target framework changed to `net10.0-windows` (latest LTS)
- Updated all NuGet packages to latest versions compatible with .NET 10
- Removed legacy package configuration system (packages.config → PackageReference)

### Dependency Removals
- **Costura.Fody**: Removed. Use `dotnet publish ... --self-contained` instead for self-contained distribution
- **WindowsAPICodePack-Core/Shell**: Removed. Replaced with native `Microsoft.Win32.OpenFolderDialog` (.NET 6+ built-in)
- Kept only: `System.Configuration.ConfigurationManager` (for App.config support)

### UI Modernization
- Created professional dark theme (`DarkTheme.xaml`) with Catppuccin Mocha palette
- Completely redesigned `MainWindow.xaml` with modern layout and spacing
- Updated `AboutWindow.xaml` with fork attribution and enhanced styling
- Replaced deprecated file dialogs with native WPF dialogs
- All controls now inherit from theme ResourceDictionary

### Code Cleanup
- Simplified `AssemblyInfo.cs` files (removed auto-generated properties)
- Updated version to **v1.4.0** across all metadata
- Removed obsolete code paths for legacy frameworks
- All compilation errors resolved (0 C# errors, 0 XAML errors)
