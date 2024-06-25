using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] PlayerTileInteraction PlayerTileInteraction;

    [SerializeField] float hoverHeight;

    private Transform grabbedUnit;

    private GameObject originalTile;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            if (hitInfo.collider.gameObject.GetComponent<Unit>() != null)
            {
                // Check if we hit a Moveable Unit
                grabbedUnit = hitInfo.collider.gameObject.transform;

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
                }
                
            }
        }
    }

    private void Release()
    {
        if (grabbedUnit == null) { return; }

        void ResetData()
        {
            grabbedUnit = null;
            originalTile = null;
        }

        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();

        if (hoveredTile == null) 
        {
            // Fail placement (Must place on a tile)
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }


        HexagonTile hoveredHexTile = hoveredTile.GetComponent<HexagonTile>();
        if (hoveredHexTile.occupied && hoveredHexTile.inhabitor == null)
        {
            // Fail placement (Tile cannot have things placed on it)
            grabbedUnit.position = originalTile.transform.position;
            ResetData();
            return;
        }

        if (grabbedUnit.gameObject.GetComponent<Attacker>() != null)
        {
            // Attacker Placment Rules
            if (hoveredHexTile.tileId.z > 0)
            {
                // Fail placement (Attackers must be placed on SideBoard (Negative index))
                grabbedUnit.position = originalTile.transform.position;
                ResetData();
                return;
            }
        }

        if (hoveredHexTile.occupied && hoveredHexTile.inhabitor != null)
        {
            // Swap Two Objects
            GameObject OtherObj = hoveredHexTile.inhabitor;

            //Positions
            grabbedUnit.position = hoveredTile.transform.position;
            OtherObj.transform.position = originalTile.transform.position;

            //Book Keeping
            originalTile.GetComponent<HexagonTile>().SetOccupied(hoveredHexTile.inhabitor);
            hoveredHexTile.SetOccupied(grabbedUnit.gameObject);

            ResetData();
            return;
        }

        // On Succsess
        if (hoveredHexTile.occupied == false)
        {
            grabbedUnit.position = hoveredTile.transform.position;
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



}
