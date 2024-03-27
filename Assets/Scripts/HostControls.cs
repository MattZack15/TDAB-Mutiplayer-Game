using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostControls : NetworkBehaviour
{
    public bool GameStarted = false;

    [SerializeField] PlayerBoards PlayerBoards;

    public void StartGame()
    {
        if (GameStarted) return;
        if (!IsServer) return;


        int playerCount = NetworkManager.ConnectedClientsIds.Count;

        //playerCount = 9;

        PlayerBoards.SpawnAllGridsRPC(playerCount);

        GameStarted = true;

    }
}
