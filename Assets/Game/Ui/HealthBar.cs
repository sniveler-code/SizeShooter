using UnityEngine;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public sealed class HealthBar
    {
        private readonly VisualElement _fill;
        private readonly VisualElement _marker;
        private readonly Label _valueLabel;

        private float _lastNormalized = -1f;
        private float _lastCritical = -1f;

        public VisualElement Root { get; }

        public HealthBar(float width = 420f, float height = 30f)
        {
            Root = UiFactory.Group();

            VisualElement header = UiFactory.Group(FlexDirection.Row);
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.width = width;

            Label captionLabel = UiFactory.Text("HEALTH", 26f, UiTheme.Muted, FontStyle.Bold);
            _valueLabel = UiFactory.Text("100%", 26f, UiTheme.Text, FontStyle.Bold, TextAnchor.MiddleRight);

            header.Add(captionLabel);
            header.Add(_valueLabel);

            VisualElement track = UiFactory.Solid(UiTheme.Track, height * 0.5f);
            track.style.width = width;
            track.style.height = height;
            track.style.marginTop = 8;
            track.style.overflow = Overflow.Hidden;
            track.pickingMode = PickingMode.Ignore;

            _fill = UiFactory.Solid(UiTheme.Accent, height * 0.5f);
            _fill.style.position = Position.Absolute;
            _fill.style.left = 0;
            _fill.style.top = 0;
            _fill.style.bottom = 0;
            _fill.style.width = Length.Percent(100);

            _marker = UiFactory.Solid(new Color(1f, 1f, 1f, 0.75f));
            _marker.style.position = Position.Absolute;
            _marker.style.top = 0;
            _marker.style.bottom = 0;
            _marker.style.width = 3;
            _marker.style.display = DisplayStyle.None;

            track.Add(_fill);
            track.Add(_marker);

            Root.Add(header);
            Root.Add(track);
        }

        public void Set(float normalized, float critical, float value, float max)
        {
            normalized = Mathf.Clamp01(normalized);
            critical = Mathf.Clamp01(critical);

            if (!Mathf.Approximately(normalized, _lastNormalized))
            {
                _lastNormalized = normalized;
                _fill.style.width = Length.Percent(normalized * 100f);
                _fill.style.backgroundColor = ColorFor(normalized);
                _valueLabel.text = Mathf.CeilToInt(normalized * 100f) + "%";
                _valueLabel.tooltip = value.ToString("0.00") + " / " + max.ToString("0.00");
            }

            if (Mathf.Approximately(critical, _lastCritical)) return;

            _lastCritical = critical;
            _marker.style.left = Length.Percent(critical * 100f);
            _marker.style.display = DisplayStyle.Flex;
        }

        private static Color ColorFor(float normalized)
        {
            return normalized switch
            {
                <= 0.35f => UiTheme.Danger,
                <= 0.6f => Color.Lerp(UiTheme.Warn, UiTheme.Accent, (normalized - 0.35f) / 0.25f),
                _ => UiTheme.Accent
            };
        }
    }
}
