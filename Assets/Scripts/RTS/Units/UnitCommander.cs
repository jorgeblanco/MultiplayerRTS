using RTS.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler selectionHandler;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;

            if (hit.collider.TryGetComponent<Targetable>(out var targetable))
            {
                if (targetable.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(targetable);
                return;
            }
            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unitController in selectionHandler.SelectedUnits)
            {
                if (unitController.UnitMovement == null) return;
                unitController.UnitMovement.CmdMove(point);
            }
        }

        private void TryTarget(Targetable target)
        {
            foreach (var unitController in selectionHandler.SelectedUnits)
            {
                if (unitController.Targeter == null) return;
                unitController.Targeter.CmdSetTarget(target.gameObject);
            }
        }
    }
}
