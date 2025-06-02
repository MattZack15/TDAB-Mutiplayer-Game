using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPlacement : NetworkBehaviour
{
    [SerializeField] GamePhaseManager GamePhaseManager;
    [SerializeField] Camera gameCamera;
    [SerializeField] PlayerTileInteraction PlayerTileInteraction;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    [SerializeField] float hoverHeight = .5f;

    // Server Track of What Players are holding what units
    Dictionary<ulong, GameObject> playerHeldUnits = new Dictionary<ulong, GameObject>();

    // Client Side Keep Track of Grabbed Unit
    public GameObject grabbedUnit;

    
    void Update()
    {        
        if (Input.GetMouseButtonDown(0) && !isMouseOverUI())
        {
            TryGrab();
        }

        if (Input.GetMouseButtonUp(0) && !isMouseOverUI())
        {
            Release();
        }

        // Client Side Drag
        if (grabbedUnit)
        {
            DragUnit();
        }

    }

    private bool isMouseOverUI()
    {
        //return EventSystem.current.IsPointerOverGameObject();
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("ShopUI"))
            {
                return true;
            }
        }

        return false;
    }

    private bool GamePhaseIsShopPhase() => GamePhaseManager.GamePhase.Value == (int)GamePhaseManager.GamePhases.ShopPhase;

    private void TryGrab()
    {
        // Requests to Grab a unit

        // Client Side Check: Can Only Grab During Shop Phase
        if (!GamePhaseIsShopPhase()) { return;}

        // Find Unit Under Mouse
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Unit hitUnit = null;
        if (Physics.Raycast(ray, out hitInfo))
        {
            hitUnit = hitInfo.collider.gameObject.GetComponent<Unit>();
        }
        if (hitUnit == null)
        {
            return;
        }

        // Send Grab Request to Server
        ulong unitNetworkID = hitUnit.GetComponent<NetworkObject>().NetworkObjectId;
        GrabUnitServerRPC(NetworkManager.Singleton.LocalClientId, unitNetworkID);
    }

    [Rpc(SendTo.Server)]
    private void GrabUnitServerRPC(ulong playerID, ulong networkObjectId)
    {
        // Can Only Grab During Shop Phase
        if (!GamePhaseIsShopPhase()) { return; }

        // Find Unit
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);
        if (networkObject == null) { return; }

        Unit unit = networkObject.GetComponent<Unit>();

        // Player can only grab units on their own board
        int playersBoardID = PlayerBoardsManager.PlayerBoardTable[playerID].BoardID;
        int unitsBoardID = (int)Mathf.Abs(unit.homeTile.tileId.z);
        if (playersBoardID != unitsBoardID) { return; }

        // Accept
        playerHeldUnits[playerID] = unit.gameObject;
        unit.transform.position = unit.homeTile.transform.position + new Vector3(0f, hoverHeight, 0f);
        NotifyOfGrabbedUnitClientRPC(playerID, networkObjectId);

        // Play Sound Effect
        AudioManager.Instance.PlayOnClient("pickupunit", playerID);
    }

    [Rpc(SendTo.Server)]
    private void UpdateGrabbedUnitServerRPC(ulong playerID, Vector3 mouseWorldPos)
    {
        // If update was sent in error
        if (playerHeldUnits[playerID] == null)
        {
            NotifyOfResetClientRPC(new ulong[] { playerID });
            return;
        }

        // Ignore if not in shop phase
        if (GamePhaseManager.GamePhase.Value != (int)GamePhaseManager.GamePhases.ShopPhase)
        {
            ResetGrabbedUnitServerRPC(playerID);
            return;
        }

        GameObject unit = playerHeldUnits[playerID];
        // Update Pos
        unit.transform.position = mouseWorldPos + new Vector3(0f, hoverHeight, 0f);
    }

    [Rpc(SendTo.Server)]
    private void ResetGrabbedUnitServerRPC(ulong playerID)
    {
        GameObject unit = playerHeldUnits[playerID];
        if (unit != null)
        {
            // Move back to home tile
            unit.transform.position = unit.GetComponent<Unit>().homeTile.transform.position;
        }

        playerHeldUnits[playerID] = null;

        NotifyOfResetClientRPC(new ulong[] {playerID});
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyOfResetClientRPC(ulong[] playerIDs)
    {
        // Ensures that the client knows it is not holding a unit
        if (playerIDs.ToList().Contains(NetworkManager.Singleton.LocalClientId))
        {
            grabbedUnit = null;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyOfGrabbedUnitClientRPC(ulong playerIDs, ulong networkObjectId)
    {
        if (NetworkManager.Singleton.LocalClientId == playerIDs)
        {
            NetworkObject networkObject;
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

            grabbedUnit = networkObject.gameObject;
        }
    }
    
    private void DragUnit()
    {

        //  Stop if mouse is not being held (like if user tab'ed out of game)
        if (!Input.GetMouseButton(0))
        {
            ResetGrabbedUnitServerRPC(NetworkManager.Singleton.LocalClientId);
            grabbedUnit = null;
            return;
        }

        Vector3 mouseWorldPos = Vector3.zero;

        // Send Raycast to find point where to hover
        // Find World Pos;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(gameCamera.ScreenPointToRay(Input.mousePosition));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<HexagonTile>() != null)
            {
                mouseWorldPos = hit.point;
            }
        }

        // Make sure there is a tile below to hover over
        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();
        if (hoveredTile != null && mouseWorldPos != Vector3.zero)
        {
            UpdateGrabbedUnitServerRPC(NetworkManager.Singleton.LocalClientId, mouseWorldPos);
        }
    }

    private void Release()
    {
        if (!grabbedUnit) { return; }

        // Find Tile
        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();
        if (hoveredTile == null) 
        {
            ResetGrabbedUnitServerRPC(NetworkManager.Singleton.LocalClientId);
            grabbedUnit = null;
            return;
        }

        ReleaseGrabServerRPC(NetworkManager.Singleton.LocalClientId, hoveredTile.GetComponent<HexagonTile>().tileId);
    }

    [Rpc(SendTo.Server)]
    private void ReleaseGrabServerRPC(ulong playerID, Vector3 tileID)
    {
        // Can only be in shop Phase
        if (GamePhaseManager.GamePhase.Value != (int)GamePhaseManager.GamePhases.ShopPhase)
        {
            ResetGrabbedUnitServerRPC(playerID);
            return;
        }

        // Make Sure that the player is holding a unit
        GameObject heldUnitObj = playerHeldUnits[playerID];
        if (heldUnitObj == null)
        {
            ResetGrabbedUnitServerRPC(playerID);
            return;
        }
        
        PlayerBoard targetBoard = PlayerBoardsManager.GetBoardByBoardID((int)tileID.z);
        
        // Find Target Tile
        HexagonTile tile;
        if (tileID.z > 0)
        {
            tile = targetBoard.HexagonGrid.GetTileById((Vector2)tileID).GetComponent<HexagonTile>();
        }
        else 
        {
            tile = targetBoard.SideBoard.SideBoardGrid.GetTileById((Vector2)tileID).GetComponent<HexagonTile>();
        }

        if (!isLegalTile(playerID, heldUnitObj, tile, tile.isOccupied()))
        {
            ResetGrabbedUnitServerRPC(playerID);
            return;
        }
        
        Unit heldUnit = heldUnitObj.GetComponent<Unit>();
        // Accept Case 1 - Placing On empty tile
        if (!tile.isOccupied())
        {
            heldUnit.homeTile.SetUnoccupied();
            heldUnit.homeTile = tile;
            tile.SetOccupied(heldUnitObj);
            ResetGrabbedUnitServerRPC(playerID);
            // Play Sound Effect
            AudioManager.Instance.PlayOnClient("placeunit", playerID);
            return;
        }
        // Case 2 - Tile is occupied
        else
        {
            // We want to swap
            // But we must check if the unit we are swaping with can move to the other tile
            GameObject otherUnitObj = tile.inhabitor;
            if (!isLegalTile(playerID, otherUnitObj, heldUnit.homeTile, true))
            {
                ResetGrabbedUnitServerRPC(playerID);
                return;
            }
            else
            {
                // Accept Case 2 - Swap Units
                // Set tiles with their new occupiers
                heldUnit.homeTile.SetOccupied(otherUnitObj);
                tile.SetOccupied(heldUnitObj);
                // Set Units with their new tile
                otherUnitObj.GetComponent<Unit>().homeTile = heldUnit.homeTile;
                heldUnit.homeTile = tile;

                heldUnit.transform.position = heldUnit.homeTile.transform.position;
                otherUnitObj.transform.position = otherUnitObj.GetComponent<Unit>().homeTile.transform.position;

                ResetGrabbedUnitServerRPC(playerID);
                // Play Sound Effect
                AudioManager.Instance.PlayOnClient("placeunit", playerID);
                return;
            }
        }

    }

    private bool isLegalTile(ulong playerID, GameObject unitObj, HexagonTile tile, bool isSwap = false)
    {
        // Cannot be a null tile
        PlayerBoard playerBoard = PlayerBoardsManager.GetBoardByBoardID((int)tile.tileId.z);
        if (tile == null)
        {
            return false;
        }

        // Can only Place on your own board
        int playersBoardID = PlayerBoardsManager.PlayerBoardTable[playerID].BoardID;
        if (playersBoardID != Mathf.Abs(tile.tileId.z))
        {
            return false;
        }

        Unit targetUnit = unitObj.GetComponent<Unit>();
        // Can Only Place Attackers On Sideboard
        if (targetUnit.isAttacker())
        {
            // z must be negative
            if (tile.tileId.z > 0)
            {
                return false;
            }
        }

        // Towers placed on main board must respect tower limits
            // Ignore this condition if we are swaping 2 towers
        if (!isSwap)
        {
            // Ignore this condition if we are moving a tower already on the board
            if (targetUnit.homeTile.tileId.z < 0)
            {
                // Check if we are placing on main board
                if (tile.tileId.z > 0)
                {
                    // Check to see if we would be over tower limit
                    int towerLimit = playerBoard.GetTowerLimit();
                    if (playerBoard.GetTowers().Count + 1 > towerLimit)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public GameObject GetMyHeldUnit()
    {
        return grabbedUnit;
    }

    public GameObject GetHeldUnitByPlayer(ulong playerID)
    {
        if (!IsServer) { return null; }

        if (!playerHeldUnits.ContainsKey(playerID))
        {
            return null;
        }

        return playerHeldUnits[playerID];
    }
}
