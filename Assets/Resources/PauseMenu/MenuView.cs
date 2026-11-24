using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PauseMenu
{
    internal sealed class MenuView
    {
        private readonly Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        private readonly Color panelColor = new(0.105f, 0.110f, 0.105f);
        private readonly Color accent = new(0.83f, 0.75f, 0.55f);
        private readonly DefaultControls.Resources resources = new();
        private readonly Transform canvas;
        public PanelScreen Home { get; }
        public PanelScreen Pause { get; }
        public PanelScreen Options { get; }
        public GameObject Hud { get; }
        public Text FrameRate { get; }
        public GameObject PreviewHud { get; }

        public MenuView(Transform parent, SettingsModel model, UnityAction play, UnityAction options,
            UnityAction back, UnityAction home, UnityAction quit, UnityAction testAudio)
        {
            var root = new GameObject("Canvas de menús", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(parent, false);
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1360, 760);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvas = root.transform;

            Home = Menu("Menú principal", ("Jugar", play), ("Configuración", options), ("Salir", quit));
            Pause = Menu("Pausa", ("Continuar", play), ("Configuración", options),
                ("Menú principal", home), ("Salir", quit));

            var overlay = Overlay("Configuración");
            var card = Card(overlay.transform, "Configuración", 500, 668);
            var video = Page(card, "Video");
            var audio = Page(card, "Audio");
            var game = Page(card, "Juego");
            var pages = new[] { video, audio, game };
            var tabNames = new[] { "Video", "Audio", "Juego" };
            var tabs = new Button[pages.Length];
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i;
                tabs[i] = Button(card, tabNames[i], (i - 1) * 144, 211, 136, () => SelectTab(index));
            }

            void SelectTab(int index)
            {
                for (int i = 0; i < pages.Length; i++)
                {
                    bool selected = i == index;
                    pages[i].gameObject.SetActive(selected);
                    tabs[i].GetComponent<Image>().color = selected ? accent : new Color(0.20f, 0.20f, 0.18f);
                    tabs[i].GetComponentInChildren<Text>().color = selected ? panelColor : Color.white;
                }
            }

            var sizes = model.Resolutions;
            var resolution = Dropdown(video, "Resolución", 0, 147,
                sizes.Select(s => $"{s.x} × {s.y}").ToArray(), i => model.SetResolution(sizes[i]));
            BindDropdown(video, "Modo de pantalla", 101, model, Setting.Fullscreen,
                "Ventana", "Pantalla completa");
            BindDropdown(video, "VSync", 55, model, Setting.VSync, "Desactivado", "Activado");
            var fps = Dropdown(video, "Límite de FPS", 0, 9,
                SettingsModel.FpsLimits.Select(i => i < 0 ? "Ilimitado" : i.ToString()).ToArray(),
                i => model.Set(Setting.Fps, SettingsModel.FpsLimits[i]));
            BindDropdown(video, "Calidad gráfica", -37, model, Setting.Preset, "Baja", "Media", "Alta");
            BindSlider(video, "Brillo", -83, 100, model, Setting.Brightness, v => $"{v:0} %", true);
            BindSlider(video, "Distancia de sombras", -129, 50, model, Setting.Shadows, v => $"{v:0} m", true);
            BindSlider(video, "Luz ambiental", -175, 2, model, Setting.Ambient, v => $"{v:0.00}");
            var vsyncHelp = Label(video, "Desactivá VSync para usar el límite de FPS.", 0, -223, 420, 15, accent);

            BindSlider(audio, "Volumen general", 147, 1, model, Setting.Master, v => $"{v * 100:0} %");
            BindSlider(audio, "Música", 101, 1, model, Setting.Music, v => $"{v * 100:0} %");
            BindSlider(audio, "Efectos", 55, 1, model, Setting.Effects, v => $"{v * 100:0} %");
            Button(audio, "Probar efectos", 127, -3, 166, testAudio);

            BindSlider(game, "Campo de visión", 147, 90, model, Setting.FieldOfView, v => $"{v:0}°", true, 45);
            BindDropdown(game, "Mostrar FPS al jugar", 101, model, Setting.ShowFps, "No", "Sí");

            void RefreshDisplay(Setting _)
            {
                var selected = new Vector2Int((int)model[Setting.Width], (int)model[Setting.Height]);
                resolution.SetValueWithoutNotify(Mathf.Max(0, Array.IndexOf(sizes, selected)));
                fps.SetValueWithoutNotify(Mathf.Max(0, Array.IndexOf(SettingsModel.FpsLimits, (int)model[Setting.Fps])));
                fps.interactable = model[Setting.VSync] == 0;
                vsyncHelp.gameObject.SetActive(!fps.interactable);
            }

            model.Changed += RefreshDisplay;
            RefreshDisplay(default);
            SelectTab(0);
            Button(card, "Restaurar valores", -113, -287, 200, model.Reset);
            Button(card, "Volver", 139, -287, 144, back, true);
            Options = new PanelScreen(overlay);
            FrameRate = Label(canvas, "", 0, 0, 330, 19, Color.white);
            var previewRect = FrameRate.rectTransform;
            previewRect.anchorMin = previewRect.anchorMax = Vector2.one;
            previewRect.pivot = Vector2.one;
            previewRect.anchoredPosition = new Vector2(-24, -12);
            FrameRate.alignment = TextAnchor.MiddleRight;
            PreviewHud = FrameRate.gameObject;
            PreviewHud.SetActive(false);
            Hud = Label(canvas, "Esc  ·  Pausa", 0, 0, 190, 20, Color.white).gameObject;
            var hudRect = Hud.GetComponent<RectTransform>();
            hudRect.anchorMin = hudRect.anchorMax = new Vector2(0, 1);
            hudRect.anchoredPosition = new Vector2(112, -37);
        }

        private Transform Page(Transform card, string name)
        {
            var page = new GameObject(name, typeof(RectTransform));
            Place(page, card, 0, 0, 450, 500);
            return page.transform;
        }

        private void BindDropdown(Transform page, string caption, float y, SettingsModel model,
            Setting setting, params string[] items)
        {
            var control = Dropdown(page, caption, 0, y, items, value => model.Set(setting, value));
            void Refresh(Setting _) => control.SetValueWithoutNotify(Mathf.Clamp((int)model[setting], 0, items.Length - 1));
            model.Changed += Refresh;
            Refresh(setting);
        }

        private void BindSlider(Transform page, string caption, float y, float max, SettingsModel model,
            Setting setting, Func<float, string> format, bool wholeNumbers = false, float min = 0)
        {
            var control = Slider(page, caption, 0, y, max, value => model.Set(setting, value), wholeNumbers);
            control.minValue = min;
            var readout = Label(page, "", 185, y, 50, 15, accent);
            readout.alignment = TextAnchor.MiddleRight;

            void Refresh(Setting _)
            {
                control.SetValueWithoutNotify(model[setting]);
                readout.text = format(model[setting]);
            }

            model.Changed += Refresh;
            Refresh(setting);
        }

        private PanelScreen Menu(string title, params (string caption, UnityAction action)[] buttons)
        {
            var overlay = Overlay(title);
            var card = Card(overlay.transform, title, 500, 376);
            for (int i = 0; i < buttons.Length; i++)
                Button(card, buttons[i].caption, 0, 45 - i * 57, 298, buttons[i].action, i == 0);
            return new PanelScreen(overlay);
        }

        private GameObject Overlay(string name)
        {
            var overlay = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = overlay.GetComponent<RectTransform>();
            rect.SetParent(canvas, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(0.40f, 1);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            overlay.GetComponent<Image>().color = panelColor;
            overlay.GetComponent<Image>().raycastTarget = false;
            overlay.SetActive(false);
            return overlay;
        }

        private Transform Card(Transform parent, string title, float width, float height)
        {
            var card = new GameObject(title, typeof(RectTransform), typeof(Image));
            Place(card, parent, 0, 0, width, height);
            var cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = cardRect.anchorMax = new Vector2(0, 0.5f);
            cardRect.anchoredPosition = new Vector2(width / 2 + 24, 0);
            card.GetComponent<Image>().color = panelColor;
            var heading = Label(card.transform, title, 0, height / 2 - 56, width - 80, 34, Color.white);
            heading.fontStyle = FontStyle.Bold;
            return card.transform;
        }

        private Text Label(Transform parent, string value, float x, float y, float width, int size, Color color)
        {
            var go = new GameObject(value, typeof(RectTransform), typeof(Text));
            Place(go, parent, x, y, width, size * 3);
            var label = go.GetComponent<Text>();
            label.font = font;
            label.fontSize = size;
            label.alignment = TextAnchor.MiddleLeft;
            label.color = color;
            label.text = value;
            label.raycastTarget = false;
            return label;
        }

        private Button Button(Transform parent, string caption, float x, float y, float width,
            UnityAction action, bool primary = false)
        {
            var go = DefaultControls.CreateButton(resources);
            go.name = caption;
            Place(go, parent, x, y, width, 43);
            go.GetComponent<Image>().color = primary ? accent : new Color(0.20f, 0.20f, 0.18f);
            var text = go.GetComponentInChildren<Text>();
            text.text = caption;
            text.font = font;
            text.fontSize = 19;
            text.color = primary ? panelColor : Color.white;
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(action);
            return button;
        }

        private Dropdown Dropdown(Transform parent, string caption, float x, float y,
            string[] items, UnityAction<int> changed)
        {
            Label(parent, caption, x, y, 420, 17, Color.white);
            var go = DefaultControls.CreateDropdown(resources);
            go.name = caption;
            Place(go, parent, x + 95, y, 230, 34);
            foreach (var label in go.GetComponentsInChildren<Text>(true))
            {
                label.font = font;
                label.fontSize = 16;
                label.color = panelColor;
            }
            var arrow = go.transform.Find("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(false);
            var indicator = Label(go.transform, "▾", 96, 0, 19, 18, accent);
            indicator.alignment = TextAnchor.MiddleCenter;
            var dropdown = go.GetComponent<Dropdown>();
            go.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.20f);
            dropdown.captionText.color = Color.white;
            dropdown.ClearOptions();
            dropdown.AddOptions(items.ToList());
            dropdown.onValueChanged.AddListener(changed);
            return dropdown;
        }

        private Slider Slider(Transform parent, string caption, float x, float y, float max,
            UnityAction<float> changed, bool wholeNumbers = false)
        {
            Label(parent, caption, x, y, 420, 17, Color.white);
            var go = DefaultControls.CreateSlider(resources);
            go.name = caption;
            Place(go, parent, x + 80, y, 152, 26);
            var slider = go.GetComponent<Slider>();
            slider.maxValue = max;
            slider.wholeNumbers = wholeNumbers;
            slider.fillRect.GetComponent<Image>().color = accent;
            slider.onValueChanged.AddListener(changed);
            return slider;
        }

        private static void Place(GameObject go, Transform parent, float x, float y, float width, float height)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(x, y);
        }
    }
}
