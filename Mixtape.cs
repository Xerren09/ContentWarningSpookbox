using UnityEngine;
using UnityEngine.Networking;

namespace Spookbox
{
    public static class Mixtape
    {
        public const string ASYNC_LOAD_ENV = "-ihaveanssd";
        public const string TRACKS_DIR_NAME = "SpookboxMixtape";
        private static bool _asyncLoadTracks = false;
        /// <summary>
        /// The maximum number of tracks the mod will load. Realistically this is more than reasonable, but also
        /// it is set to ensure the track index can be encoded into a single byte during network synchronisation.
        /// </summary>
        public const int MAX_TRACKS = 255;
        public static readonly List<AudioClip> Tracks = new();
        public static int Length => Tracks.Count;

        /// <summary>
        /// Checks if any tracks are loaded.
        /// </summary>
        /// <returns></returns>
        public static bool HasTracks()
        {
            return Tracks.Count > 0;
        }

        /// <summary>
        /// Wipes the current music list and loads all music tracks.
        /// </summary>
        /// <remarks>
        /// Note that the number of tracks loaded will not exceed <see cref="MAX_TRACKS"/>.
        /// </remarks>
        public static void LoadTracks()
        {
            _asyncLoadTracks = Environment.GetCommandLineArgs().Contains(ASYNC_LOAD_ENV);
            Debug.Log($"Async load audio clips ({ASYNC_LOAD_ENV} flag) : {_asyncLoadTracks}");
            //
            Tracks.Clear();
            var files = GetAllTracks();
            Debug.Log($"{files.Count} potential mixtape tracks found.");
            foreach (var file in files)
            {
                if (Tracks.Count == MAX_TRACKS)
                {
                    Debug.LogWarning($"Mixtape full: maximum track count limit reached ({MAX_TRACKS}).");
                    break;
                }
                var type = GetAudioType(file);
                if (type == AudioType.UNKNOWN)
                {
                    Debug.LogWarning($"Track \"{file}\" could not be loaded due to unsupported audio type.");
                    continue;
                }
                var track = LoadAudioClipFromFile(file, GetAudioType(file));
                if (track == null)
                {
                    continue;
                }
                track.name = Path.GetFileNameWithoutExtension(file);
                Tracks.Add(track);
                Debug.Log($"Added track to mixtape: {track.name}");
            }

        }

        /// <summary>
        /// Gets the path to the main directory where users can put their custom songs into.
        /// </summary>
        /// <returns></returns>
        private static string GetUserMusicDirPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), TRACKS_DIR_NAME);
        }

        /// <summary>
        /// Gets the path to the mod's own music directory. This is gone once the mod is uninstalled.
        /// </summary>
        /// <remarks>
        /// Creates the directory if it doesn't already exists.
        /// </remarks>
        /// <returns></returns>
        private static string GetPluginMusicDirPath()
        {
            var dirPath = Path.GetFullPath(Path.Combine(SpookboxPlugin.PluginDirPath, TRACKS_DIR_NAME));
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }

        /// <summary>
        /// Gets the list of all music files from every eligible directory.
        /// </summary>
        /// <remarks>
        /// Depending on target platform the directories differ:
        /// 
        /// If <c>STEAM</c> is defined, only scans the plugin's own directory and the user's music directory.
        /// 
        /// If <c>MODMAN</c> is defined, also scans other plugins for the content mod directory.
        /// 
        /// </remarks>
        /// <returns></returns>
        internal static List<string> GetAllTracks()
        {
            var ret = new List<string>();
            ret.AddRange(GetTracksFromDir(GetUserMusicDirPath()));
            ret.AddRange(GetTracksFromDir(GetPluginMusicDirPath()));
#if MODMAN
            //scan content mods
            var bepinPluginDir = Path.Combine(SpookboxPlugin.PluginDirPath, @"..\");
            var pluginDirs = Directory.GetDirectories(bepinPluginDir);
            foreach ( var dir in pluginDirs )
            {
                ret.AddRange(GetTracksFromDir(Path.Combine(dir, TRACKS_DIR_NAME)));
            }
#endif
            ret = ret.OrderBy(Path.GetFileName, StringComparer.InvariantCultureIgnoreCase).ToList();
            return ret;
        }

        private static IEnumerable<string> GetTracksFromDir(string path)
        {
            IEnumerable<string> ret = Array.Empty<string>();
            if (Directory.Exists(path) == false)
            {
                return ret;
            }
            // (char)0xfffd : Fixes stupid bug caused by mp3 converters sometimes not encoding titles correctly
            ret = Directory.GetFiles(path).Where(p => GetAudioType(p) != AudioType.UNKNOWN && p.Contains((char)0xfffd) == false);
            return ret;
        }

        private static AudioType GetAudioType(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".wav":
                    return AudioType.WAV;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".mp3":
                    return AudioType.MPEG;
                default:
                    return AudioType.UNKNOWN;
            }
        }

        private static AudioClip LoadAudioClipFromFile(string path, AudioType type)
        {
            AudioClip ret = null;
            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(path, type);
            // Safe load by default, or stream by request. Fast load works well for SSDs where realistically by the time the player
            // is in a lobby, all the tracks are loaded.
            if (_asyncLoadTracks)
            {
                ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = true;
            }
            try
            {
                req.SendWebRequest();
                try
                {
                    while (req.isDone == false)
                    { }
                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        ret = DownloadHandlerAudioClip.GetContent(req);
                    }
                    else
                    {
                        Debug.LogError($"Loading AudioClip from {path} failed: {req.error}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Loading AudioClip failed:");
                    Debug.LogException(ex);
                }
            }
            catch (Exception ex)
            {

                Debug.LogError($"Request for {path} failed:");
                Debug.LogException(ex);
            }
            finally
            {
                req?.Dispose();
            }
            return ret;
        }
    }
}
