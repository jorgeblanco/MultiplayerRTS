using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask = new LayerMask();
        
        private Camera _mainCamera;
        public List<UnitController> SelectedUnits { get; } = new List<UnitController>();

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Start selection
                StartSelection();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // End selection
                EndSelection();
            }
        }

        private void StartSelection()
        {
            foreach (var unit in SelectedUnits)
            {
                unit.Deselect();
            }
            SelectedUnits.Clear();
        }

        private void EndSelection()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;
            if (!hit.collider.TryGetComponent<UnitController>(out var unitController)) return;
            if (!unitController.hasAuthority) return;
            
            SelectedUnits.Add(unitController);

            foreach (var unit in SelectedUnits)
            {
               unit.Select();
            }
        }
    }
}
