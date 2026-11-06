using UnityEngine;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public static class UiPanel
    {
        private const string _panelResource = "Ui/PanelSettings";
        private const string _themeResource = "Ui/UnityDefaultRuntimeTheme";
        private static PanelSettings _panel;

        private static PanelSettings panel
        {
            get
            {
                if (_panel != null) return _panel;
                _panel = Resources.Load<PanelSettings>(_panelResource);
                if (_panel != null) return _panel;

                _panel = ScriptableObject.CreateInstance<PanelSettings>();
                _panel.name = "RuntimePanelSettings";
                _panel.hideFlags = HideFlags.HideAndDontSave;
                _panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                _panel.referenceResolution = new Vector2Int(1080, 1920);
                _panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                _panel.match = 0.5f;
                _panel.sortingOrder = 10;

                ThemeStyleSheet theme = Resources.Load<ThemeStyleSheet>(_themeResource);
                if (theme != null)
                    _panel.themeStyleSheet = theme;

                return _panel;
            }
        }

        public static PanelRenderer Attach(GameObject host)
        {
            PanelRenderer renderer = host.GetComponent<PanelRenderer>();
            if (renderer == null)
                renderer = host.AddComponent<PanelRenderer>();

            if (renderer.panelSettings == null)
                renderer.panelSettings = panel;

            return renderer;
        }
    }
}
