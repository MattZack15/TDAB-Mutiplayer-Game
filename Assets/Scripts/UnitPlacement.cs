using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    [SerializeField] PlayerTileInteraction PlayerTileInteraction;

    [SerializeField] float hoverHeight;

    private Transform grabbedUnit;

    private GameObject lastHoveredTile;
    
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
            }
        }
    }

    private void Release()
    {
        if (grabbedUnit == null) { return; }

        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();

        if (hoveredTile != null)
        {
            // Check if we hit a hex tile
            grabbedUnit.position = hoveredTile.transform.position;
        }
        else if (lastHoveredTile != null)
        {
            grabbedUnit.position = lastHoveredTile.transform.position;
        }

        grabbedUnit = null;
        lastHoveredTile = null;
    }

    private void DragUnit()
    {

        GameObject hoveredTile = PlayerTileInteraction.GetSelectedTile();

        if (hoveredTile != null)
        {
            // Check if we hit a hex tile
            grabbedUnit.position = hoveredTile.transform.position + new Vector3(0f, hoverHeight, 0f);
            lastHoveredTile = hoveredTile;
        }
        else if (lastHoveredTile != null) 
        {
            grabbedUnit.position = lastHoveredTile.transform.position + new Vector3(0f, hoverHeight, 0f);
        }

    }

}
