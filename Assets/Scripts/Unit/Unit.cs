using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Data Container
    [SerializeField] public string UnitName;
    [SerializeField] public int UnitID;
    [SerializeField] public Sprite UnitIcon;

    [SerializeField] Attacker AttackerScript;
    [SerializeField] GameObject HpBar;

    [SerializeField] Tower TowerScirpt;

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
    }
}
