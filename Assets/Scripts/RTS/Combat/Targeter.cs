using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; } = null;
        public bool hasTarget { get; private set; } = false;
        
        #region Server
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
        #endregion

        #region Client

        

        #endregion
    }
}
