using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkManagerMain : NetworkManager
{
    int numberOfPlayer = 0;
    public Transform firstPlayer;
    public Transform secondPlayer;


    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = numPlayers == 0 ? firstPlayer : secondPlayer;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        numberOfPlayer++;
        // player.GetComponent<ControllerKeyBoard>().playerId = numberOfPlayer;
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
