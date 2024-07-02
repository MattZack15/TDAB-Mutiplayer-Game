using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class UnitPlacement : NetworkBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] PlayerTileInteraction PlayerTileInteraction;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;

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
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Unit hitUnit = hitInfo.collider.gameObject.GetComponent<Unit>();

            if (hitUnit == null) 
            {
                return;
            }

            if (hitUnit.active)
            {
                print("2");
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
        if (unit.GetComponent<Attacker>() != null)
        {
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


        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();


        if (!isLeagalTile(grabbedUnit.gameObject, hoveredTile))
        {
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }

        HexagonTile hoveredHexTile = hoveredTile.GetComponent<HexagonTile>();

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

            //Positions
            RequestUnitPlacmentServerRPC(hoveredTile.GetComponent<HexagonTile>().tileId, grabbedUnit.GetComponent<NetworkObject>().NetworkObjectId);
            RequestUnitPlacmentServerRPC(originalTile.GetComponent<HexagonTile>().tileId, OtherObj.GetComponent<NetworkObject>().NetworkObjectId);

            //Book Keeping
            //originalTile.GetComponent<HexagonTile>().SetOccupied();
            //hoveredHexTile.SetOccupied(grabbedUnit.gameObject);

            ResetData();
            return;
        }

        // On Succsess
        if (hoveredHexTile.occupied == false)
        {
            //grabbedUnit.position = hoveredTile.transform.position;

            RequestUnitPlacmentServerRPC(hoveredTile.GetComponent<HexagonTile>().tileId, grabbedUnit.GetComponent<NetworkObject>().NetworkObjectId);

            hoveredTile.GetComponent<HexagonTile>().SetOccupied(grabbedUnit.gameObject);
            originalTile.GetComponent<HexagonTile>().SetUnoccupied();
        }


        ResetData();
    }

    private void DragUnit()
    {
        Vector3 HoverPoint = originalTile.transform.position;

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

        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();

        if (hoveredTile != null)
        {
            grabbedUnit.position = HoverPoint + new Vector3(0f, hoverHeight, 0f);
        }

    }

    [Rpc(SendTo.Server)]
    public void RequestUnitPlacmentServerRPC(Vector3 tileID, ulong networkObjectId)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        // Find that tiles pos
        
        Vector3 pos = PlayerBoardsManager.GetTileById(tileID).transform.position;
        networkObject.transform.position = pos;

        UnitPlacmentClientRPC(tileID, networkObjectId, pos);
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void UnitPlacmentClientRPC(Vector3 tileID, ulong networkObjectId, Vector3 pos)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        networkObject.transform.position = pos;

        // Set tile to occupied
        PlayerBoardsManager.GetTileById(tileID).GetComponent<HexagonTile>().SetOccupied(networkObject.gameObject);
    }


}
