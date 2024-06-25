using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoard : MonoBehaviour
{

    private HexagonGrid SideBoardGrid;
    
    // Start is called before the first frame update
    public void Init(HexagonGrid SideBoardGrid)
    {
        this.SideBoardGrid = SideBoardGrid;
    }


    public void AddUnitToSideBoard(GameObject Unit)
    {
        foreach (Vector2 TileID in SideBoardGrid.Tiles.Keys)
        {
            HexagonTile tile = SideBoardGrid.GetTileById(TileID).GetComponent<HexagonTile>();
            // Spawn Unit here if not occupied
            if (!tile.occupied)
            {
                GameObject newUnit = Instantiate(Unit, tile.gameObject.transform.position, Quaternion.identity);
                newUnit.GetComponent<Unit>().SetInactive();
                tile.SetOccupied(newUnit);
                return;
            }
        }

        print("No Open Tiles to display unit");
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
