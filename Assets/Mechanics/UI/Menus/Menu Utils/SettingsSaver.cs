using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Android;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public static class SettingsSaver
    {

        public static readonly string SettingsSaveFileName = "Settings.json";

        public static void SaveSettings(SettingsState settings)
        {
            var savePath = Path.Combine(Application.persistentDataPath, SettingsSaveFileName);
            var json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(savePath, json);
            Logger.Log($"Saved settings file to {savePath}", Color.magenta);
        }

        public static SettingsState LoadSettings()
        {
            var savePath = Path.Combine(Application.persistentDataPath, SettingsSaveFileName);
            if (File.Exists(savePath))
            {
                var json = File.ReadAllText(savePath);
                var settings = JsonUtility.FromJson<SettingsState>(json);
                Logger.Log($"Loaded Settings file from {savePath}", Color.magenta);
                return settings;
            }

            Logger.Log($"Settings file not found - Creating default", Color.magenta);
            var newSettings = GetSettingsState();
            SaveSettings(newSettings);
            return newSettings;
        }

        public static SettingsState GetSettingsState()
        {
            var newSettings = new SettingsState
            {
                vSyncCount = QualitySettings.vSyncCount,
                fullscreen = Screen.fullScreen,
                qualityLevelIndex = QualitySettings.GetQualityLevel(),
                resolutionHeight = Screen.currentResolution.height,
                resolutionWidth = Screen.currentResolution.width,
                anisotropicFiltering = QualitySettings.anisotropicFiltering switch
                {
                    AnisotropicFiltering.Disable => 0,
                    AnisotropicFiltering.Enable => 1,
                    AnisotropicFiltering.ForceEnable => 2,
                    _ => 0
                },
                antiAliasing = QualitySettings.antiAliasing switch
                {
                    0 => 0,
                    2 => 1,
                    4 => 2,
                    8 => 3,
                    _ => 0
                },
                masterVolume = 0.5f,
                musicVolume = 0.5f,
                sfxVolume = 0.5f,
                ambientVolume = 0.5f,
            };
            return newSettings;
        }
    }

    [Serializable]
    public struct SettingsState
    {
        public int vSyncCount;
        public bool fullscreen;
        public int anisotropicFiltering;
        public int antiAliasing;
        public int qualityLevelIndex;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float ambientVolume;
        public int resolutionWidth;
        public int resolutionHeight;
        // public int resolutionRefreshRate;  // TODO: Save also this
    }
}
