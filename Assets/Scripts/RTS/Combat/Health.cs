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
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= damageAmount;

            if (_currentHealth > 0) return;
            
            ServerOnDie?.Invoke();
            Debug.Log($"Unit died: {gameObject.name}");
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
