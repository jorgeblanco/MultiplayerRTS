using System;
using Mirror;

namespace RTS.Units
{
    public class HqController : NetworkBehaviour
    {
        public static event Action<HqController> ServerOnHqSpawned;
        public static event Action<HqController> ServerOnHqDespawned;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnHqSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerOnHqDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    

    #endregion
    }
}