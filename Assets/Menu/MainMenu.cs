using SizeShooter.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SizeShooter.Menu
{
    [DisallowMultipleComponent]
    public sealed class MainMenu : MonoBehaviour
    {
        private PanelRenderer _renderer;
        private PanelRenderer.UIReloadCallback _onReload;
        private MenuView _view;

        private void Awake()
        {
            _renderer = UiPanel.Attach(gameObject);
            _onReload = OnUiReload;
            _renderer.RegisterUIReloadCallback(_onReload);
        }

        private void OnDestroy()
        {
            if (_renderer != null)
                _renderer.UnregisterUIReloadCallback(_onReload);
        }

        private void OnUiReload(PanelRenderer renderer, VisualElement root)
        {
            if (root == null)
                return;

            _view = new MenuView(root);
            _view.LevelSelected += Open;

            UiPointer.Root = root;
            Debug.Log("UI_MENU_READY panel=" + (root.panel != null));
        }

        private static void Open(int buildIndex)
        {
            if (buildIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(buildIndex);
        }
    }
}
