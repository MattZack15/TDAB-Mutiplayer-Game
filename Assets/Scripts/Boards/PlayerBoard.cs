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

    [SerializeField] public GameObject EndZone;
    [SerializeField] private PathCreator PathCreator;
    [SerializeField] private PathManager PathManager;
    [SerializeField] public SideBoard SideBoard;
    [SerializeField] public Transform camPos;

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
        PathManager.Init();
        SideBoard.Init(sideGrid);

        // Highlight Start and End Tiles
        HexagonGrid.GetTileById(startTile).GetComponent<HexagonTile>().SetStartEndTile(Color.red);
        HexagonTile EndTile = HexagonGrid.GetTileById(endTile).GetComponent<HexagonTile>();
        EndTile.SetStartEndTile(Color.red);


        if (owner.Value == NetworkManager.Singleton.LocalClientId)
        {
            // Only Do this if my Board
            FindObjectOfType<CameraMovement>().MoveToStartPos();
        }

        if (IsServer)
        {
            // Server Only Board Init

            AttackerSpawner.Init();

            //Set Ending Collider
            EndZone.transform.position = EndTile.transform.position;
        }
    }


}
