using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitUpgrades : NetworkBehaviour
{
    [SerializeField] PlayerBoardsManager playerBoardsManager;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Checking For Upgrades");
            CheckForUnitUpgrade(1);
        }
    }

    // When we get three of a unit, we destory them and spawn in the upgraded unit

    public void CheckForUnitUpgrade(int boardID)
    {
        if (!IsServer) { return; }

        print("Starting");

        Dictionary<string, List<GameObject>> UnitsCount = new Dictionary<string, List<GameObject>>();

        void IncrementDict(string unitName, GameObject Unit)
        {
            if (UnitsCount.ContainsKey(unitName))
            {
                UnitsCount[unitName].Add(Unit);
            }
            else
            {
                UnitsCount.Add(unitName, new List<GameObject> { Unit });
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

                    print(Unit);
                    
                    Unit UnitComp = Unit.GetComponent<Unit>();
                    // Check if Unit is unupgraded
                    if (UnitComp.level == 1)
                    {
                        string unitName = UnitComp.UnitName;
                        IncrementDict(unitName, Unit);
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

            print($"{key}: {UnitsCount[key].Count}");
        }


    }

    private void UpgradeUnit(List<GameObject> Units, SideBoard sideBoard)
    {

        // Spawn Upgraded Unit
        GameObject UpgradedUnit = Units[0].GetComponent<Unit>().UpgradedVersion;

        if (UpgradedUnit == null)
        {
            print($"This Unit Has no Upgrade {Units[0].GetComponent<Unit>().UnitName}");
            return;
        }

        sideBoard.AddUnitToSideBoard(UpgradedUnit);


        // Destory previous Units
        foreach (GameObject unit in Units)
        {
            unit.GetComponent<NetworkObject>().Despawn();
        }
    }

}
