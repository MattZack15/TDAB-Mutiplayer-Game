using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SideBoard : NetworkBehaviour
{
    UnitPlacement UnitPlacement;
    UnitUpgrades UnitUpgrades;
    [HideInInspector] public HexagonGrid SideBoardGrid;
    
    // Start is called before the first frame update
    public void Init(HexagonGrid SideBoardGrid)
    {
        this.SideBoardGrid = SideBoardGrid;
        UnitPlacement = FindObjectOfType<UnitPlacement>();
        SetSideBoardColor(SideBoardGrid);
        UnitUpgrades = FindObjectOfType<UnitUpgrades>();
    }

    private void SetSideBoardColor(HexagonGrid sideGrid)
    {
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
    }

    public GameObject AddUnitToSideBoard(GameObject Unit)
    {
        if (!IsServer) { return null; }
        // Soley Responsible for Spawning unit into the players board

        foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
            // Spawn Unit here if not occupied
            if (!tile.occupied)
            {
                GameObject newUnit = Instantiate(Unit, tile.gameObject.transform.position, Quaternion.identity);
                NetworkObject newUnitNetworkObject = newUnit.GetComponent<NetworkObject>();
                newUnitNetworkObject.Spawn();

                newUnit.GetComponent<Unit>().SetInactive();

                newUnit.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                UnitPlacement.PlaceUnitOnSideBoardClientRPC(tile.tileId, newUnitNetworkObject.NetworkObjectId, tile.transform.position);

                AddUnitToSideBoardClientRPC(newUnitNetworkObject.NetworkObjectId);

                UnitUpgrades.CheckForUnitUpgrade((int)tile.tileId.z);

                return newUnit;
            }
        }

        print("No Open Tiles to display unit");
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

    public List<GameObject> GetAttackers()
    {
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
}
