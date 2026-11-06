using SizeShooter.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SizeShooter.Game
{
    [DisallowMultipleComponent]
    public sealed class GameUi : MonoBehaviour
    {
        [SerializeField] private GameplayController controller;

        private PanelRenderer _renderer;
        private PanelRenderer.UIReloadCallback _onReload;
        private GameHudView _view;
        private bool _warned;

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

        private void Update()
        {
            if (_view == null) return;

            if (controller == null)
            {
                if (_warned) return;
                _warned = true;
                return;
            }

            GameModel model = controller.Model;
            if (model == null)
                return;

            _view.SetHealth(model.HealthNormalized, model.CriticalNormalized, model.Health, model.HealthMax);
            _view.SetDoor(controller.DistanceToDoor);
            _view.SetHint(HintFor(controller.StateName));
            _view.SetResult(model.Result, HasNextScene());
        }

        private void OnUiReload(PanelRenderer renderer, VisualElement root)
        {
            if (root == null) return;

            _view = new GameHudView(root);
            _view.RestartRequested += ReloadLevel;
            _view.NextRequested += OpenNextLevel;
            _view.MenuRequested += OpenMenu;

            UiPointer.Root = root;
        }

        private static string HintFor(string stateName)
        {
            switch (stateName)
            {
                case "IdleState": return "HOLD TO CHARGE";
                case "ChargingState": return "RELEASE TO FIRE";
                case "FiringState": return "FIRING";
                case "ExplosionState": return "BOOM";
                case "MovingState": return "MOVING";
                default: return string.Empty;
            }
        }

        private static void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private static void OpenNextLevel()
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            if (next < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(next);
        }

        private static void OpenMenu() => SceneManager.LoadScene(0);

        private static bool HasNextScene()
        {
            return SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;
        }
    }
}
