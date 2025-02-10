# Changelog

All notable changes will be documented in this file.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.1.0
- DLLs are now correctly versioned
- Scale monster alert radius with instance volume
- Inputs now respect other objects that capture them, so the tape player will no longer interfere with (for example) shop clicks
- Changed primary track selection keybind to mouse wheel
  - Scroll in either direction to cycle tracks up or down
- Fixed instance volume not synchronising between players correctly
- Fixed interaction SFX only playing for the player holding the item
  - Now plays correctly for everyone else
- [STEAM] Fixed issue where if the mod was installed *while* the game is running, sometimes the ShopAPI dependency would load second, causing the game to break
  - While the load order in this case is outside of my control, the plugin loader will now prompt players to restart the game

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