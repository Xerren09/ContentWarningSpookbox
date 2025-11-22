Content Warning Spöökbox
===

[![Steam Downloads](https://img.shields.io/steam/downloads/3410924959?style=for-the-badge&logo=steam&label=Downloads)](https://steamcommunity.com/sharedfiles/filedetails/?id=3410924959)
[![Steam Subscriptions](https://img.shields.io/steam/subscriptions/3410924959?style=for-the-badge&logo=steam&label=Subscriptions)](https://steamcommunity.com/sharedfiles/filedetails/?id=3410924959)
[![Steam Last Update Date](https://img.shields.io/steam/update-date/3410924959?style=for-the-badge&logo=steam&label=Updated)](https://steamcommunity.com/sharedfiles/filedetails/?id=3410924959)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/Xerren/Spookbox?style=for-the-badge&logo=thunderstore&label=Downloads)](https://thunderstore.io/c/content-warning/p/Xerren/Spookbox/)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Xerren/Spookbox?style=for-the-badge&logo=thunderstore&label=Version)](https://thunderstore.io/c/content-warning/p/Xerren/Spookbox/)

Adds a silly boombox (The Spöökbox™) that lets players dance to any tune they want while going viral :]

## Features
* **Play your own music:** Load your local mp3 files to create your own mixtape to play!
* **Dance with the monsters of the Old World:** In-game setting to allow monsters to also hear the music and let them know where you are... Some may not be happy.
* **The party never stops:** Even if you stash the Spöökbox it will keep playing, so you are free to record or fight G L O R P and keep the beats flowing.


## How to install

You can download this mod from either the Steam Workshop or Thunderstore. Please note that you should only download it from one place, otherwise you might run into problems.

> [!IMPORTANT]
> Everyone in your lobby must have the mod installed for it to work!

### Steam Workshop

Spöökbox is easiest to install from the Steam Workshop. Just go the [mod's Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=3410924959) and click Subscribe.

### Thunderstore (r2modman)

If you are using r2modman, search for "spookbox" in the app, or go to [the mod's page directly](https://thunderstore.io/c/content-warning/p/Xerren/Spookbox/) and click "Install with Mod Manager".


## Dependencies

This mod depends on the [ShopAPI mod](https://github.com/Xerren09/ContentWarningShopAPI) to register itself into the in-game store. If you are installing this mod from the Steam Workshop or Thunderstore, this *should be* resolved and handled automatically.


## How to use

### Loading music

Loading music can be done from multiple source directories at the same time. **Keep in mind that you need to have your own mp3 files for this to work.** 

> [!IMPORTANT]
> Everyone in your lobby must have the same mp3 files! Once you created a good list of tunes, send them to your friends and have them copy them into the above directory.

> [!NOTE]
> The maximum number of individual tracks that can be loaded is 255.

Once you have the list of music you want to use, there are two easy ways to load them into the boombox:
1. Go to your Music folder (`%userprofile%\Music`), create a folder called `SpookboxMixtape`, and copy your files in there.
2. Launch the game, open Settings > Mods, and click the `Open music folder` setting. This will open the mod's own music directory. Do note that this folder will be deleted when you uninstall the mod.
   - This folder changes depending on the version of the mod you use. For the steam version, it will be in your Steam Library folder (by default at `C:\Program Files (x86)\Steam\steamapps\workshop\content\2881650\3410924959\SpookboxMixtape`) 
   - For the Thunderstore version it will be at your profile's plugins folder, in the mod's own folder

> [!IMPORTANT]
> If you have the game open while adding or removing tracks. make sure to click the `Reload music` setting to have your changes take effect.


### Thunderstore

Once you have Content Warning and your profile selected, go to Settings -> Browse profile folder. From there go to `BepInEx\plugins\Xerren-Spookbox\SpookboxMixtape` and copy-paste your music files. You can also make use of content mods. The boombox's music loader will scan all other mods for directories with the `SpookboxMixtape` name and load all music files within them automatically.

#### Content Mods

If you are using Thunderstore, you can also create content mods. These mods don't add extra functionality, but can provide extra assets to other mods; in Spöökbox's case this means you can create a content pack with music inside!

Follow the [thunderstore guide](https://thunderstore.io/c/content-warning/create/docs/) to create a new mod. Add the dependency string from the [mod's page](https://thunderstore.io/c/content-warning/p/Xerren/Spookbox/) to the `manifest.json`'s `dependencies` array. Then, in the same folder with all your files, create a `SpookboxMixtape` folder, and copy all your music files into it.

Zip the entire base directory up (directory with the `icon.png`, `manifest.json`, `readme.md` files and the `SpookboxMixtape` folder), and upload it to Thunderstore. Done! :) Now your friends can download the content mod and be assured that you all share the same tunes.

## In-game settings

Settings that affect the experience for *everyone* are designed to be quickly adaptable during gameplay. For example, the host can set the boombox's battery to no longer discharge. This will automatically synchronise for every player in the lobby, but will **not** change your own settings, so while the effect will be the same for every player, visually the toggle may be off in their own settings menu. 

> [!NOTE]
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
