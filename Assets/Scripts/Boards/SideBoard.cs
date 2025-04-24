using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SideBoard : NetworkBehaviour
{
    // Side Board is Split into Attackers and Towers
    UnitUpgrades UnitUpgrades;
    [HideInInspector] public HexagonGrid SideBoardGrid;
    private ServerPlayerDataManager ServerPlayerDataManager;
    [HideInInspector] public PlayerBoard parentBoard;

    [SerializeField] List<int> AttackerLimits = new List<int>();

    // Start is called before the first frame update
    public void Init(HexagonGrid SideBoardGrid, PlayerBoard parentBoard)
    {
        this.SideBoardGrid = SideBoardGrid;
        this.parentBoard = parentBoard;
        UnitUpgrades = FindObjectOfType<UnitUpgrades>();

        ServerPlayerDataManager = FindObjectOfType<ServerPlayerDataManager>();

        SplitSideBoard(SideBoardGrid);
        SetSideBoardColor(SideBoardGrid);
        
    }

    private void SplitSideBoard(HexagonGrid sideGrid)
    {
        Vector3 offset = new Vector3(.75f, 0f, 0f);
        // Pulls Apart the sideboard
        foreach (Vector2 tildeID in sideGrid.Tiles.Keys)
        {
            if (tildeID.x == 1)
            {
                sideGrid.Tiles[tildeID].transform.position += offset;
            }
            if (tildeID.x == 0)
            {
                sideGrid.Tiles[tildeID].transform.position += new Vector3(0f, 0f, 0.649519f);
            }
        }
    }

    private void SetSideBoardColor(HexagonGrid sideGrid)
    {
        // Side Board Coloring
        float i = 0;
        foreach (Vector2 tildeID in sideGrid.Tiles.Keys)
        {
            Color newColor;
            if (tildeID.x == 0)
            {
                newColor = Color.Lerp(Color.cyan, new Color(165f/255f, 209f / 255f, 240f / 255f), i / sideGrid.Tiles.Keys.Count);
            }
            else
            {
                newColor = Color.Lerp(Color.red, new Color(235f/255f, 158f / 255f, 52f / 255f), i / sideGrid.Tiles.Keys.Count);
            }

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
    }

    public GameObject AddUnitToSideBoard(GameObject Unit)
    {
        if (!IsServer) { return null; }
        // Soley Responsible for Spawning unit into the players board

        HexagonTile tile = FindPlacementTile(Unit);

        if (tile == null)
        {
            print("No Open Tiles to display unit");
            return null;
        }
        
        // Spawn Unit here
        GameObject newUnit = Instantiate(Unit, tile.gameObject.transform.position, Quaternion.identity);
        NetworkObject newUnitNetworkObject = newUnit.GetComponent<NetworkObject>();
        newUnitNetworkObject.Spawn();

        newUnit.GetComponent<Unit>().SetInactive();

        newUnit.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // unit placement
        newUnit.transform.position = tile.transform.position;
        newUnit.GetComponent<Unit>().homeTile = tile;
        tile.SetOccupied(newUnit.gameObject);

        AddUnitToSideBoardClientRPC(newUnitNetworkObject.NetworkObjectId);

        UnitUpgrades.CheckForUnitUpgrade((int)tile.tileId.z);

        return newUnit;

    }

    private HexagonTile FindPlacementTile(GameObject Unit)
    {
        // Looks thorugh the sideboard and finds the right tile for this unit to be placed on
        // Just need a tile that is unoccupied
        // But we have some preferences

        HexagonTile tile;

        // Handle Towers
        if (Unit.GetComponent<Unit>().isTower())
        {
            // Prio is Left Side 
            foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
            {
                tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
                if (tile.tileId.x != 0f) { continue; }
                if (!tile.occupied)
                {
                    return tile;
                }
            }
            // If did not find a tile then look through all board spaces
            foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
            {
                tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
                if (!tile.occupied)
                {
                    return tile;
                }
            }
        }
        // Handle Attackers
        else if (Unit.GetComponent<Unit>().isAttacker())
        {
            // Prio is Right Side 
            foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
            {
                tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
                if (tile.tileId.x != 1f) { continue; }
                if (!tile.occupied)
                {
                    return tile;
                }
            }
            // If did not find a tile then look through all board spaces
            foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
            {
                tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
                if (!tile.occupied)
                {
                    return tile;
                }
            }
        }

        return null;
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void AddUnitToSideBoardClientRPC(ulong networkObjectId)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        GameObject newUnit = networkObject.gameObject;
        // we do have to do this
        newUnit.GetComponent<Unit>().SetInactive();
    }

    public List<GameObject> GetAllAttackersOnSideBoard()
    {
        // Searches though all of the side board and returns all of the attacker there

        List<GameObject> attackers = new List<GameObject>();

        // Loop Through every tile on the side board and get the units in Order
        foreach (Vector2 TileId in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileId).GetComponent<HexagonTile>();

            if (tile.inhabitor != null && tile.inhabitor.GetComponent<Attacker>() != null)
            {
                attackers.Add(tile.inhabitor);
            }
        }

        return attackers;
    }

    public List<GameObject> GetAttackersForBattle()
    {
        // Returns A list of all attackers on this sideboard that are leaglly able to be sent into battle

        List<GameObject> attackers = new List<GameObject>();
        int AttackerLimit = GetAttackerLimit();

        // Loop Through every tile on the side board and get the units in Order
        foreach (Vector2 TileId in SideBoardGrid.Tiles.Keys)
        {
            // Ignore Tower Sideboard
            if (TileId.x != 1) { continue; }
            // Only count upto attacker limit
            if ((TileId.y+1)/2 > AttackerLimit) { continue; }

            HexagonTile tile = SideBoardGrid.GetTileById(TileId).GetComponent<HexagonTile>();

            if (tile.inhabitor != null && tile.inhabitor.GetComponent<Attacker>() != null)
            {
                attackers.Add(tile.inhabitor);
            }
        }

        return attackers;
    }

    public int GetAttackerLimit()
    {
        // How many attackers will be sent into battle
        // Will be called client side

        if (!ServerPlayerDataManager) return 0;
        int level = ServerPlayerDataManager.GetPlayerData(parentBoard.owner.Value).level.Value;
        level = Mathf.Min(level, AttackerLimits.Count-1);
        return AttackerLimits[level - 1];
    }

    public bool isAvailableSpace(GameObject unitPrefab)
    {
        // Checks if there is enough open tiles to buy a unit and place it

        int openSpacesNeeded = 1;
        if (unitPrefab.GetComponent<ReplicatingBlobOnPurchase>())
        {
            openSpacesNeeded = 2;
        }

        int openSpaces = 0;
        foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
            if (!tile.occupied)
            {
                openSpaces += 1;
            }
        }

        return openSpacesNeeded <= openSpaces;
    }

}
