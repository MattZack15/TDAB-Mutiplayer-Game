using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct BoardState
{
    public int[] Towers;
    public Vector3[] positions;
}
public class PlayerBoard : NetworkBehaviour
{
    public NetworkVariable<ulong> owner = new NetworkVariable<ulong>();
    public int BoardID;

    // Set During board generation
    [HideInInspector] public HexagonGrid HexagonGrid;

    // Accssed by other object to spawn stuff
    public AttackerSpawner AttackerSpawner;

    [SerializeField] private GameObject EndZone;
    [SerializeField] private PathCreator PathCreator;
    [SerializeField] public SideBoard SideBoard;

    public static Vector2 startTile = new Vector2(5f, 17f);
    public static Vector2 endTile = new Vector2(6f, 0f);


    public void Init(HexagonGrid mainGrid, HexagonGrid sideGrid, int id)
    {
        // Assign data
        HexagonGrid = mainGrid;
        BoardID = id;

        gameObject.name = $"Board {id}, Owner: {owner.Value}";

        // Set board position to match the hexgrid
        transform.position = HexagonGrid.transform.position;
        // Make board the parent object
        HexagonGrid.transform.SetParent(transform);
        sideGrid.transform.SetParent(transform);

        PathCreator.Init();
        SideBoard.Init(sideGrid);

        // Highlight Start and End Tiles
        HexagonGrid.GetTileById(startTile).GetComponent<HexagonTile>().SetStartEndTile(Color.red);
        HexagonTile EndTile = HexagonGrid.GetTileById(endTile).GetComponent<HexagonTile>();
        EndTile.SetStartEndTile(Color.red);

        // Side Board Coloring
        float i = 0;
        foreach (Vector2 tildeID in sideGrid.Tiles.Keys)
        {
            Color newColor = Color.Lerp(Color.yellow, Color.red, i / sideGrid.Tiles.Keys.Count);

            if (i % 3 == 0)
            {
                newColor = Color.Lerp(newColor, Color.black, .1f);
            }
            if (i % 3 == 2)
            {
                newColor = Color.Lerp(newColor, Color.white, .2f);
            }


            sideGrid.Tiles[tildeID].GetComponent<HexagonTile>().SetSideBoard(newColor);
            i++;
        }

        if (IsServer)
        {
            // Server Only Board Init

            AttackerSpawner.Init();

            //Set Ending Collider
            EndZone.transform.position = EndTile.transform.position;
        }
    }



    public BoardState GetTowerInfo()
    {
        List<int> TowerIDs = new List<int>();
        List<Vector3> positions = new List<Vector3>();


        // Loop Through every tile on the board and get the Tower
        foreach (Vector2 TileId in HexagonGrid.Tiles.Keys)
        {
            HexagonTile tile = HexagonGrid.GetTileById(TileId).GetComponent<HexagonTile>();

            if (tile.inhabitor != null && tile.inhabitor.GetComponent<Tower>() != null)
            {
                TowerIDs.Add(tile.inhabitor.GetComponent<Unit>().UnitID);
                positions.Add(tile.inhabitor.transform.position);
            }
        }

        BoardState boardState = new BoardState();
        boardState.Towers = TowerIDs.ToArray();
        boardState.positions = positions.ToArray();

        return boardState;
    }
}
