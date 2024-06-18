using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class TowerPlacer : NetworkBehaviour
{
    public GameObject tower;
    public GameObject attacker;
    [SerializeField] private PlayerTileInteraction playerTileInteraction;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {

            GameObject SelectedTile = playerTileInteraction.GetSelectedTile();

            if (SelectedTile == null){ return;}

            Vector3 tilePos = SelectedTile.transform.position;

            int boardId = (int)SelectedTile.GetComponent<HexagonTile>().tileId.z;


            if (Input.GetKeyDown(KeyCode.A))
            {

                SpawnTowerServerRpc(tilePos, 0, boardId);
            }

        }

    }

    [Rpc(SendTo.Server)]
    private void SpawnTowerServerRpc(Vector3 spawnPos, int obj, int boardID)
    {
        GameObject wantedObj;
        if(obj == 0)
        {
            wantedObj = tower;
        }
        else
        {
            wantedObj = attacker;
        }


        GameObject newObj = Instantiate(wantedObj, spawnPos, Quaternion.identity);
        newObj.GetComponent<NetworkObject>().Spawn();

        PlayerBoard CurrentBoard = PlayerBoardsManager.GetBoardByBoardID(boardID);
        Transform EndPos = CurrentBoard.HexagonGrid.GetTileById(PlayerBoard.endTile).transform;

        newObj.GetComponent<Tower>().trackEndPoint = EndPos;


    }
}
