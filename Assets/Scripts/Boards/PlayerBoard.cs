using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBoard : NetworkBehaviour
{
    public NetworkVariable<ulong> owner = new NetworkVariable<ulong>();
    public int BoardID;

    // Set During board generation
    [HideInInspector] public HexagonGrid HexagonGrid;

    // Accssed by other object to spawn stuff
    public AttackerSpawner AttackerSpawner;

    [SerializeField] private GameObject EndZone;

    public static Vector2 startTile = new Vector2(5f, 17f);
    public static Vector2 endTile = new Vector2(6f, 0f);


    public void Init(HexagonGrid HexagonGrid, int id)
    {
        // Assign data
        this.HexagonGrid = HexagonGrid;
        BoardID = id;

        // Set board position to match the hexgrid
        transform.position = HexagonGrid.transform.position;
        // Make board the parent object
        HexagonGrid.transform.SetParent(transform);

        // Highlight Start and End Tiles
        HexagonGrid.GetTileById(startTile).GetComponent<HexagonTile>().UpdateNewColor(Color.red);
        HexagonTile EndTile = HexagonGrid.GetTileById(endTile).GetComponent<HexagonTile>();
        EndTile.UpdateNewColor(Color.red);

        if (IsServer)
        {
            // Server Only Board Init

            //Set Ending Collider
            EndZone.transform.position = EndTile.transform.position;
        }
    }

}
