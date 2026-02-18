# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Symlink Creator** is a WPF desktop application for creating Windows symlinks (symbolic links) via the `mklink` command. This is a fork with added configuration support for default paths.

- **Framework**: .NET Framework 4.8 (WPF)
- **Build System**: MSBuild / Visual Studio
- **Testing**: MSTest unit tests
- **Dependencies**: NuGet (Costura.Fody for assembly embedding)

## Architecture

### Folder Structure
- **`SymlinkCreator/`**: Main WPF application
  - `core/`: Business logic
    - `SymlinkAgent.cs`: Creates symlinks and manages script execution
    - `ScriptExecutor.cs`: Executes batch scripts with admin privileges
    - `ApplicationConfiguration.cs`: Loads config from App.config
  - `ui/`: User interface (WPF/XAML)
    - `mainWindow/`: Main application window with MVVM ViewModel
    - `aboutWindow/`: About dialog
    - `utility/`: UI helpers (admin shield icon, path handling)
  - `resources/`: Icons and media
  - `App.config`: Configuration file with `DefaultSourceFolderPath` and `DefaultDestinationFolderPath` settings
  - `app.manifest`: Requires admin privileges for symlink creation

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
# Using Visual Studio or MSBuild
msbuild SymlinkCreator/SymlinkCreator.sln /p:Configuration=Release

# Output: SymlinkCreator/bin/Release/SymlinkCreator.exe
```

### Run
The application requires **administrator privileges** (enforced by `app.manifest`).

```bash
# Run the executable directly (will prompt for admin)
SymlinkCreator/bin/Release/SymlinkCreator.exe
```

### Tests
```bash
# Run all tests
msbuild SymlinkCreator/SymlinkCreator.sln /t:Test /p:Configuration=Debug

# Or using Visual Studio Test Explorer
# Open the solution and run tests via Test > Run All Tests
```

## Key Implementation Details

### Symlink Creation Process
1. **SymlinkAgent** collects source files/folders and destination path
2. **ScriptExecutor** generates a batch script with `mklink` commands
3. Script is executed with elevated privileges (admin mode)
4. Results are returned to the UI

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

## Important Notes

### Never Modify
- Do not change the target .NET Framework version without careful testing
- Do not remove the `app.manifest` - it's required for admin privileges
- Do not bypass admin checks

### When Adding Features
- Keep MVVM pattern: logic in ViewModel/Agent, UI in XAML
- Test both relative and absolute path scenarios
- Consider Windows compatibility (paths with spaces, Unicode, long paths)
- Update `App.config` for any new configurable settings
- Add tests to `SymlinkCreatorTests/` for new core logic

### Build Process
- Costura.Fody embeds dependent assemblies into the main executable
- The generated `.exe` is fully self-contained (no DLL dependencies needed)
- Release builds are optimized for distribution

## Git Workflow

This is a fork of [arnobpl/SymlinkCreator](https://github.com/arnobpl/SymlinkCreator). Licensed under MIT.

- **Fork Location**: https://github.com/scopweb/SymlinkCreator
- **Upstream**: https://github.com/arnobpl/SymlinkCreator.git
- Push changes to the fork's `main` branch
