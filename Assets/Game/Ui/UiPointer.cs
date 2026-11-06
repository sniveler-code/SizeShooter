using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SizeShooter.Ui
{
    public static class UiPointer
    {
        public static VisualElement Root;

        public static bool IsOverUi()
        {
            Pointer pointer = Pointer.current;
            return pointer != null && IsOverScreenPoint(pointer.position.ReadValue());
        }

        public static bool IsOverScreenPoint(Vector2 screenPoint)
        {
            VisualElement root = Root;
            if (root?.panel == null) return false;

            Vector2 panelPoint = RuntimePanelUtils.ScreenToPanel(root.panel, screenPoint);
            VisualElement picked = root.panel.Pick(panelPoint);
            return picked != null && picked != root && root.Contains(picked);
        }
    }
}
