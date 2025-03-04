Content Warning Spöökbox
===
Adds a silly boombox (The Spöökbox™) to the store so you can dance to any tune you want while going viral :]

## How to use

### Loading music

Loading music can be done from multiple source directories. **Keep in mind that you need to have your own mp3 files for this to work.** 

> **IMPORTANT:**
> Everyone in your lobby must have the same mp3 files! Once you created a good list of tunes, send them to your friends and have them copy them into the above directory.

> **NOTE:**
> The maximum number of individual tracks that can be loaded is 255.

Once you have the list of music you want to use, there are two easy ways to load them into the boombox:
1. Go to your Music folder (`%userprofile%\Music`), create a folder called `SpookboxMixtape`, and copy your files in there.
2. Launch the game, open Settings > Mods, and click the `Open music folder` setting. This will open the mod's own music directory. Do note that this folder will be deleted when you uninstall the mod.

> **IMPORTANT:**
> If you have the game open while adding or removing tracks. make sure to click the `Reload music` setting to have your changes take effect.

### Thunderstore Content Mods

If you are using Thunderstore, you can also create content mods. These mods don't add extra functionality, but can provide extra assets to other mods; in Spöökbox's case this means you can create a content pack with music inside! 

Follow the [thunderstore guide](https://thunderstore.io/c/content-warning/create/docs/) to create a new mod. Add the dependency string from the [mod's page](https://thunderstore.io/c/content-warning/p/Xerren/Spookbox/) to the `manifest.json`'s `dependencies` array. Then, in the same folder with all your files, create a `SpookboxMixtape` folder, and copy all your music files into it.

Zip the entire base directory up (directory with the `icon.png`, `manifest.json`, `readme.md` files and the `SpookboxMixtape` folder), and upload it to Thunderstore. Done! :) Now your friends can download the content mod and be assured that you all share the same tunes.

## In-game settings

Settings that affect the experience for *everyone* are designed to be quickly adaptable during gameplay. For example, the host can set the boombox's battery to no longer discharge. This will automatically synchronise for every player in the lobby, but will **not** change your own settings, so while the effect will be the same for every player, visually the toggle may be off in their own settings menu. 

> **NOTE:**
> This may be confusing at first glance but it is deliberate, so when the next time you are the one hosting a game, your settings will be unique to you, instead of whatever your friends had. These settings are suffixed with [HOST], meaning only the current lobby's host's changes will have effect; if you are not the host, changing these will do nothing.

### Local boombox volume

Controls all boombox's local maximum volume. While you can still change their individual, shared volume during gameplay, you can use this slider to personalise how loud you want the music to be on your end (similar to the other volume sliders in the settings). Sometimes TUCA DONKA blasting in your ear can be a bit too much. I get you.

### Volume Up and Down Keys

Each boombox has an individual volume setting, so you can control how ~~annoying~~ loud each of you are. By default it is always 50%, but you can change it by pressing these keys. This is synchronised between player for the specific boombox instances.

### [HOST] "Let BigSlap hear the tunes"

You know what this does (alerts nearby monsters) (also affects other monsters). Only the host can set this, but they may freely change it anytime during the run (which will affect everyone).

### [HOST] Infinite battery

Check this if you don't want the boombox to ever run out of battery. Only the host can set this, but they may freely change it anytime during the run (which will affect everyone).

### [HOST] Price

Sets the boombox's price. By default it is 100$. Only the host can set this, but they may freely change it anytime during the run (which will affect everyone).

### Open music folder

Opens the mod's own music folder in Windows Explorer.

Please note that if you change music while the game is running, you must then click `Rescan music` to reload them.

### Rescan music

Rescans and reloads all tracks to the boombox.

## Credits

Model: [My dearest friend, Cinnaboop <3](https://sketchfab.com/3d-models/content-warning-mod-boombox-a62dd39d143c41d18e68ab55de2cb0ca)

Emotional support: Echo (was in the call) :]

Click SFX (slightly edited): Tape Start 139BPM Sync by djlprojects -- https://freesound.org/s/392890/ -- License: Attribution 4.0