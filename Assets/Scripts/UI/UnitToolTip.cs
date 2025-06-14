using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Netcode;

public class UnitToolTip : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] TMP_Text unitname;
    [SerializeField] Image UnitArt;
    [SerializeField] TMP_Text unitdescription;

    [Header("Tower")]
    [SerializeField] GameObject towerStats;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text attackSpeedText;
    [SerializeField] TMP_Text rangeText;
    [SerializeField] GameObject trackedStatsObj;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text damageDealtText;

    [Header("Attacker")]
    [SerializeField] GameObject attackerStats;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text moveSpeedText;

    [Header("Other")]
    [SerializeField] GameObject container;
    [SerializeField] Camera cam;
    [SerializeField] GameObject TribeLabelPrefab;
    [SerializeField] Transform TribeLayoutGroup;
    List<GameObject> TribeList = new List<GameObject>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Close Tool Tip
            HidePanel();
        }
        ToolTipOnUnitCheck();
    }

    public void SetDisplay(Unit unit)
    {
        container.SetActive(true);
        
        unitname.SetText(unit.UnitName);
        UnitArt.sprite = unit.UnitIcon;
        unitdescription.SetText(unit.description);

        // Tribes
        foreach (GameObject go in TribeList)
        {
            Destroy(go);
        }
        foreach (Tribe tribe in unit.tribes) 
        {
            TribeLabel tribeLabel = Instantiate(TribeLabelPrefab, TribeLayoutGroup).GetComponent<TribeLabel>();
            tribeLabel.SetDisplay(tribe);
            TribeList.Add(tribeLabel.gameObject);
        }

        if (unit.unitType == Unit.UnitType.Tower)
        {
            towerStats.SetActive(true);
            attackerStats.SetActive(false);

            Tower tower = unit.gameObject.GetComponent<Tower>();
            damageText.SetText($"{tower.damage}");
            attackSpeedText.SetText($"{ tower.attackSpeed}");
            rangeText.SetText($"{tower.range}");

            if (unit.gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                trackedStatsObj.SetActive(true);
                killsText.SetText($"{tower.kills.Value}");
                damageDealtText.SetText($"{tower.damageDealt.Value}");
            }
            else 
            {
                trackedStatsObj.SetActive(false);
            }

        }
        else if (unit.unitType == Unit.UnitType.Attacker)
        {
            attackerStats.SetActive(true);
            towerStats.SetActive(false);

            Attacker attacker = unit.gameObject.GetComponent<Attacker>();
            AttackerStats stats = attacker.GetAttackerStats();

            if (unit.gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                healthText.SetText($"{attacker.hp.Value}");
                // Correct on Server Side Only
                moveSpeedText.SetText($"{stats.moveSpeed}");
            }
            else
            {
                healthText.SetText($"{stats.baseMaxHp}");
                moveSpeedText.SetText($"{stats.baseMoveSpeed}");
            }
        }
    }

    private void ToolTipOnUnitCheck()
    {
        // When right clicking a unit see its tool tip
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                Unit hitUnit = hitInfo.collider.gameObject.GetComponent<Unit>();

                if (hitUnit != null)
                {
                    SetDisplay(hitUnit);
                }
            }
        }
    }

    public void HidePanel()
    {
        container.SetActive(false);
    }


}
