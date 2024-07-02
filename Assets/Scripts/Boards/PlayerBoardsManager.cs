using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBoardsManager : NetworkBehaviour
{
    public Dictionary<ulong, PlayerBoard> PlayerBoardTable = new Dictionary<ulong, PlayerBoard>();
    //private Dictionary<int, PlayerBoard> BoardIDToBoard = new Dictionary<int, PlayerBoard>();


    public void Initialize(List<PlayerBoard> PlayerBoardsFound)
    {
        foreach (PlayerBoard PlayerBoard in PlayerBoardsFound)
        {

            PlayerBoardTable.Add(PlayerBoard.owner.Value, PlayerBoard);
        }
    }

    public PlayerBoard GetBoardByBoardID(int boardID)
    {
        boardID = Mathf.Abs(boardID);

        foreach (ulong playerID in PlayerBoardTable.Keys)
        {
            if (PlayerBoardTable[playerID].BoardID == boardID)
            {
                return PlayerBoardTable[playerID];
            }
        }
        print($"Board ID: {boardID} Not Found");
        return null;
    }

    public PlayerBoard GetMyBoard()
    {        
        return PlayerBoardTable[NetworkManager.Singleton.LocalClientId];
    }

    public GameObject GetTileById(Vector3 tileID)
    {
        int boardID = (int)tileID.z;

        if (tileID.z > 0)
        {
            return GetBoardByBoardID(boardID).HexagonGrid.GetTileById(new Vector2(tileID.x, tileID.y));
        }
        else
        {
            return GetBoardByBoardID(boardID).SideBoard.SideBoardGrid.GetTileById(new Vector2(tileID.x, tileID.y));
        }

        
    }
}
