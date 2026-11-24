using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace PauseMenu
{
    public sealed class PauseMenuRoot : MonoBehaviour
    {
        private readonly ScreenManager screens = new();
        private MenuView view;
        private SettingsModel model;
        private SettingsController settings;
        private Transform sample;
        private Camera sceneCamera;
        private Rect originalViewport;
        private float frameWindow;
        private int frames;
        private readonly System.Collections.Generic.List<Material> stageMaterials = new();
        private AudioSource music;
        private AudioSource effects;
        private float previousTimeScale;
        private bool gameplayActive;
        private CursorLockMode previousLock;
        private bool previousVisibility;

        private void Awake()
        {
            previousTimeScale = Time.timeScale;
            previousLock = Cursor.lockState;
            previousVisibility = Cursor.visible;

            if (EventSystem.current == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem),
                    typeof(InputSystemUIInputModule));
                eventSystem.transform.SetParent(transform, false);
            }

            music = CreateSource("Música de muestra", 130.81f, 4f, true);
            effects = CreateSource("Sonido de navegación", 440f, 0.11f, false);
            model = new SettingsModel();
            settings = new SettingsController(model, music, effects, Camera.main);
            view = new MenuView(transform, model, Play, OpenOptions, Back, ShowHome, Quit, effects.Play);
            BuildStage();
            ShowHome();
            music.Play();
        }

        private void BuildStage()
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                originalViewport = sceneCamera.rect;
                sceneCamera.transform.position = new Vector3(0, 4.7f, -13);
                sceneCamera.transform.LookAt(new Vector3(0, 1.25f, 2));
            }

            var stage = new GameObject("Banco de pruebas");
            stage.transform.SetParent(transform, false);
            var metal = StageMaterial(new Color32(52, 73, 83, 255), 0.45f);
            var concrete = StageMaterial(new Color32(88, 99, 108, 255), 0.12f);
            var teal = StageMaterial(new Color32(59, 188, 172, 255), 0.42f);
            var orange = StageMaterial(new Color32(226, 156, 66, 255), 0.30f);

            Block(stage.transform, "Suelo", new Vector3(0, -0.23f, 10), new Vector3(24, 0.45f, 52), concrete);
            Block(stage.transform, "Base", new Vector3(0, 0.20f, -1), new Vector3(3.4f, 0.40f, 3.4f), metal);
            Block(stage.transform, "Plataforma", new Vector3(0, 0.44f, -1), new Vector3(2.95f, 0.12f, 2.95f), teal);
            sample = Block(stage.transform, "Rotor", new Vector3(0, 2.13f, -1),
                Vector3.one, teal);
            Block(sample, "Eje", Vector3.zero, new Vector3(0.50f, 2.45f, 0.50f), orange);
            Block(sample, "Contrapeso", Vector3.zero, new Vector3(2.30f, 0.31f, 0.44f), metal);

            for (int i = 0; i < 5; i++)
            {
                float depth = 3.2f + i * 4;
                float side = i % 2 == 0 ? -1 : 1;
                Block(stage.transform, $"Columna {i + 1}", new Vector3(side * 3.7f, 1.4f, depth),
                    new Vector3(1.25f, 2.8f, 1.25f), metal);
                Block(stage.transform, $"Señal {i + 1}", new Vector3(side * 3.7f, 2.79f, depth),
                    new Vector3(1.40f, 0.16f, 1.40f), orange);
                Block(stage.transform, $"Marca {i + 1}", new Vector3(0, 0.012f, depth),
                    new Vector3(1.4f, 0.018f, 0.13f), teal);
            }
        }

        private Material StageMaterial(Color color, float smoothness)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = color;
            material.SetFloat("_Smoothness", smoothness);
            stageMaterials.Add(material);
            return material;
        }

        private static Transform Block(Transform parent, string objectName, Vector3 position,
            Vector3 scale, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = objectName;
            block.transform.SetParent(parent, false);
            block.transform.localPosition = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
            return block.transform;
        }

        private AudioSource CreateSource(string sourceName, float pitch, float seconds, bool loop)
        {
            var sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);
            var source = sourceObject.AddComponent<AudioSource>();
            const int sampleRate = 22050;
            var samples = new float[Mathf.RoundToInt(seconds * sampleRate)];
            for (int i = 0; i < samples.Length; i++)
            {
                float time = (float)i / sampleRate;
                float envelope = Mathf.Sin(Mathf.PI * i / (samples.Length - 1));
                samples[i] = Mathf.Sin(2 * Mathf.PI * pitch * time) * envelope * (loop ? 0.026f : 0.16f);
                if (loop)
                    samples[i] += Mathf.Sin(2 * Mathf.PI * pitch * 1.5f * time) * envelope * 0.012f;
            }
            source.clip = AudioClip.Create(sourceName, samples.Length, 1, sampleRate, false);
            source.clip.SetData(samples, 0);
            source.loop = loop;
            source.playOnAwake = false;
            return source;
        }

        private void Update()
        {
            sample.Rotate(23 * Time.deltaTime, 42 * Time.deltaTime, 0, Space.World);
            UpdateFrameRate();
            if (Keyboard.current?.escapeKey.wasPressedThisFrame != true) return;

            if (screens.Current == view.Options)
            {
                // Si hay una lista desplegada, Escape debe cerrar primero esa lista.
                foreach (var dropdown in GetComponentsInChildren<UnityEngine.UI.Dropdown>())
                {
                    if (dropdown.transform.Find("Dropdown List") == null) continue;
                    dropdown.Hide();
                    return;
                }
                Back();
            }
            else if (screens.Current == view.Pause) Play();
            else if (gameplayActive)
            {
                SetGameplay(false);
                screens.Push(view.Pause);
                effects.Play();
            }
        }

        private void UpdateFrameRate()
        {
            view.PreviewHud.SetActive(screens.Current == view.Options ||
                (gameplayActive && model[Setting.ShowFps] > 0));
            if (!view.PreviewHud.activeSelf) return;
            frameWindow += Time.unscaledDeltaTime;
            frames++;
            if (frameWindow < 0.4f) return;
            var sync = model[Setting.VSync] > 0 ? "  ·  VSync activo" : "";
            view.FrameRate.text = $"{frames / frameWindow:0} FPS{sync}";
            frameWindow = 0;
            frames = 0;
        }

        private void ShowHome()
        {
            screens.Clear();
            SetGameplay(false);
            screens.Push(view.Home);
            model.Save();
        }

        private void Play()
        {
            screens.Clear();
            SetGameplay(true);
            effects.Play();
        }

        private void OpenOptions()
        {
            screens.Push(view.Options);
            view.PreviewHud.SetActive(true);
            effects.Play();
        }

        private void Back()
        {
            screens.Pop();
            view.PreviewHud.SetActive(false);
            model.Save();
            effects.Play();
        }

        private void SetGameplay(bool active)
        {
            gameplayActive = active;
            Time.timeScale = active ? 1 : 0;
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
            view.Hud.SetActive(active);
            view.PreviewHud.SetActive(false);
            if (sceneCamera != null)
                sceneCamera.rect = active ? originalViewport : new Rect(0.40f, 0, 0.60f, 1);
        }

        private void Quit()
        {
            model.Save();
            Application.Quit();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) model?.Save();
        }

        private void OnApplicationQuit() => model?.Save();

        private void OnDestroy()
        {
            if (settings == null) return;
            model.Save();
            settings.Dispose();
            Time.timeScale = previousTimeScale;
            Cursor.lockState = previousLock;
            Cursor.visible = previousVisibility;
            if (sceneCamera != null) sceneCamera.rect = originalViewport;
            foreach (var material in stageMaterials) Destroy(material);
            Destroy(music.clip);
            Destroy(effects.clip);
        }
    }
}
