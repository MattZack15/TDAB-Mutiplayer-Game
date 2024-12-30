using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitUpgrades : NetworkBehaviour
{
    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitPlacement unitPlacement;


    // When we get three of a unit, we destory them and spawn in the upgraded unit

    public void CheckForUnitUpgrade(int boardID)
    {
        if (!IsServer) { return; }

        
        // <UnitName, List<(Unit, Tile)>>
        Dictionary<string, List<(GameObject, GameObject)>> UnitsCount = new Dictionary<string, List<(GameObject, GameObject)>>();

        void IncrementDict(string unitName, GameObject Unit, GameObject tile)
        {
            if (UnitsCount.ContainsKey(unitName))
            {
                UnitsCount[unitName].Add((Unit, tile));
            }
            else
            {
                UnitsCount.Add(unitName, new List<(GameObject, GameObject)> { (Unit, tile) });
            }
        }

        void LoopThroughGird(HexagonGrid hexagonGrid)
        {
            foreach (Vector2 TileId in hexagonGrid.Tiles.Keys)
            {
                HexagonTile tile = hexagonGrid.GetTileById(TileId).GetComponent<HexagonTile>();

                if (tile.inhabitor != null && tile.inhabitor.GetComponent<Unit>() != null)
                {
                    
                    GameObject Unit = tile.inhabitor;
                    
                    Unit UnitComp = Unit.GetComponent<Unit>();
                    // Check if Unit is unupgraded
                    if (UnitComp.level == 1)
                    {
                        string unitName = UnitComp.UnitName;
                        IncrementDict(unitName, Unit, tile.gameObject);
                    }
                }
            }
        }
        
        PlayerBoard Board = playerBoardsManager.GetBoardByBoardID(boardID);
        LoopThroughGird(Board.HexagonGrid);
        LoopThroughGird(Board.SideBoard.SideBoardGrid);

        // Check if any have 3 of a kind
        foreach (string key in UnitsCount.Keys)
        {
            if (UnitsCount[key].Count >= 3)
            {
                UpgradeUnit(UnitsCount[key], Board.SideBoard);
            }

            //print($"{key}: {UnitsCount[key].Count}");
        }


    }

    private void UpgradeUnit(List<(GameObject, GameObject)> UnitsAndTiles, SideBoard sideBoard)
    {

        // Spawn Upgraded Unit
        GameObject UpgradedUnitPrefab = UnitsAndTiles[0].Item1.GetComponent<Unit>().UpgradedVersion;

        if (UpgradedUnitPrefab == null)
        {
            print($"This Unit Has no Upgrade {UnitsAndTiles[0].Item1.GetComponent<Unit>().UnitName}");
            return;
        }

        GameObject spawnedUnit = sideBoard.AddUnitToSideBoard(UpgradedUnitPrefab);

        // New tower will have the combined kill count of level 1 towers
        int totalKillCount = 0;
        
        // Destory previous Units
        int i = 0;
        foreach ((GameObject, GameObject) UnitAndTile in UnitsAndTiles)
        {
            unitPlacement.ClearTileClientRPC(UnitAndTile.Item2.GetComponent<HexagonTile>().tileId);

            // Track kills
            GameObject unit = UnitAndTile.Item1;
            if (unit.GetComponent<Unit>().isTower())
            {
                totalKillCount += unit.GetComponent<Tower>().kills.Value;
            }

            unit.GetComponent<NetworkObject>().Despawn();

            // Only Destory 3 for an upgrade
            i++;
            if (i == 3)
            {
                break;
            }
        }

        // Track kills
        if (spawnedUnit.GetComponent<Unit>().isTower()) 
        {
            spawnedUnit.GetComponent<Tower>().kills.Value = totalKillCount;
        }
    }

    IEnumerator WriteKillsToUpgradedTower(GameObject UpgradedUnit, int totalKillCount)
    {
        while (!UpgradedUnit.GetComponent<NetworkObject>().IsSpawned)
        {
            print("Waiting");
            yield return null;
        }

        
    }



}
