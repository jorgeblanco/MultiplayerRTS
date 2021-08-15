using System;
using Mirror;
using RTS.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Units
{
    public class UnitController : NetworkBehaviour
    {
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public static event Action<UnitController> ServerOnUnitSpawned;
        public static event Action<UnitController> ServerOnUnitDespawned;
        public static event Action<UnitController> AuthorityOnUnitSpawned;
        public static event Action<UnitController> AuthorityOnUnitDespawned;
        
        public UnitMovement UnitMovement { get; private set; } = null;
        public Targeter Targeter { get; private set; } = null;

        private void Awake()
        {
            UnitMovement = GetComponent<UnitMovement>();
            Targeter = GetComponent<Targeter>();
        }

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnUnitDespawned?.Invoke(this);
        }

        #endregion
        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!hasAuthority || !isClientOnly) return;
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!hasAuthority || !isClientOnly) return;
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            onDeselected?.Invoke();
        }

        #endregion
    }
}
