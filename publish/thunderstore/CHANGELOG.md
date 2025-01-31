# Changelog

All notable changes will be documented in this file.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.0.3
- Added missing monster alerts when boombox is stashed
- Host settings are now correctly applied when you leave a lobby and then host your own
- Changed mod startup to manually register settings
  - This (hopefully) fixes the surprisingly common scenario where if the mod is installed *after* the game is started, the game doesn't auto register its settings.

## 1.0.2
- Fixed issue during config if no tracks were loaded
- Removed BoomboxPriceSetting no lobby restriction. Price can now be changed even in a lobby, but only by the host.
- Adjusted Spookbox text emission to be slightly green

## 1.0.1
- Added missing resource file

## 1.0.0
- Initial Release