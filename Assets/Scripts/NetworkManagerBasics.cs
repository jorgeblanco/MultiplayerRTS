using Mirror;
using UnityEngine;

public class NetworkManagerBasics : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn.identity.GetComponent<NetworkedPlayerData>();
        player.SetDisplayName($"Player {numPlayers}");
        player.SetDisplayColor(new Color(Random.value, Random.value, Random.value));
    }
}
