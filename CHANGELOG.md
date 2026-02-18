# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
