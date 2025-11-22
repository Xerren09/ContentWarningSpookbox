# Changelog

All notable changes will be documented in this file.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.3
- Updated to Unity 6 (6000.0.59) to match game version 1.20
- Added additional install error messages
- Clarified error messages
- Updated model texture to match the game's vibe a bit more :3
- Added equip SFX
- Added physics SFX

## 1.2.0
- Fixed some settings not being registered when installed late
- Settings are now manually registered
  - This is to avoid corrupting the game when a load fails
- Fixed boombox still playing in stashed mode when the holding player dies
- Fixed mixtape rescan action breaking the currently playing song
  - Now attempts to continue where it left off if possible
- Removed right click track selection completely
  - Use mouse scroll / camera zoom action instead
  - RMB now does something silly :)
- Adjusted PluginLoader for the Steam release to behave more like BepInEx
  - Should capture more faults, provide better warnings and not corrupt the game when something goes wrong
  - Settings will now NOT appear in game when a load fails

## 1.1.0
- DLLs are now correctly versioned
- Scale monster alert radius with instance volume
- Inputs now respect other objects that capture them, so the tape player will no longer interfere with (for example) shop clicks
- Changed primary track selection keybind to mouse wheel
  - Scroll in either direction to cycle tracks up or down
  - For now right click still works, but this will be removed in the next release
- Fixed instance volume not synchronising between players correctly
- Fixed interaction SFX only playing for the player holding the item
  - Now plays correctly for everyone else
- [STEAM] Fixed issue where if the mod was installed *while* the game is running, sometimes the ShopAPI dependency would load second, causing the game to break
  - While the load order in this case is outside of my control, the plugin loader will now prompt players to restart the game
- Added two entries to settings
  - One opens the mod's own music directory from the game, for easy access. This is folder is next to the mod DLL, and will be deleted when the mod is uninstalled
  - The other one reloads mixtape tracks, so restarting the game is no longer needed

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