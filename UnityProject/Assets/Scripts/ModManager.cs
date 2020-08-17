﻿using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ModManager : MonoBehaviour {
    public static List<Mod> loadedGunMods = new List<Mod>();
    public static List<Mod> loadedLevelMods = new List<Mod>();
    public static List<Mod> loadedTapeMods = new List<Mod>();

    public static List<Mod> availableMods = new List<Mod>();

    private static int numSteamMods = 0;

    public LevelCreatorScript levelCreatorScript;
    public GUISkinHolder guiSkinHolder;
    public InbuildMod[] inbuildMods;

    public static Dictionary<ModType, string> mainAssets = new Dictionary<ModType, string> {
        {ModType.Gun, "gun_holder.prefab"},
        {ModType.LevelTile, "tiles_holder.prefab"},
        {ModType.Tapes, "tape_holder.prefab"},
    };

    private static ModManager _instance;
    public static ModManager instance {
        get {
            if(!_instance) // static "_instance" is cleared during hotswaps, reassign value
                _instance = UnityEngine.Object.FindObjectOfType<ModManager>();
            return _instance;
        }
    }

    public void Awake() {
        // Setup static reference
        ModManager._instance = this;

        //Make sure these folders are generated if they don't exist
        if(!Directory.Exists(GetModsfolderPath())) {
            Directory.CreateDirectory(GetModsfolderPath());

            // Generate inbuild mods
            foreach (var mod in inbuildMods)
                mod.Generate();
        }

        // Are mods enabled?
        if(PlayerPrefs.GetInt("mods_enabled", 0) != 1)
            return;

        LoadCache();

        if(availableMods.Count != GetModFolderCount() + PlayerPrefs.GetInt("num_steam_mods", 0)) { // Is our Cache up to date?
            UnloadAll();
            ImportMods();
            UpdateCache();
        }
        
        // Load everything but guns
        foreach (var mod in availableMods.Where((mod) => mod.modType != ModType.Gun))
            mod.Load();

        InsertMods();
    }

    public void OnDestroy() {
        UnloadAll();
        availableMods.Clear();
    }

    public Mod LoadSteamItem(string path) {
        Mod mod = ImportMod(path);
        UpdateCache();
        
        // Load everything but guns
        if (mod.modType != ModType.Gun) {
            mod.Load();
        }

        // Insert mods into lists
        switch (mod.modType) {
            case ModType.Gun: {
                ModLoadType gun_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_gun_loading", 0);
                if (gun_load_type != ModLoadType.DISABLED) {
                    var guns = new List<GameObject>(guiSkinHolder.weapons);
                    WeaponHolder placeholder = new GameObject().AddComponent<WeaponHolder>();
                    placeholder.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                    placeholder.mod = mod;
                    placeholder.display_name = mod.name;
                    guns.Add(placeholder.gameObject);
                    guiSkinHolder.weapons = guns.ToArray();
                }
                break;
            }
            case ModType.LevelTile: {
                if (levelCreatorScript) {
                    ModLoadType tile_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tile_loading", 0);
                    if (tile_load_type != ModLoadType.DISABLED) {
                        var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
                        foreach (GameObject tile in mod.mainAsset.GetComponent<ModTilesHolder>().tile_prefabs) {
                            tiles.Add(tile);
                        }
                        levelCreatorScript.level_tiles = tiles.ToArray();
                    }
                }
                break;
            }
            case ModType.Tapes: {
                ModLoadType tape_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tape_loading", 0);
                if (tape_load_type != ModLoadType.DISABLED) {
                    foreach (AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes) {
                        guiSkinHolder.sound_tape_content.Add(tape);
                    }
                }
                break;
            }
        }

        numSteamMods++;
        PlayerPrefs.SetInt("num_steam_mods", numSteamMods);

        return mod;
    }

    public static string GetModsfolderPath() {
        return Path.Combine(Application.persistentDataPath, "Mods").Replace('\\', '/');
    }

    public void InsertMods() {
        // Insert all gun mods
        ModLoadType gun_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_gun_loading", 0);
        if(gun_load_type != ModLoadType.DISABLED) {
            var guns = new List<GameObject>(guiSkinHolder.weapons);
            var availableGuns = availableMods.Where((mod) => mod.modType == ModType.Gun);
            if(availableGuns.Count() > 0 && gun_load_type == ModLoadType.EXCLUSIVE)
                guns.Clear();

            foreach (var mod in availableGuns) {
                WeaponHolder placeholder = new GameObject().AddComponent<WeaponHolder>();
                placeholder.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                placeholder.mod = mod;
                placeholder.display_name = mod.name;
                guns.Add(placeholder.gameObject);
            }
            guiSkinHolder.weapons = guns.ToArray();
        }

        // Insert all Level Tile mods
        if(levelCreatorScript) {
            ModLoadType tile_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tile_loading", 0);
            if(tile_load_type != ModLoadType.DISABLED) {
                var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
                if(loadedLevelMods.Count > 0 && tile_load_type == ModLoadType.EXCLUSIVE)
                    tiles.Clear();

                foreach (var mod in loadedLevelMods)
                    foreach(GameObject tile in mod.mainAsset.GetComponent<ModTilesHolder>().tile_prefabs)
                        tiles.Add(tile);
                levelCreatorScript.level_tiles = tiles.ToArray();
            }
        }

        // Insert all Tape mods
        ModLoadType tape_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tape_loading", 0);
        if(tape_load_type != ModLoadType.DISABLED) {
            if(loadedTapeMods.Count > 0 && tape_load_type == ModLoadType.EXCLUSIVE)
                guiSkinHolder.sound_tape_content.Clear();

            foreach (var mod in loadedTapeMods)
                foreach(AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes)
                    guiSkinHolder.sound_tape_content.Add(tape);
        }
    }

    public void UnloadAll() {
        foreach (var mod in availableMods) {
            mod.Unload();
        }
    }

    private static List<Mod> GetModList(ModType modType) {
        switch (modType) {
            case ModType.Gun: return loadedGunMods;
            case ModType.LevelTile: return loadedLevelMods;
            case ModType.Tapes: return loadedTapeMods;

            default:
                throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");
        }
    }

    public static String GetMainAssetName(ModType modType) {
        if(!mainAssets.ContainsKey(modType))
            throw new System.InvalidOperationException($"Unknown Mod Type \"{modType.ToString()}\"");

        return mainAssets[modType];
    }

    /// <summary> Add / Remove mod from the ModManager.loadedGunMods, ModManager.loadedLevelMods or ModManager.loadedLevelMods list. <summary>
    public static void UpdateModInLoadedModlist(Mod mod) {
        List<Mod> list = GetModList(mod.modType);

        if(list.Contains(mod) == mod.loaded) // Is the mod already registered as loaded/unloaded?
            return;

        if(mod.loaded) {
            list.Add(mod);
        } else {
            list.Remove(mod);
        }
    }

    public static string GetModsFolder(ModType modType) {
        var path = Path.Combine(GetModsfolderPath(), modType.ToString());
        
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static ModType GetModTypeFromBundle(AssetBundle assetBundle) {
        foreach (ModType modType in mainAssets.Keys)
            if(assetBundle.Contains(mainAssets[modType]))
                return modType;

        throw new System.InvalidOperationException($"Unable to find Mod Type for \"{assetBundle.name}\"");
    }

    public static string[] GetModPaths() {
        return Directory.GetDirectories(GetModsfolderPath(), "modfile_*", SearchOption.AllDirectories);
    }

    public static int GetModFolderCount() {
        return GetModPaths().Count();
    }

    public static void ImportMods() {
        Debug.Log($"Importing mods..");
        availableMods = new List<Mod>();
        foreach(var path in GetModPaths()) {
            try {
                ImportMod(path);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {path}: {e.Message}");
            }
        }
        Debug.Log($"Mod importing completed. Imported {availableMods.Count} mods!");
    }

    public static Mod ImportMod(string path) {
        string[] bundles = Directory.GetFiles(path);
        string bundleName = bundles.FirstOrDefault((name) => name.EndsWith(SystemInfo.operatingSystemFamily.ToString(), true, null));

        // Fallback to unsigned mods (old naming version without os versions)
        if(bundleName == null && Path.GetFileName(path).StartsWith("modfile_")) {
            bundleName = bundles.FirstOrDefault((name) => name.EndsWith(Path.GetFileName(path).Substring(8), true, null) && !Path.GetFileName(name).StartsWith("modfile_"));
            if(bundleName == null) {
                throw new Exception($"No compatible mod version found for os family: '{SystemInfo.operatingSystemFamily}' for mod: '{path}'");
            }
        }

        // Init
        var assetPath = Path.Combine(path, bundleName);
        var modBundle = AssetBundle.LoadFromFile(assetPath);

        // Generate Mod Object
        var mod = new Mod(assetPath);
        mod.name = Path.GetFileName(bundleName);
        mod.modType = GetModTypeFromBundle(modBundle);

        // Determine gun display name for the cache
        if(mod.modType == ModType.Gun)
            mod.name = modBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(ModType.Gun)).GetComponent<WeaponHolder>().display_name;

        // Register mod and clean up
        availableMods.Add(mod);
        modBundle.Unload(true);
        //Debug.Log($" + {bundleName} ({mod.modType})");

        return mod;
    }

    private static void LoadCache() {
        string path = Path.Combine(ModManager.GetModsfolderPath(), "cache");

        if(File.Exists(path))
            availableMods = new List<Mod> (JsonUtility.FromJson<Cache>(File.ReadAllText(path)).mods);
        else
            availableMods = new List<Mod> ();
    }

    private static void UpdateCache() {
        try {
            string path = Path.Combine(ModManager.GetModsfolderPath(), "cache");

            if(File.Exists(path))
                File.Delete(path);

            File.Create(path).Close();
            File.WriteAllText(path, JsonUtility.ToJson(new Cache(availableMods.ToArray())));
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    [System.Serializable]
    private struct Cache {
        public Mod[] mods;

        public Cache (Mod[] mods) {
            this.mods = mods;
        }
    }
}

public enum ModLoadType {
    ENABLED,
    EXCLUSIVE,
    DISABLED,
}

public enum ModType {
    Gun,
    Tapes,
    LevelTile
}

[System.Serializable]
public class Mod {
    public ModType modType;
    public string name = "None";

    private UnityWebRequest thumbnailProcess;
    private Texture2D _thumbnail;
    public Texture2D thumbnail {
        get {
            if(_thumbnail)
                return _thumbnail;

            if(thumbnailProcess == null) {
                if(File.Exists(GetThumbnailPath())) {
                    thumbnailProcess = UnityWebRequestTexture.GetTexture($"file:///{GetThumbnailPath()}");
                    thumbnailProcess.SendWebRequest();
                } else {
                    _thumbnail = new Texture2D(450, 450);
                }
            } else if(thumbnailProcess.isDone && !thumbnailProcess.isNetworkError) {
                return _thumbnail = DownloadHandlerTexture.GetContent(thumbnailProcess);
            }

            return new Texture2D(450, 450);
        }
    }

    [NonSerialized] public bool loaded = false;
    
    public string path;
    [NonSerialized] public AssetBundle assetBundle;

    [NonSerialized] public GameObject mainAsset;

    [NonSerialized] public SteamworksUGCItem steamworksItem;

    public Mod(string path) {
        this.path = path;
    }

    public void Load() {
        if(loaded)
            return;

        // Loading
        loaded = true;
        assetBundle = AssetBundle.LoadFromFile(path);
        mainAsset = assetBundle.LoadAsset<GameObject>(ModManager.GetMainAssetName(this.modType));
        ModManager.UpdateModInLoadedModlist(this);
    }

    public void Unload() {
        if(!loaded)
            return;

        loaded = false;
        assetBundle.Unload(true);
        ModManager.UpdateModInLoadedModlist(this);
    }

    public string GetTypeString() {
        switch (modType) {
            case ModType.Gun:
                return "Gun";
            case ModType.LevelTile:
                return "Tile";
            case ModType.Tapes:
                return "Tapes";
        }
        return "";
    }

    public string GetThumbnailPath() {
        return Path.Combine(Path.GetDirectoryName(path), "thumbnail.jpg");
    }
}

[System.Serializable]
public class InbuildMod {
    public string name;
    public TextAsset[] files;

    public void Generate() {
        try {
            // Create directory
            string directory = Path.Combine(ModManager.GetModsfolderPath(), $"modfile_inbuild_{name}");
            Directory.CreateDirectory(directory);

            // Create files
            foreach(TextAsset file in files) {
                string path = Path.Combine(directory, Path.GetFileNameWithoutExtension(file.name));
                File.Create(path).Close();
                File.WriteAllBytes(path, file.bytes);
            }
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
