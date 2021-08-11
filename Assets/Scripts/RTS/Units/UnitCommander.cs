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

            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unitController in selectionHandler.SelectedUnits)
            {
                unitController.UnitMovement.CmdMove(point);
            }
        }
    }
}
