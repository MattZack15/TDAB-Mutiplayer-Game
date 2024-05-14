using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostControls : NetworkBehaviour
{
    public bool GameStarted = false;

    [SerializeField] PlayerBoardsGenerator PlayerBoardsGenerator;

    [SerializeField] private PlayerHealthManager PlayerHealthManager;
    [SerializeField] private PlayerHealthUI PlayerHealthUI;


    public void StartGame()
    {
        if (GameStarted) return;
        if (!IsServer) return;


        //int playerCount = NetworkManager.ConnectedClientsIds.Count;

        //playerCount = 9;

        // Spawn in the boards
        PlayerBoardsGenerator.SpawnPlayerBoards();
        // Init Player Health
        PlayerHealthManager.InitPlayerHealth();
        // Show Health UI
        PlayerHealthUI.GeneratePlayerHealthUI();

        GameStarted = true;

    }
}
