using System;
using System.Collections.Generic;
using Mechanics.UI.Menus.Menu_Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Settings Menu")]
    public class SettingsMenu : BaseMenuController
    {

        #region Inspector

        [Header("Settings Menu UI")]
        [SerializeField]
        private Slider audioMasterSlider;

        [SerializeField]
        private Slider audioMusicSlider;

        [SerializeField]
        private Slider audioEffectsSlider;
        
        [SerializeField]
        private Slider audioAmbientSlider;

        [SerializeField]
        private Toggle vSyncToggle;

        [SerializeField]
        private Toggle fullscreenToggle;

        [SerializeField]
        private TMP_Dropdown anisDropdown;

        [SerializeField]
        private TMP_Dropdown aaDropdown;

        [SerializeField]
        private TMP_Text presetLabel;

        [SerializeField]
        private TMP_Text resolutionLabel;

        #endregion

        #region BaseMenuManager

        public override void OpenMenu()
        {
            base.OpenMenu(); // TODO: Add MipMap to settings!
            // QualitySettings.globalTextureMipmapLimit
            UIFromQuality();
        }

        private void UIFromQuality()
        {
            vSyncToggle.isOn = QualitySettings.vSyncCount >= 1;
            fullscreenToggle.isOn = Screen.fullScreen;
            anisDropdown.value = QualitySettings.anisotropicFiltering switch
            {
                AnisotropicFiltering.Disable => 0,
                AnisotropicFiltering.Enable => 1,
                AnisotropicFiltering.ForceEnable => 2,
                _ => anisDropdown.value
            };

            aaDropdown.value = QualitySettings.antiAliasing switch
            {
                0 => 0,
                2 => 1,
                4 => 2,
                8 => 3,
                _ => aaDropdown.value
            };

            presetLabel.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
            resolutionLabel.text = Screen.currentResolution.ToString();
        }

        public override void CloseMenu()
        {
            if (menuUiObject.activeSelf)
            {
                SaveSettings();
            }

            base.CloseMenu();
        }

        #endregion

        #region Inputs

        public void ToggleVsync(bool state)
        {
            QualitySettings.vSyncCount = state ? 1 : 0;
        }

        public void UpdateTextureQuality(float quality)
        {
            QualitySettings.globalTextureMipmapLimit = Mathf.RoundToInt(quality);
        }

        public void SetFullScreen(bool state)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, state);
        }

        public void UpdateAnisotropicFiltering(int anisoSetting)
        {
            QualitySettings.anisotropicFiltering = anisoSetting switch
            {
                0 => AnisotropicFiltering.Disable,
                1 => AnisotropicFiltering.Enable,
                2 => AnisotropicFiltering.ForceEnable,
                _ => QualitySettings.anisotropicFiltering
            };
        }
        
        public void UpdateMipmapLimit(int mapRes)
        {
            QualitySettings.globalTextureMipmapLimit = mapRes;
        }

        public void UpdateMSAA(int msaaAmount)
        {
            QualitySettings.antiAliasing = msaaAmount switch
            {
                0 => 0,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => QualitySettings.antiAliasing
            };
        }

        public void IncreaseQualityPreset()
        {
            QualitySettings.IncreaseLevel(true);
            UIFromQuality();
        }

        public void DecreaseQualityPreset()
        {
            QualitySettings.DecreaseLevel(true);
            UIFromQuality();
        }

        public void IncreaseResolution()
        {
            if (_currentResolutionIndex >= _supportedResolutions.Count - 1)
            {
                return;
            }

            _currentResolutionIndex++;
            ApplyResolution();
        }

        public void DecreaseResolution()
        {
            if (_currentResolutionIndex <= 0)
            {
                return;
            }

            _currentResolutionIndex--;
            ApplyResolution();
        }

        #endregion

        #region Private Fields

        private int _currentResolutionIndex;
        private List<Resolution> _supportedResolutions;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            _supportedResolutions = new List<Resolution>(Screen.resolutions);
            LoadSettings();
            _currentResolutionIndex = _supportedResolutions.FindIndex(
                resolution => resolution.width == Screen.currentResolution.width &&
                              resolution.height == Screen.currentResolution.height
            );
        }

        #endregion

        #region Private Methods

        private void SaveSettings()
        {
            var settings = SettingsSaver.GetSettingsState();
            settings.musicVolume = audioMusicSlider.value;
            settings.masterVolume = audioMasterSlider.value;
            settings.sfxVolume = audioEffectsSlider.value;
            settings.ambientVolume = audioAmbientSlider.value;
            SettingsSaver.SaveSettings(settings);
        }

        private void LoadSettings()
        {
            var settings = SettingsSaver.LoadSettings();
            QualitySettings.SetQualityLevel(settings.qualityLevelIndex);
            QualitySettings.vSyncCount = settings.vSyncCount;
            Screen.fullScreen = settings.fullscreen;
            QualitySettings.anisotropicFiltering = settings.anisotropicFiltering switch
            {
                0 => AnisotropicFiltering.Disable,
                1 => AnisotropicFiltering.Enable,
                2 => AnisotropicFiltering.ForceEnable,
                _ => QualitySettings.anisotropicFiltering
            };

            QualitySettings.antiAliasing = settings.antiAliasing switch
            {
                0 => 0,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => QualitySettings.antiAliasing
            };

            bool Match(Resolution resolution) => resolution.width == settings.resolutionWidth && resolution.height == settings.resolutionHeight;
            _currentResolutionIndex = _supportedResolutions.FindIndex(Match);
            ApplyResolution();
            
            audioMasterSlider.value = settings.masterVolume;
            audioMusicSlider.value = settings.musicVolume;
            audioEffectsSlider.value = settings.sfxVolume;
            audioAmbientSlider.value = settings.ambientVolume;
            // TODO: add globalMipmap slider
        }

        private void ApplyResolution()
        {
            if (_currentResolutionIndex < 0 || _currentResolutionIndex > _supportedResolutions.Count)
            {
                _currentResolutionIndex = _supportedResolutions.FindIndex(
                    resolution => resolution.width == Screen.currentResolution.width &&
                                  resolution.height == Screen.currentResolution.height
                );
            }

            var resolution = _supportedResolutions[_currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            resolutionLabel.text = resolution.ToString();
            // if (Screen.fullScreen)
            // {
            //     Screen.SetResolution(resolution.width, resolution.height, true);
            // }
            // else
            // {
            //     // Calculate the current window position and size
            //     Rect currentWindowRect = new Rect(Screen.currentResolution.width * 0.5f - Screen.width * 0.5f, Screen.currentResolution.height * 0.5f - Screen.height * 0.5f, Screen.width, Screen.height);
            //
            //     // Change the resolution
            //     Screen.SetResolution(resolution.width, resolution.height, false);
            //
            //     // Calculate the new window position and size based on the new resolution
            //     float newWindowPosX = currentWindowRect.x + (currentWindowRect.width - Screen.width) * 0.5f;
            //     float newWindowPosY = currentWindowRect.y + (currentWindowRect.height - Screen.height) * 0.5f;
            //     Rect newWindowRect = new Rect(newWindowPosX, newWindowPosY, Screen.width, Screen.height);
            //
            //     // Set the new window position and size
            //     Screen.SetResolution(resolution.width, resolution.height, false, Mathf.RoundToInt(newWindowRect.x), Mathf.RoundToInt(newWindowRect.y));
            // }
        }

        #endregion

    }
}
