using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SideBoard : NetworkBehaviour
{
    UnitPlacement UnitPlacement;
    public HexagonGrid SideBoardGrid;
    
    // Start is called before the first frame update
    public void Init(HexagonGrid SideBoardGrid)
    {
        this.SideBoardGrid = SideBoardGrid;
        UnitPlacement = FindObjectOfType<UnitPlacement>();
    }


    public void AddUnitToSideBoard(GameObject Unit)
    {
        if (!IsServer) { return; }
        
        foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
            // Spawn Unit here if not occupied
            if (!tile.occupied)
            {
                GameObject newUnit = Instantiate(Unit, tile.gameObject.transform.position, Quaternion.identity);
                NetworkObject newUnitNetworkObject = newUnit.GetComponent<NetworkObject>();
                newUnitNetworkObject.Spawn();
                
                newUnit.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                UnitPlacement.RequestUnitPlacmentServerRPC(tile.tileId, newUnitNetworkObject.NetworkObjectId);

                AddUnitToSideBoardClientRPC(newUnitNetworkObject.NetworkObjectId);
                return;
            }
        }

        print("No Open Tiles to display unit");
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void AddUnitToSideBoardClientRPC(ulong networkObjectId)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        GameObject newUnit = networkObject.gameObject;

        newUnit.GetComponent<Unit>().SetInactive();
    }

    public List<int> GetAttackers()
    {
        List<int> attackerIDs = new List<int>();

        // Loop Through every tile on the side board and get the units in Order
        foreach (Vector2 TileId in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileId).GetComponent<HexagonTile>();

            if (tile.inhabitor != null && tile.inhabitor.GetComponent<Attacker>() != null)
            {
                attackerIDs.Add(tile.inhabitor.GetComponent<Unit>().UnitID);
            }
        }

        return attackerIDs;
    }
}
