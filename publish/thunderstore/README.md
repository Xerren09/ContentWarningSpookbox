Content Warning Boombox [Reloaded]
===
Adds a boombox into the store that can play any music you add to it.

## Dependencies

This mod depends on the ShopAPI mod to register itself into the in-game store. If you are installing this mod from the Steam Workshop or Thunderstore, this *should be* resolved and handled automatically.

## How to use

### Loading music

Loading music can be done from multiple source directories. Keep in mind that you need to have your own mp3 files for this to work. Once you have the list of music you want to use, the simplest way to load them into the boombox is to go to your Music folder (`%userprofile%\Music`), create a folder called `BoomboxTracks`, and copy your files there.

> [!NOTE]
> The maximum number of individual tracks that can be loaded is 255.

If you don't want to use your music folder, you can also use the mod's own tracks folder (note however that this will be deleted along with the mod when you uninstall it):

#### Steam Workshop

Locate your Steam installation folder (by default at `C:\Program Files (x86)\Steam\steamapps`), go to `workshop\content\2881650\BoomboxReloaded\BoomboxTracks` and copy-paste your music files.

#### R2Modman

Once you have Content Warning and your profile selected, go to settings -> Browse profile folder. From there go to `BepInEx\plugins\xerren-boombox\BoomboxTracks` and copy-paste your music files. You can also make use of content mods. The boombox's music loader will scan all other mods for directories with the `BoomboxTracks` name and load all music files within them automatically.

> [!TIP]
> Tracks are loaded synchronously by default, so it may take a little while for the game to start up and it may appear frozen for a few seconds. If you have an SSD or only a few songs, add "-ihaveanssd" to the Steam Launch Options for a much faster startup experience.

> [!WARNING]
> Setting this flag with a large number of tracks or a slow drive may cause issues; remove the flag and restart the game if you experience desync problems with the music.

---
## In-game settings

Settings that affect the experience for *everyone* are designed to be quickly adaptable during gameplay. For example, the host can set the boombox's battery to no longer discharge. This will automatically synchronise for every player in the lobby, but will **not** change your own settings, so while the effect will be the same for every player, visually the toggle may be off in their own settings menu. 

> [!NOTE]
> This may be confusing at first glance but it is deliberate, so when the next time you are the one hosting a game, your settings will be unique to you, instead of whatever your friends had. These settings are suffixed with [HOST], meaning only the current lobby's host's changes will have effect; if you are not the host, changing these will do nothing.

#### Local boombox volume

Controls the Boombox's local volume. While you can still set a global, shared volume during gameplay, use this slider to personalise how loud you want the music to be on your end (similar to the other volume sliders in the settings). Sometimes TUCA DONKA blasting in your ear can be a bit too much. I get you.

#### "Let BigSlap hear the tunes"

You know what this does (alerts nearby monsters) (also affects other monsters). Only the host can set this, but they may freely change it anytime during the run (which will affect everyone).

#### Price

Sets the boombox's price. By default it is 100$. Only the host can set this. Once a lobby has been created, changing it will have no effect until a new lobby is started, so set this early, before starting a game.

#### Infinite battery

Check this if you don't want the boombox to ever run out of battery. Only the host can set this, but they may freely change it anytime during the run (which will affect everyone).
