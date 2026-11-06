using System;
using SizeShooter.Game;
using UnityEngine;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public sealed class GameHudView
    {
        public event Action RestartRequested;
        public event Action NextRequested;
        public event Action MenuRequested;

        private readonly VisualElement _hud;
        private readonly VisualElement _overlay;
        private readonly HealthBar _health;
        private readonly Label _massLabel;
        private readonly Label _doorLabel;
        private readonly Label _hintLabel;
        private readonly Label _resultTitle;
        private readonly Label _resultReason;
        private readonly Button _nextButton;

        private string _lastHint;
        private int _lastDoorKey = int.MinValue;
        private int _lastMassKey = int.MinValue;
        private GameModel.Outcome _lastOutcome = GameModel.Outcome.None;

        public GameHudView(VisualElement root)
        {
            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            UiFactory.FullScreen(root);

            _hud = UiFactory.Group();
            UiFactory.FullScreen(_hud);
            _hud.SetEnabled(true);

            VisualElement healthCard = UiFactory.Card();
            UiFactory.AnchorTopLeft(healthCard, UiTheme.Margin, UiTheme.Margin);
            _health = new HealthBar();
            healthCard.Add(_health.Root);
            _massLabel = UiFactory.Text("mass 0.00", 24f, UiTheme.Muted);
            _massLabel.style.marginTop = 6;
            healthCard.Add(_massLabel);

            VisualElement doorCard = UiFactory.Card();
            UiFactory.AnchorTopRight(doorCard, UiTheme.Margin, UiTheme.Margin);
            doorCard.style.alignItems = Align.FlexEnd;
            doorCard.Add(UiFactory.Text("DOOR", 26f, UiTheme.Muted, FontStyle.Bold, TextAnchor.MiddleRight));
            _doorLabel = UiFactory.Text("0.0 m", 34f, UiTheme.Text, FontStyle.Bold, TextAnchor.MiddleRight);
            _doorLabel.style.marginTop = 6;
            doorCard.Add(_doorLabel);

            _hintLabel = UiFactory.Text(string.Empty, 30f, UiTheme.Muted, FontStyle.Bold, TextAnchor.MiddleCenter);
            _hintLabel.style.position = Position.Absolute;
            _hintLabel.style.left = 0;
            _hintLabel.style.right = 0;
            _hintLabel.style.bottom = 90;
            _hintLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            _hud.Add(healthCard);
            _hud.Add(doorCard);
            _hud.Add(_hintLabel);

            _overlay = UiFactory.Group();
            UiFactory.FullScreen(_overlay);
            _overlay.style.backgroundColor = UiTheme.Dim;
            _overlay.style.alignItems = Align.Center;
            _overlay.style.justifyContent = Justify.Center;

            VisualElement panel = UiFactory.Group();
            panel.style.backgroundColor = UiTheme.Panel;
            panel.style.width = 820;
            panel.style.paddingTop = 56;
            panel.style.paddingBottom = 56;
            panel.style.paddingLeft = 56;
            panel.style.paddingRight = 56;
            panel.style.alignItems = Align.Center;
            UiFactory.SetRadius(panel, 32f);

            _resultTitle = UiFactory.Text("RESULT", 72f, UiTheme.Text, FontStyle.Bold, TextAnchor.MiddleCenter);
            _resultReason = UiFactory.Text(string.Empty, 30f, UiTheme.Muted, FontStyle.Normal, TextAnchor.MiddleCenter);
            _resultReason.style.marginTop = 18;
            _resultReason.style.whiteSpace = WhiteSpace.Normal;
            _resultReason.style.unityTextAlign = TextAnchor.MiddleCenter;
            _resultReason.style.maxWidth = 700;
            _resultReason.style.display = DisplayStyle.None;

            Button restart = UiFactory.Action("RETRY", () => RestartRequested?.Invoke(), 700f);
            restart.style.marginTop = 48;
            _nextButton = UiFactory.Action("NEXT LEVEL", () => NextRequested?.Invoke(), 700f);
            _nextButton.style.marginTop = 16;
            Button menu = UiFactory.Action("MENU", () => MenuRequested?.Invoke(), 700f);
            menu.style.marginTop = 16;

            panel.Add(_resultTitle);
            panel.Add(_resultReason);
            panel.Add(restart);
            panel.Add(_nextButton);
            panel.Add(menu);

            _overlay.Add(panel);
            _overlay.style.display = DisplayStyle.None;

            root.Add(_hud);
            root.Add(_overlay);
        }

        public void SetHealth(float normalized, float critical, float value, float max)
        {
            _health.Set(normalized, critical, value, max);

            int key = Mathf.RoundToInt(value * 100f);
            if (key == _lastMassKey) return;

            _lastMassKey = key;
            _massLabel.text = "mass " + value.ToString("0.00") + " / " + max.ToString("0.00");
        }

        public void SetDoor(float meters)
        {
            int key = Mathf.RoundToInt(meters * 10f);
            if (key == _lastDoorKey) return;

            _lastDoorKey = key;
            _doorLabel.text = Mathf.Max(0f, meters).ToString("0.0") + " m";
        }

        public void SetHint(string hint)
        {
            if (hint == _lastHint) return;
            _lastHint = hint;
            _hintLabel.text = hint;
            _hintLabel.style.display = string.IsNullOrEmpty(hint) ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void SetResult(GameModel.Outcome outcome, bool hasNext)
        {
            if (outcome == _lastOutcome) return;
            _lastOutcome = outcome;

            if (outcome == GameModel.Outcome.None)
            {
                _overlay.style.display = DisplayStyle.None;
                _hud.style.display = DisplayStyle.Flex;
                return;
            }

            bool win = outcome == GameModel.Outcome.Win;
            _resultTitle.text = win ? "WIN" : "LOSS";
            _resultTitle.style.color = win ? UiTheme.Accent : UiTheme.Danger;
            _resultReason.text = ReasonFor(outcome);
            _resultReason.style.display =
                string.IsNullOrEmpty(_resultReason.text) ? DisplayStyle.None : DisplayStyle.Flex;
            _nextButton.style.display = win && hasNext ? DisplayStyle.Flex : DisplayStyle.None;
            _overlay.style.display = DisplayStyle.Flex;
            _hud.style.display = DisplayStyle.None;
        }

        private static string ReasonFor(GameModel.Outcome outcome)
        {
            switch (outcome)
            {
                case GameModel.Outcome.Win: return "The door is open. Path cleared.";
                case GameModel.Outcome.LoseOvercharge: return "Overcharged - you shrank below critical mass.";
                case GameModel.Outcome.LoseDeadEnd: return "Dead end - not enough mass left to clear the path.";
                default: return string.Empty;
            }
        }
    }
}
