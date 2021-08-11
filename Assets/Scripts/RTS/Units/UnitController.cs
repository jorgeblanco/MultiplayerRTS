using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Units
{
    public class UnitController : NetworkBehaviour
    {
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public UnitMovement UnitMovement { get; private set; } = null;

        private void Awake()
        {
            UnitMovement = GetComponent<UnitMovement>();
        }

        #region Server

        

        #endregion
        #region Client

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
