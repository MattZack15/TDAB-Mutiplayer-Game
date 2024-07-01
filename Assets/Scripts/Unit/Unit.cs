using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static Tribe;

public class Unit : MonoBehaviour
{
    public enum UnitType {Attacker, Tower };

    // Data Container
    [Header("Unit Design")]
    [SerializeField] public string UnitName;
    [SerializeField] public int UnitID;
    [SerializeField] public Sprite UnitIcon;
    [SerializeField] public string description;
    [SerializeField] public List<Tribe> tribes;
    [SerializeField] public UnitType unitType;
    
    [Header("Serialize for Object Interaction")]
    [SerializeField] Attacker AttackerScript;
    [SerializeField] GameObject HpBar;
    [SerializeField] Tower TowerScirpt;

    [HideInInspector] public bool active { get; private set; } = true;

    public void SetInactive()
    {
        // used for when the unit is not in combat
        // They can sit on your board and do nothing

        if (AttackerScript != null)
        {
            GetComponent<AttackerMovement>().enabled = false;
            HpBar.SetActive(false);
        }

        if (TowerScirpt != null)
        {
            TowerScirpt.enabled = false;
        }

        active = false;
    }

    public int GetBoard()
    {
        // Finds Board number where this unit is
        
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + new Vector3(0f, .5f, 0f), Vector3.down * 5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<HexagonTile>() != null)
            {
                return (int)hit.collider.gameObject.GetComponent<HexagonTile>().tileId.z;
            }
        }

        return 0;
    }
}
