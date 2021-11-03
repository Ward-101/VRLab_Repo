using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Network;


     public class NetworkManagerMain : NetworkManager
    {
        int numberOfPlayer = 0;
        public Transform firstPlayer;
        public Transform secondPlayerTransform;


        public GameObject[] startSpawn;
        private SCR_LocomotionController playerController;

        public static NetworkManagerMain instance;
        public override void Awake()
        {
            base.Awake();

            if (instance == null)
                instance = this;
            else
                Destroy(instance.gameObject);
        }
        /// <summary>
        /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Transform start = numPlayers == 0 ? firstPlayer : secondPlayerTransform;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            if (numberOfPlayer == 0)
                playerController = player.GetComponent<SCR_LocomotionController>();
            numberOfPlayer++;
            // player.GetComponent<ControllerKeyBoard>().playerId = numberOfPlayer;
            NetworkServer.AddPlayerForConnection(conn, player);
            StartCoroutine(WaitToSpawn(conn, player));
        }
        IEnumerator WaitToSpawn(NetworkConnection conn, GameObject player)
        {
            yield return new WaitForSeconds(2f);
            playerController.SpawnObecjt(player, player.transform);

        }

        public void SpawnPieces(GameObject piecesToSpaw, GameObject authority, Transform parent)
        {
            playerController.SpawnPiece(piecesToSpaw, authority, parent);
        }

        public void Win(int playerId)
        {
            if (playerId == 0)
                playerController.CmdEndTurn1();
            else
                playerController.CmdEndTurn();
        }
    public void Restart()
    {

    }
    }

