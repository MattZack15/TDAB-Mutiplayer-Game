using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


public class UnitPlacement : NetworkBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] PlayerTileInteraction PlayerTileInteraction;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    [SerializeField] GamePhaseManager GamePhaseManager;

    [SerializeField] float hoverHeight;

    private Transform grabbedUnit;

    private GameObject originalTile;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrab();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }

        if (grabbedUnit != null)
        {
            DragUnit();
        }
    }

    private void TryGrab()
    {

        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase)
        {
            return;
        }

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Unit hitUnit = hitInfo.collider.gameObject.GetComponent<Unit>();

            if (hitUnit == null) 
            {
                return;
            }


            // Check if we hit a Moveable Unit
            grabbedUnit = hitUnit.gameObject.transform;

            // Find Original Tile;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(grabbedUnit.transform.position + new Vector3(0f, .5f, 0f), Vector3.down * 5f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<HexagonTile>() != null)
                {
                    originalTile = hit.collider.gameObject;
                    break;
                }
            }

            if (originalTile == null)
            {
                print("Can't Find Original Tile");
                ResetData();
            }

            // Only Grab Units on our board
            if (PlayerBoardsManager.GetMyBoard().BoardID != Mathf.Abs(originalTile.gameObject.GetComponent<HexagonTile>().tileId.z))
            {
                ResetData();
            }



        }
    }

    private bool isLeagalTile(GameObject unit, GameObject tile)
    {
        // Tile must be not null
        if (tile == null)
        {
            // Fail placement (Must place on a tile)
            return false;
        }

        HexagonTile hexagonTile = tile.GetComponent<HexagonTile>();

        // Cant swap with something that is null
        if (hexagonTile.occupied && hexagonTile.inhabitor == null)
        {
            // Fail placement (Tile cannot have things placed on it)
            return false;
        }

        // Can only place attackers on side board
        if (unit.GetComponent<Unit>().isAttacker())
        {
            // If on main board
            if (hexagonTile.tileId.z > 0)
            {
                return false;
            }
        }

        // Can only place on YOUR board
        PlayerBoard targetBoard = PlayerBoardsManager.GetBoardByBoardID((int)tile.GetComponent<HexagonTile>().tileId.z);
        if (targetBoard.BoardID != PlayerBoardsManager.GetMyBoard().BoardID)
        {
            return false;
        }

        // Cannot Exceed Tower Limit for main board
        // If on we want to place on the main board and we are placing a tower
        // AND the unit we are holding is not already on the main board - because we can freely move units already on board or swap with it
        if (hexagonTile.tileId.z > 0 && unit.GetComponent<Unit>().isTower() && GetHeldUnitTileID().z <= 0)
        {
            // Ignore if we are swapping with a tower already on board
            if (hexagonTile.inhabitor == null)
            {
                // Finally we check for tower limit
                int towerLimit = targetBoard.GetTowerLimit();
                if (targetBoard.GetTowers().Count + 1 > towerLimit)
                {
                    return false;
                }
            }
        }


        return true;
    }

    private void ResetData()
    {
        grabbedUnit = null;
        originalTile = null;
    }

    private void Release()
    {
        if (grabbedUnit == null) { return; }

        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase)
        {
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }

        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();


        if (!isLeagalTile(grabbedUnit.gameObject, hoveredTile))
        {
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }

        HexagonTile hoveredHexTile = hoveredTile.GetComponent<HexagonTile>();
        Vector3 targetTileId = hoveredHexTile.tileId;
        Vector3 prevTileId = originalTile.GetComponent<HexagonTile>().tileId;

        if (hoveredHexTile.occupied && hoveredHexTile.inhabitor != null)
        {
            // Swap Two Objects
            GameObject OtherObj = hoveredHexTile.inhabitor;

            // Must Check to make sure moving other object is also legal
            if (!isLeagalTile(OtherObj, originalTile))
            {
                grabbedUnit.position = originalTile.transform.position;
                ResetData();
                return;
            }

            RequestUnitSwapServerRPC(targetTileId, prevTileId, grabbedUnit.GetComponent<NetworkObject>().NetworkObjectId, OtherObj.GetComponent<NetworkObject>().NetworkObjectId);

            ResetData();
            return;
        }

        // On Succsess
        if (hoveredHexTile.occupied == false)
        {
            RequestUnitPlacmentServerRPC(targetTileId, prevTileId, grabbedUnit.GetComponent<NetworkObject>().NetworkObjectId);
        }


        ResetData();
    }

    private void DragUnit()
    {
        // Stop Holding unit if Phase changes
        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase)
        {
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }

        // Default Hover point is above orignal tile
        Vector3 HoverPoint = originalTile.transform.position;

        // Send Raycast to find point where to hover
        // Find World Pos;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(gameCamera.ScreenPointToRay(Input.mousePosition));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<HexagonTile>() != null)
            {
                HoverPoint = hit.point;
            }
        }

        // Make sure there is a tile below to hover over
        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();
        if (hoveredTile != null)
        {
            grabbedUnit.position = HoverPoint + new Vector3(0f, hoverHeight, 0f);
        }

    }

    [Rpc(SendTo.Server)]
    public void RequestUnitPlacmentServerRPC(Vector3 targettileID, Vector3 prevtileID, ulong networkObjectId)
    {
        // Find that tiles pos
        Vector3 pos = PlayerBoardsManager.GetTileById(targettileID).transform.position;

        UnitPlacmentClientRPC(targettileID, prevtileID, networkObjectId, pos);
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void UnitPlacmentClientRPC(Vector3 targettileID, Vector3 prevtileID, ulong networkObjectId, Vector3 pos)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        networkObject.transform.position = pos;

        // Tile book keeping
        PlayerBoardsManager.GetTileById(targettileID).GetComponent<HexagonTile>().SetOccupied(networkObject.gameObject);
        PlayerBoardsManager.GetTileById(prevtileID).GetComponent<HexagonTile>().SetUnoccupied();

    }

    [Rpc(SendTo.Server)]
    public void RequestUnitSwapServerRPC(Vector3 targettileID, Vector3 prevtileID, ulong networkObjectId1, ulong networkObjectId2)
    {
        // Swaps Unit1 to targettileID and Unit2 to prevtileID

        Vector3 pos1 = PlayerBoardsManager.GetTileById(targettileID).transform.position;
        Vector3 pos2 = PlayerBoardsManager.GetTileById(prevtileID).transform.position;

        UnitSwapClientRPC(targettileID, prevtileID, networkObjectId1, networkObjectId2, pos1, pos2);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UnitSwapClientRPC(Vector3 targettileID, Vector3 prevtileID, ulong networkObjectId1, ulong networkObjectId2, Vector3 pos1, Vector3 pos2)
    {
        NetworkObject Unit1;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId1, out Unit1);

        NetworkObject Unit2;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId2, out Unit2);

        Unit1.transform.position = pos1;
        Unit2.transform.position = pos2;

        // Tile book keeping
        PlayerBoardsManager.GetTileById(targettileID).GetComponent<HexagonTile>().SetOccupied(Unit1.gameObject);
        PlayerBoardsManager.GetTileById(prevtileID).GetComponent<HexagonTile>().SetOccupied(Unit2.gameObject);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlaceUnitOnSideBoardClientRPC(Vector3 targettileID, ulong networkObjectId, Vector3 pos)
    {
        // Use this to start because Unit was never on a tile before
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        if (!networkObject.gameObject.GetComponent<NetworkTransform>().enabled)
        {
            networkObject.transform.position = pos;
        }
        

        // Tile book keeping
        PlayerBoardsManager.GetTileById(targettileID).GetComponent<HexagonTile>().SetOccupied(networkObject.gameObject);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ClearTileClientRPC(Vector3 tileID)
    {
        PlayerBoardsManager.GetTileById(tileID).GetComponent<HexagonTile>().SetUnoccupied();
    }


    public GameObject GetHeldUnit()
    {
        if (grabbedUnit)
        {
            return grabbedUnit.gameObject;
        }
        else
        {
            return null;
        }
    }

    public Vector3 GetHeldUnitTileID()
    {
        if (grabbedUnit)
        {
            return originalTile.GetComponent<HexagonTile>().tileId;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
