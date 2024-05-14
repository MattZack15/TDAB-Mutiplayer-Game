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

    [SerializeField] GameObject PlayerBoard;

    [SerializeField] PlayerBoardsManager PlayerBoardsManager;

    public void SpawnPlayerBoards()
    {
        if (!IsServer) return;
        
        // Server spawns the player board server objects

        int playerCount = NetworkManager.ConnectedClientsIds.Count;

        int i = 0;
        while (i < playerCount)
        {
            GameObject board = Instantiate(PlayerBoard);
            board.GetComponent<NetworkObject>().Spawn();
            board.GetComponent<PlayerBoard>().owner.Value = NetworkManager.ConnectedClientsIds[i];
            board.name = $"Board {NetworkManager.ConnectedClientsIds[i]}";
            

            
            
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
            yield return null;
        }

           // Maybe wait for them to be assined ids


        PlayerBoardsManager.Initialize(PlayerBoards.ToList());
        SpawnGridsLocally(playerCount);

    }

    private void SpawnGridsLocally(int playerCount)
    {
        int i = 0;

        while (i < playerCount)
        {
            Vector2 GridOffset = new Vector2(spacing.x * (i % 3), (int)(i / 3) * spacing.y);

            Transform newGrid = HexagonGridGenerator.SpawnHexagonGrid(i, GridOffset);
            Transform BoardTransform = PlayerBoardsManager.PlayerBoards[i].transform;

            BoardTransform.position = newGrid.position;
            newGrid.SetParent(BoardTransform);

            PlayerBoardsManager.PlayerBoards[i].GetComponent<PlayerBoard>().HexagonGrid = newGrid.gameObject.GetComponent<HexagonGrid>();

            i++;

        }
    }

}
