using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform spawnPoint;

        #region Server

        [Command]
        private void CmdSpawnUnit()
        {
            var unitInstance = Instantiate(
                unitPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );
            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            
            CmdSpawnUnit();
        }

        #endregion
    }
}
