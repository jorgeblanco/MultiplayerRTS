using System.Collections.Generic;
using Mirror;
using RTS.Units;
using UnityEngine;

namespace RTS.GameManagement
{
    public class GameOverHandler : NetworkBehaviour
    {
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
            base.OnStopServer();
            HqController.ServerOnHqSpawned -= ServerHandleBaseSpawned;
            HqController.ServerOnHqDespawned -= ServerHandleBaseDespawned;
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

            if (_hqs.Count > 1) return;
            Debug.Log("Game Over");
        }

        #endregion

        #region Client

        

        #endregion
    }
}