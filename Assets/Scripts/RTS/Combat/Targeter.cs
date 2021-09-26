using Mirror;
using RTS.GameManagement;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; } = null;
        public bool hasTarget { get; private set; } = false;
        
        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
            base.OnStopServer();
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out var target)) return;
            Target = target;
            hasTarget = true;
        }

        [Server]
        public void ClearTarget()
        {
            Target = null;
            hasTarget = false;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }
        #endregion

        #region Client

        

        #endregion
    }
}
