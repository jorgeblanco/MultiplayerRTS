using Mirror;
using UnityEngine;

namespace RTS.Networking
{
    public class NetworkManagerBasics : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<NetworkedPlayerData>();
            player.SetDisplayName($"Player {numPlayers}");
            player.SetDisplayColor(new Color(Random.value, Random.value, Random.value));

            var playerTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab,
                playerTransform.position,
                playerTransform.rotation
            );
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}
