using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PauseMenu
{
    internal enum Setting
    {
        Width, Height, Fullscreen, VSync, Fps, Preset, Brightness, FieldOfView, ShowFps, Ambient, Shadows, Master, Music, Effects
    }

    internal sealed class SettingsModel
    {
        private const string Prefix = "PauseMenu.";
        private readonly Dictionary<Setting, float> defaults;
        private readonly Dictionary<Setting, float> values = new();
        public event Action<Setting> Changed;
        public Vector2Int[] Resolutions { get; }
        public static readonly int[] FpsLimits = { -1, 30, 60, 120, 144, 240 };

        public SettingsModel()
        {
            var current = new Vector2Int(Screen.width, Screen.height);
            Resolutions = Screen.resolutions.Select(r => new Vector2Int(r.width, r.height))
                .Append(current).Distinct().OrderBy(r => r.x).ThenBy(r => r.y).ToArray();
            var pipeline = QualitySettings.renderPipeline as UniversalRenderPipelineAsset
                ?? UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            defaults = new()
            {
                [Setting.Width] = current.x,
                [Setting.Height] = current.y,
                [Setting.Fullscreen] = Screen.fullScreen ? 1 : 0,
                [Setting.VSync] = QualitySettings.vSyncCount,
                [Setting.Fps] = Array.IndexOf(FpsLimits, Application.targetFrameRate) >= 0
                    ? Application.targetFrameRate : -1,
                [Setting.Preset] = 2,
                [Setting.Brightness] = 50,
                [Setting.FieldOfView] = 60,
                [Setting.ShowFps] = 0,
                [Setting.Ambient] = RenderSettings.ambientIntensity,
                [Setting.Shadows] = pipeline != null ? pipeline.shadowDistance : QualitySettings.shadowDistance,
                [Setting.Master] = 1,
                [Setting.Music] = 1,
                [Setting.Effects] = 1
            };

            foreach (var entry in defaults)
                values[entry.Key] = PlayerPrefs.GetFloat(Prefix + entry.Key, entry.Value);

            var saved = new Vector2Int((int)this[Setting.Width], (int)this[Setting.Height]);
            if (!Resolutions.Contains(saved))
            {
                values[Setting.Width] = current.x;
                values[Setting.Height] = current.y;
            }
            if (!FpsLimits.Contains((int)this[Setting.Fps])) values[Setting.Fps] = defaults[Setting.Fps];
            values[Setting.Preset] = Mathf.Clamp((int)this[Setting.Preset], 0, 2);
            values[Setting.Brightness] = Mathf.Clamp(this[Setting.Brightness], 0, 100);
            values[Setting.FieldOfView] = Mathf.Clamp(this[Setting.FieldOfView], 45, 90);
            values[Setting.Ambient] = Mathf.Clamp(this[Setting.Ambient], 0, 2);
            values[Setting.Shadows] = Mathf.Clamp(this[Setting.Shadows], 0, 50);
            foreach (var setting in new[] { Setting.Master, Setting.Music, Setting.Effects })
                values[setting] = Mathf.Clamp01(this[setting]);
        }

        public float this[Setting setting] => values[setting];

        public void Set(Setting setting, float value)
        {
            if (values[setting] == value) return;
            Store(setting, value);
            if (setting == Setting.Preset)
            {
                Store(Setting.Shadows, value == 0 ? 14 : value == 1 ? 26 : 50);
                Changed?.Invoke(Setting.Shadows);
            }
            Changed?.Invoke(setting);
        }

        public void SetResolution(Vector2Int resolution)
        {
            Store(Setting.Width, resolution.x);
            Store(Setting.Height, resolution.y);
            Changed?.Invoke(Setting.Width);
        }

        public void Reset()
        {
            foreach (var entry in defaults) Store(entry.Key, entry.Value);
            foreach (var entry in defaults) Changed?.Invoke(entry.Key);
            Save();
        }

        public void Save() => PlayerPrefs.Save();

        private void Store(Setting setting, float value)
        {
            values[setting] = value;
            PlayerPrefs.SetFloat(Prefix + setting, value);
        }
    }
}
