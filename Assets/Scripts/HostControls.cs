using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HostControls : NetworkBehaviour
{
    public bool GameStarted = false;

    [SerializeField] PlayerBoardsGenerator PlayerBoardsGenerator;

    [SerializeField] private PlayerHealthManager PlayerHealthManager;
    [SerializeField] private PlayerHealthUI PlayerHealthUI;
    [SerializeField] private ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] private Shop shop;
    [SerializeField] private GamePhaseManager GamePhaseManager;

    List<ulong> playersReady = new List<ulong>();

    public void StartGame()
    {
        if (GameStarted) return;
        if (!IsServer)
        {
            print("You are not Server");
            return;
        };


        //int playerCount = NetworkManager.ConnectedClientsIds.Count;

        //playerCount = 9;

        // Spawn in the boards
        PlayerBoardsGenerator.SpawnPlayerBoards();
        // Init Player Health
        PlayerHealthManager.InitPlayerHealth();
        // Show Health UI
        //PlayerHealthUI.GeneratePlayerHealthUI();
        // Server Side Player Data
        ServerPlayerDataManager.Init(NetworkManager.Singleton.ConnectedClientsIds.ToList());

        StartCoroutine(WaitBoardSpawnsOnClients());
        GameStarted = true;

    }

    [Rpc(SendTo.Server)]
    public void SignalBoardsSpawnedServerRpc(ulong clientID)
    {
        if (!playersReady.Contains(clientID))
        {
            playersReady.Add(clientID);
        }
    }

    IEnumerator WaitBoardSpawnsOnClients()
    {
        playersReady = new List<ulong>();
        while (playersReady.Count < NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            yield return null;
        }

        OnGameStart();
    }

    private void OnGameStart()
    {
        // Set Defautl Paths
        foreach (PathManager pathManager in FindObjectsOfType<PathManager>())
        {
            pathManager.CreateDefaultPath();
        }

        GamePhaseManager.StartGame();

    }
}
