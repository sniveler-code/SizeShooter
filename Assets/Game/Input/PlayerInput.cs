using System;
using SizeShooter.Ui;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace SizeShooter.Input
{
    [DisallowMultipleComponent]
    public sealed class PlayerInput : MonoBehaviour
    {
        public event Action ChargeStarted;
        public event Action ChargeEnded;

        private InputSystem_Actions _actions;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
            _actions.Player.Charge.started += OnChargeStarted;
            _actions.Player.Charge.canceled += OnChargeCanceled;
            _actions.Player.Enable();
        }

        private void OnEnable() => _actions?.Player.Enable();
        private void OnDisable() => _actions?.Player.Disable();

        private void OnDestroy()
        {
            _actions.Player.Charge.started -= OnChargeStarted;
            _actions.Player.Charge.canceled -= OnChargeCanceled;
            _actions.Dispose();
            _actions = null;
        }

        private void OnChargeStarted(InputAction.CallbackContext context)
        {
            if (IsPointerOverUi()) return;
            ChargeStarted?.Invoke();
        }

        private void OnChargeCanceled(InputAction.CallbackContext context) => ChargeEnded?.Invoke();

        private static bool IsPointerOverUi()
        {
            if (UiPointer.IsOverUi()) return true;

            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null) return false;

            foreach (TouchControl touch in touchscreen.touches)
            {
                if (touch.press.isPressed && UiPointer.IsOverScreenPoint(touch.position.ReadValue()))
                    return true;
            }

            return false;
        }
    }
}
