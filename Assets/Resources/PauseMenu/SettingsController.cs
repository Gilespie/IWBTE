using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PauseMenu
{
    internal sealed class SettingsController : IDisposable
    {
        private readonly SettingsModel model;
        private readonly AudioSource music;
        private readonly AudioSource effects;
        private UniversalRenderPipelineAsset shadowAsset;
        private float originalShadowDistance;
        private float originalRenderScale;
        private int originalSamples;
        private readonly Camera camera;
        private readonly UniversalAdditionalCameraData cameraData;
        private readonly bool previousPostProcessing;
        private readonly float previousFieldOfView;
        private readonly VolumeProfile profile;
        private readonly Volume volume;
        private readonly ColorAdjustments exposure;
        private readonly AmbientMode previousAmbientMode = RenderSettings.ambientMode;
        private readonly Color previousAmbientLight = RenderSettings.ambientLight;

        public SettingsController(SettingsModel model, AudioSource music, AudioSource effects, Camera camera)
        {
            this.model = model;
            this.music = music;
            this.effects = effects;
            this.camera = camera;
            if (camera != null)
            {
                previousFieldOfView = camera.fieldOfView;
                cameraData = camera.GetUniversalAdditionalCameraData();
                previousPostProcessing = cameraData.renderPostProcessing;
                cameraData.renderPostProcessing = true;
                var exposureObject = new GameObject("Brillo de pantalla");
                exposureObject.transform.SetParent(camera.transform, false);
                volume = exposureObject.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.priority = 100;
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                volume.sharedProfile = profile;
                exposure = profile.Add<ColorAdjustments>(false);
            }

            model.Changed += Apply;
            Apply(Setting.Preset);
            Apply(Setting.Brightness);
            Apply(Setting.FieldOfView);
            Apply(Setting.Width);
            Apply(Setting.Shadows);
            Apply(Setting.VSync);
            Apply(Setting.Ambient);
            Apply(Setting.Master);
            Apply(Setting.Music);
            Apply(Setting.Effects);
        }

        private void Apply(Setting setting)
        {
            switch (setting)
            {
                case Setting.Width:
                case Setting.Height:
                case Setting.Fullscreen:
                    Screen.SetResolution((int)model[Setting.Width], (int)model[Setting.Height],
                        model[Setting.Fullscreen] > 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
                    break;
                case Setting.VSync:
                case Setting.Fps:
                    QualitySettings.vSyncCount = Mathf.Clamp((int)model[Setting.VSync], 0, 4);
                    Application.targetFrameRate = (int)model[Setting.Fps];
                    break;
                case Setting.Ambient:
                    RenderSettings.ambientMode = AmbientMode.Flat;
                    RenderSettings.ambientLight = new Color(0.38f, 0.49f, 0.62f) * (model[setting] * 0.7f);
                    break;
                case Setting.Brightness:
                    if (exposure != null) exposure.postExposure.Override((model[setting] - 50f) * 0.04f);
                    break;
                case Setting.FieldOfView:
                    if (camera != null) camera.fieldOfView = model[setting];
                    break;
                case Setting.Preset:
                case Setting.Shadows:
                    var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if (pipeline == null) break;
                    if (shadowAsset != pipeline)
                    {
                        RestoreShadowDistance();
                        shadowAsset = pipeline;
                        originalShadowDistance = pipeline.shadowDistance;
                        originalRenderScale = pipeline.renderScale;
                        originalSamples = pipeline.msaaSampleCount;
                    }
                    int preset = (int)model[Setting.Preset];
                    shadowAsset.renderScale = preset == 0 ? 0.65f : preset == 1 ? 0.85f : 1f;
                    shadowAsset.msaaSampleCount = preset == 0 ? 1 : preset == 1 ? 2 : 4;
                    shadowAsset.shadowDistance = model[Setting.Shadows];
                    break;
                case Setting.Master:
                    AudioListener.volume = model[setting];
                    break;
                case Setting.Music:
                    if (music != null) music.volume = model[setting];
                    break;
                case Setting.Effects:
                    if (effects != null) effects.volume = model[setting];
                    break;
            }
        }

        private void RestoreShadowDistance()
        {
            if (shadowAsset == null) return;
            shadowAsset.shadowDistance = originalShadowDistance;
            shadowAsset.renderScale = originalRenderScale;
            shadowAsset.msaaSampleCount = originalSamples;
            shadowAsset = null;
        }

        public void Dispose()
        {
            model.Changed -= Apply;
            RenderSettings.ambientMode = previousAmbientMode;
            RenderSettings.ambientLight = previousAmbientLight;
            RestoreShadowDistance();
            if (camera != null) camera.fieldOfView = previousFieldOfView;
            if (cameraData != null) cameraData.renderPostProcessing = previousPostProcessing;
            if (volume != null) UnityEngine.Object.Destroy(volume.gameObject);
            if (profile != null)
            {
                UnityEngine.Object.Destroy(exposure);
                UnityEngine.Object.Destroy(profile);
            }
        }
    }
}
