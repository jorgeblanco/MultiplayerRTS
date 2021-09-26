using System;
using System.Collections.Generic;
using Mirror;
using RTS.Units;
using UnityEngine;

namespace RTS.GameManagement
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action ServerOnGameOver;
        public static event Action<string> ClientOnGameOver;
        
        private readonly List<HqController> _hqs = new List<HqController>();
        
        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            HqController.ServerOnHqSpawned += ServerHandleBaseSpawned;
            HqController.ServerOnHqDespawned += ServerHandleBaseDespawned;
        }

        public override void OnStopServer()
        {
            HqController.ServerOnHqSpawned -= ServerHandleBaseSpawned;
            HqController.ServerOnHqDespawned -= ServerHandleBaseDespawned;
            base.OnStopServer();
        }

        [Server]
        private void ServerHandleBaseSpawned(HqController hqController)
        {
            _hqs.Add(hqController);
        }

        [Server]
        private void ServerHandleBaseDespawned(HqController hqController)
        {
            _hqs.Remove(hqController);

            if (_hqs.Count != 1) return;

            var winnerId = _hqs[0].connectionToClient.connectionId;
            RpcGameOver($"Player {winnerId}");
            Debug.Log($"Game Over. Player {winnerId} won!");
            ServerOnGameOver?.Invoke();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion
    }
}