using Mirror;
using RTS.GameManagement;
using UnityEngine;

namespace RTS.Networking
{
    public class NetworkManagerBasics : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        [SerializeField] private GameObject gameOverHandlerPrefab;

        private GameObject _gameOverHandlerInstance;
        
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

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            if (FindObjectOfType<PlayableScene>() == null) return;
            
            _gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(_gameOverHandlerInstance.gameObject);
        }
    }
}
