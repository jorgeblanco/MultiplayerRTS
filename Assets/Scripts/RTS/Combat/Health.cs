using System;
using Mirror;
using UnityEngine;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            _currentHealth = maxHealth;
            HqDeathHandler.ServerOnPlayerDie += ServerHandlePlayerDeath;
        }

        public override void OnStopServer()
        {
            HqDeathHandler.ServerOnPlayerDie -= ServerHandlePlayerDeath;
            base.OnStopServer();
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= damageAmount;

            if (_currentHealth > 0) return;
            
            ServerOnDie?.Invoke();
        }

        [Server]
        private void ServerHandlePlayerDeath(int connectionId)
        {
            if (connectionId != connectionToClient.connectionId) return;
            
            DealDamage(_currentHealth);
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}
