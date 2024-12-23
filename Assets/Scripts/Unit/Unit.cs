using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode.Components;
using UnityEngine;
using static Tribe;

public class Unit : MonoBehaviour
{
    public enum UnitType {Attacker, Tower };

    [Header("Unit Design")]
    [SerializeField] public string UnitName;
    [SerializeField] public int UnitID;
    [SerializeField] public Sprite UnitIcon;
    [SerializeField] public string description;
    [SerializeField] public List<Tribe> tribes;
    [SerializeField] public UnitType unitType;

    [Header("Upgrade")]
    [SerializeField] public GameObject UpgradedVersion;
    [SerializeField] public int level;


    [Header("Serialize for Object Interaction")]
    [SerializeField] Attacker AttackerScript;
    [SerializeField] GameObject HpBar;
    [SerializeField] Tower TowerScirpt;

    [Header("Serialize")]
    [SerializeField] public Animator Animator;


    public void SetInactive()
    {
        // used for when the unit is not in combat
        // They can sit on your board and do nothing

        // So client can pick it up
        NetworkTransform NetworkTransform = GetComponent<NetworkTransform>();
        if (NetworkTransform != null)
        {
            NetworkTransform.enabled = false;
        }

        if (AttackerScript != null)
        {
            GetComponent<AttackerMovement>().enabled = false;
            AttackerScript.enabled = false;
            HpBar.SetActive(false);
        }

        if (TowerScirpt != null)
        {
            TowerScirpt.enabled = false;
        }

    }

    public void SetActive()
    {
        NetworkTransform NetworkTransform = GetComponent<NetworkTransform>();
        if (NetworkTransform != null)
        {
            NetworkTransform.enabled = true;
        }

        if (AttackerScript != null)
        {
            GetComponent<AttackerMovement>().enabled = true;
            AttackerScript.enabled = true;
            HpBar.SetActive(true);
        }

        if (TowerScirpt != null)
        {
            TowerScirpt.enabled = true;
        }

    }

    public bool isTower()
    {
        return TowerScirpt != null;
    }

    public bool isAttacker()
    {
        return AttackerScript != null;
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
