using System.Collections;
using System.Collections.Generic;
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
}
