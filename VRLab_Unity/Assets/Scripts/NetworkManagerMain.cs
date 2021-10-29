using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkManagerMain : NetworkManager
{
    int numberOfPlayer = 0;
    public Transform firstPlayer;
    public Transform secondPlayerTransform;

    public GameObject[] startSpawn;
    private SCR_LocomotionController playerController;
    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = numPlayers == 0 ? firstPlayer : secondPlayerTransform;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        if(numberOfPlayer==0)
         playerController = player.GetComponent<SCR_LocomotionController>();
        numberOfPlayer++;
        // player.GetComponent<ControllerKeyBoard>().playerId = numberOfPlayer;
        NetworkServer.AddPlayerForConnection(conn, player);
        StartCoroutine(WaitToSpawn(player));
    }
    IEnumerator WaitToSpawn(GameObject player)
    {
        yield return new WaitForSeconds(2f);
        playerController.SpawnObecjt(player, player.transform);
    }
}
