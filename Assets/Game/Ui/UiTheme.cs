using UnityEngine;

namespace SizeShooter.Ui
{
    public static class UiTheme
    {
        public static readonly Color Dim = new(0.02f, 0.03f, 0.05f, 0.74f);
        public static readonly Color Panel = new(0.07f, 0.09f, 0.13f, 0.94f);
        public static readonly Color Card = new (0.06f, 0.08f, 0.12f, 0.82f);
        public static readonly Color Track = new (0.15f, 0.18f, 0.24f, 1f);
        public static readonly Color Accent = new (0.31f, 0.85f, 0.55f, 1f);
        public static readonly Color Warn = new (0.98f, 0.73f, 0.25f, 1f);
        public static readonly Color Danger = new (0.93f, 0.31f, 0.32f, 1f);
        public static readonly Color Text = new (0.95f, 0.96f, 1f, 1f);
        public static readonly Color Muted = new (0.64f, 0.70f, 0.80f, 1f);
        public static readonly Color Button = new (0.14f, 0.19f, 0.27f, 1f);
        public static readonly Color ButtonHover = new (0.20f, 0.27f, 0.37f, 1f);
        public static readonly Color ButtonPressed = new (0.10f, 0.14f, 0.20f, 1f);

        public const float Margin = 40f;
        public const float Radius = 18f;
        public const float ButtonHeight = 96f;
    }
}
