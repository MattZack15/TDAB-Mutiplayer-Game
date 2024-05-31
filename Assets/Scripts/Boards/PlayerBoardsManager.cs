using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardsManager : MonoBehaviour
{
    //public Dictionary<ulong, PlayerBoard> PlayerBoardTable = new Dictionary<ulong, PlayerBoard>();
    public List<PlayerBoard> PlayerBoards;


    public void Initialize(List<PlayerBoard> PlayerBoardsFound)
    {
        foreach (PlayerBoard PlayerBoard in PlayerBoardsFound)
        {
            //PlayerBoardTable.Add(PlayerBoard.owner.Value, PlayerBoard);
            PlayerBoards.Add(PlayerBoard);
        }
    }
}
