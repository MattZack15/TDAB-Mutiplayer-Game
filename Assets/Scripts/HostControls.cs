using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HostControls : NetworkBehaviour
{
    public bool GameStarted = false;

    [SerializeField] PlayerBoardsGenerator PlayerBoardsGenerator;

    [SerializeField] private PlayerHealthUI PlayerHealthUI;
    [SerializeField] private ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] private Shop shop;
    [SerializeField] private GamePhaseManager GamePhaseManager;
    [SerializeField] private ShopPool ShopPool;
    [SerializeField] private GameObject HostButtons;
    [SerializeField] private GameObject LoadingScreen;

    List<ulong> playersReady = new List<ulong>();

    private void Start()
    {

        SignalLoadedSceneServerRpc(NetworkManager.Singleton.LocalClientId);


        if (!IsServer) { return; }
        // To mananage when to Start the game
        UsernameCollector UsernameCollector = FindObjectOfType<UsernameCollector>();
        if (UsernameCollector)
        {
            // Assumes we have loaded into the scence from menu and there is already a lobby
            HostButtons.SetActive(false);
            StartCoroutine(WaitSceneLoadOnClients());
        }
        else
        {
            // Assume we are in debug mode
            // no action needed
            HostButtons.SetActive(true);
            return;
        }

    }


    [Rpc(SendTo.Server)]
    public void SignalLoadedSceneServerRpc(ulong clientID)
    {
        if (!playersReady.Contains(clientID))
        {
            playersReady.Add(clientID);
        }
    }
    IEnumerator WaitSceneLoadOnClients()
    {

        while (playersReady.Count < NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            yield return null;
        }

        // Now we are ready to start preparing game start
        StartGame();
    }


    public void StartGame()
    {
        if (GameStarted) return;
        if (!IsServer)
        {
            print("You are not Server");
            return;
        };


        int playerCount = NetworkManager.ConnectedClientsIds.Count;
        // Init shop with number of players
        ShopPool.Init(playerCount);

        StartCoroutine(WaitBoardSpawnsOnClients());

        // Spawn in the boards
        PlayerBoardsGenerator.SpawnPlayerBoards();

        // Server Side Player Data
        ServerPlayerDataManager.Init(NetworkManager.Singleton.ConnectedClientsIds.ToList());

        
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
        // Empty List from before
        playersReady = new List<ulong>();
        while (playersReady.Count < NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            yield return null;
        }

        OnGameStart();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DisableLoadingScreenRPC()
    {
        LoadingScreen.SetActive(false);
    }

    private void OnGameStart()
    {
        // Set Defautl Paths
        foreach (PathManager pathManager in FindObjectsOfType<PathManager>())
        {
            pathManager.CreateDefaultPath();
        }

        DisableLoadingScreenRPC();
        GamePhaseManager.StartGame();

    }
}
