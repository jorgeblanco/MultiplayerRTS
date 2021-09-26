using System.Collections.Generic;
using Mirror;
using RTS.GameManagement;
using RTS.Networking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask = new LayerMask();
        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private float selectionAreaSizeThreshold = 1;
        
        private Camera _mainCamera;
        private NetworkedPlayerData _playerData;
        private Vector2 _startPosition;

        public List<UnitController> SelectedUnits { get; } = new List<UnitController>();

        private void Awake()
        {
            _mainCamera = Camera.main;
            // _playerData = NetworkClient.connection.identity.GetComponent<NetworkedPlayerData>();
            UnitController.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            UnitController.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            // Temporary hack until the lobby is set up so the player exists on awake
            if (_playerData == null)
            {
                _playerData = NetworkClient.connection.identity.GetComponent<NetworkedPlayerData>();
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelection();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndSelection();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelection();
            }
        }

        private void StartSelection()
        {
            if (!Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.rightShiftKey.isPressed)
            {
                foreach (var unit in SelectedUnits)
                {
                    unit.Deselect();
                }
                SelectedUnits.Clear();
            }
            
            unitSelectionArea.gameObject.SetActive(true);
            _startPosition = Mouse.current.position.ReadValue();
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var areaWidth = mousePosition.x - _startPosition.x;
            var areaHeight = mousePosition.y - _startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void EndSelection()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.sqrMagnitude <= selectionAreaSizeThreshold)
            {
                var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;
                if (!hit.collider.TryGetComponent<UnitController>(out var unitController)) return;
                if (!unitController.hasAuthority) return;
                
                SelectedUnits.Add(unitController);

            }
            else
            {
                var min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
                var max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
                foreach (var unit in _playerData.Units)
                {
                    if (SelectedUnits.Contains(unit)) continue;
                    
                    var screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
                    if (
                        screenPosition.x < min.x
                        || screenPosition.y < min.y
                        || screenPosition.x > max.x
                        || screenPosition.y > max.y
                    ) continue;
                    
                    SelectedUnits.Add(unit);
                }
            }
            
            foreach (var unit in SelectedUnits)
            {
               unit.Select();
            }
        }

        private void AuthorityHandleUnitDespawned(UnitController unitController)
        {
            SelectedUnits.Remove(unitController);
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }
    }
}
