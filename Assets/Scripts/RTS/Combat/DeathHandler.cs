using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    [RequireComponent(typeof(Health))]
    public class DeathHandler : NetworkBehaviour
    {
        private Health _health;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            _health = GetComponent<Health>();
            _health.ServerOnDie += ServerHandleDeath;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            _health.ServerOnDie -= ServerHandleDeath;
        }

        protected virtual void ServerHandleDeath()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}