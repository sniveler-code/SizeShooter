using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public sealed class MenuView
    {
        public event Action<int> LevelSelected;

        public MenuView(VisualElement root)
        {
            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            UiFactory.FullScreen(root);
            root.style.alignItems = Align.Center;
            root.style.justifyContent = Justify.Center;

            Label title = UiFactory.Text("SIZE SHOOTER", 110f, UiTheme.Text, FontStyle.Bold, TextAnchor.MiddleCenter);
            Label subtitle = UiFactory.Text("charge, shrink, clear the path", 32f, UiTheme.Muted, FontStyle.Normal,
                TextAnchor.MiddleCenter);
            subtitle.style.marginTop = 12;

            root.Add(title);
            root.Add(subtitle);
            root.Add(UiFactory.Spacer(90));

            int levels = Mathf.Max(0, SceneManager.sceneCountInBuildSettings - 1);
            for (int i = 0; i < levels; i++)
            {
                int buildIndex = i + 1;
                Button button = UiFactory.Action(LevelLabel(buildIndex), () => LevelSelected?.Invoke(buildIndex), 720f);
                button.style.marginBottom = 20;
                root.Add(button);
            }

            Label footer = UiFactory.Text("hold to charge - release to fire", 26f,
                UiTheme.Muted, FontStyle.Normal, TextAnchor.MiddleCenter);

            footer.style.marginTop = 40;
            root.Add(footer);
        }

        private static string LevelLabel(int buildIndex)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            if (string.IsNullOrEmpty(path)) return "LEVEL " + buildIndex.ToString("00");

            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            return name.Replace('_', ' ').ToUpperInvariant();
        }
    }
}
