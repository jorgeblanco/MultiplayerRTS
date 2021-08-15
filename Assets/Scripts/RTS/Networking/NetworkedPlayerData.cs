using System.Collections.Generic;
using Mirror;
using RTS.Units;
using TMPro;
using UnityEngine;

namespace RTS.Networking
{
    public class NetworkedPlayerData : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
        [SerializeField]
        private string playerName;
        [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
        [SerializeField]
        private Color playerColor;

        public List<UnitController> Units { get; } = new List<UnitController>();

        [SerializeField] private TextMeshProUGUI displayNameText;
        // [SerializeField] private MeshRenderer meshRenderer;
    
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            UnitController.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            UnitController.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            UnitController.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            UnitController.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }

        private void ServerHandleUnitSpawned(UnitController unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            Units.Add(unit);
        }

        private void ServerHandleUnitDespawned(UnitController unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            Units.Remove(unit);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            playerName = displayName;
        }

        [Server]
        public void SetDisplayColor(Color color)
        {
            playerColor = color;
        }

        [Command]
        private void CmdSetDisplayName(string newName)
        {
            if (newName.Length < 5)
            {
                Debug.Log($"'{newName}' is too short");
                return;
            }
            SetDisplayName(newName);
        }
        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!isClientOnly) return;
            UnitController.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            UnitController.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!isClientOnly) return;
            UnitController.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            UnitController.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        private void AuthorityHandleUnitSpawned(UnitController unit)
        {
            if (!hasAuthority) return;
            Units.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(UnitController unit)
        {
            if (!hasAuthority) return;
            Units.Remove(unit);
        }
        
        private void HandleDisplayColorUpdated(Color prevColor, Color nextColor)
        {
            // meshRenderer.material.SetColor(BaseColor, nextColor);
        }

        private void HandleDisplayNameUpdated(string prevName, string nextName)
        {
            displayNameText.SetText(nextName);
        }

        [ContextMenu("Set my name")]
        public void SetPlayerName()
        {
            CmdSetDisplayName("My");
        }
    
        [ClientRpc]
        private void RpcKillPlayer()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
