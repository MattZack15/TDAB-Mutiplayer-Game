using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    Tower towerScript;
    Unit unit;
    Camera cam;
    UnitPlacement UnitPlacement;
    public GameObject circle;

    
    private void Start()
    {
        towerScript = transform.parent.gameObject.GetComponent<Tower>();
        unit = towerScript.gameObject.GetComponent<Unit>();
        cam = FindObjectOfType<Camera>();
        UnitPlacement = FindObjectOfType<UnitPlacement>();

        // Setup Circle
        float d = towerScript.range * 2f;
        circle.transform.localScale = new Vector3(d, d, d);
        circle.SetActive(false);
    }

    private void Update()
    {
        if (!circle.activeSelf)
        {
            // When to show the range indicator
            // When the player right clicks on it, 
            if (CheckRightClick())
            {
                circle.SetActive(true);
            }
            // when the player is dragging it
            else if (UnitPlacement.GetMyHeldUnit())
            {
                // If the unit im holding is this unit
                if (UnitPlacement.GetMyHeldUnit().GetComponent<Unit>() == unit)
                {
                    circle.SetActive(true);
                }
            }

        }
        else
        {
            // When to turn off range indicator
            // When player clicks else where
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
            {
                circle.SetActive(false);
            }
        }
    }

    private bool CheckRightClick()
    {
        // Checks if the player right clicked this object
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                Unit hitUnit = hitInfo.collider.gameObject.GetComponent<Unit>();

                if (hitUnit == unit)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
