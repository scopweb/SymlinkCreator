# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.4.1] - 2026-02-19

### Changed
- **Dark Theme Refinements**:
  - Improved button hover and press states with gray color transitions (#262630 hover, #303039 pressed)
  - Fixed TextBlock color inheritance for better title visibility
  - Adjusted subtext color to #888888 for improved contrast on dark backgrounds
  - Implemented custom ControlTemplate for Button to properly support theme triggers
- **UI Polish**: Removed hardcoded button foreground colors to allow proper style inheritance

### Fixed
- Fixed issue where section titles (Source files, Destination path, Options) were hard to read on dark backgrounds
- Button hover states now properly display gray instead of default system colors

## [1.4.0] - 2026-02-18

### Added
- **CLAUDE.md**: Comprehensive project architecture and development guide for future contributors
- **Configurable Default Paths**: New configuration options in `App.config`:
  - `DefaultSourceFolderPath`: Sets default directory for "Add folders" dialog
  - `DefaultDestinationFolderPath`: Sets default directory for "Destination path" dialog
- Long path awareness support for Windows paths exceeding 260 characters

### Changed
- Updated README.md with fork documentation and feature overview
- Removed donation and sponsorship section from README

### Fixed
- Unicode folder name support in SymlinkCreator executable path
- Long path aware registry settings integration

## [1.3.0] and Earlier

See original repository: https://github.com/arnobpl/SymlinkCreator/releases

---

## Fork Information

This is a fork of [Symlink Creator](https://github.com/arnobpl/SymlinkCreator) by Arnob Paul.

**Original License**: MIT License (© 2019 Arnob Paul)
**Fork Repository**: https://github.com/scopweb/SymlinkCreator
