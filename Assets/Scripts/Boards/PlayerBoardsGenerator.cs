using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBoardsGenerator : NetworkBehaviour
{
    [SerializeField] private HexagonGridGenerator HexagonGridGenerator;
    [SerializeField] private Vector2 spacing;

    [SerializeField] GameObject PlayerBoardPrefab;
    [SerializeField] HostControls HostControls;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;

    public void SpawnPlayerBoards()
    {
        if (!IsServer) return;
        
        // Server spawns the player board server objects

        int playerCount = NetworkManager.ConnectedClientsIds.Count;

        int i = 0;
        while (i < playerCount)
        {
            GameObject board = Instantiate(PlayerBoardPrefab);
            board.GetComponent<NetworkObject>().Spawn();
            board.GetComponent<PlayerBoard>().owner.Value = NetworkManager.ConnectedClientsIds[i];
            // board.name = $"Board {NetworkManager.ConnectedClientsIds[i]}";
            

            i++;
        }

        // Then calls the rpc
        SpawnAllGridsRPC(playerCount);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnAllGridsRPC(int playerCount)
    {
        StartCoroutine(WaitForPlayerBoards(playerCount));
    }

    IEnumerator WaitForPlayerBoards(int playerCount)
    {
        PlayerBoard[] PlayerBoards = FindObjectsByType<PlayerBoard>(0);

        // Waits for all boards to be spawned server side
        while (PlayerBoards.Length < playerCount)
        {
            PlayerBoards = FindObjectsByType<PlayerBoard>(0);
            
            yield return null;
        }
        // Wait For Unique Ids
        bool wait = true;
        while (wait)
        {
            wait = false;
            List<ulong> owners = new List<ulong>(); 
            foreach (PlayerBoard board in PlayerBoards)
            {
                if (owners.Contains(board.owner.Value))
                {
                    wait = true;
                    break;
                }
                else
                {
                    owners.Add(board.owner.Value);
                }
            }
            
            yield return null;
        }

        PlayerBoardsManager.Initialize(PlayerBoards.ToList());
        SpawnGridsLocally();

        // Signal We are done

        HostControls.SignalBoardsSpawnedServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void SpawnGridsLocally()
    {
        // Get Sorted List of Players
        List<ulong> playerIds = new List<ulong>();
        foreach (ulong playerID in NetworkManager.ConnectedClientsIds)
        {
            playerIds.Add(playerID);
        }
        playerIds.Sort();

        int i = 1;
        foreach (ulong playerID in playerIds)
        {
            Vector2 GridOffset = new Vector2(spacing.x * ((i-1) % 3), (int)((i - 1) / 3) * spacing.y);
            Transform mainGrid = HexagonGridGenerator.SpawnHexagonGrid(i, GridOffset, HexagonGridGenerator.playerBoardSize);

            // Side Board
            Vector2 sideBoardOffset = new Vector2(17f, 0) + GridOffset;
            Transform sideGrid = HexagonGridGenerator.SpawnHexagonGrid(-i, sideBoardOffset, HexagonGridGenerator.sideBoardSize);

            PlayerBoard CurrentBoard = PlayerBoardsManager.PlayerBoardTable[playerID];
            CurrentBoard.Init(mainGrid.gameObject.GetComponent<HexagonGrid>(), sideGrid.gameObject.GetComponent<HexagonGrid>(), i);


            i++;
        }



    }

}
