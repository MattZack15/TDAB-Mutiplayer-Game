using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UnitToolTip : MonoBehaviour
{
    [SerializeField] TMP_Text unitname;
    [SerializeField] Image UnitArt;
    [SerializeField] TMP_Text unitdescription;

    [SerializeField] GameObject towerStats;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text attackSpeedText;
    [SerializeField] TMP_Text rangeText;

    [SerializeField] GameObject attackerStats;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text moveSpeedText;

    [SerializeField] Camera cam;
    [SerializeField] GameObject TribeLabelPrefab;
    [SerializeField] Transform TribeLayoutGroup;
    List<GameObject> TribeList = new List<GameObject>();

    private void Update()
    {
        ToolTipOnUnitCheck();
    }

    public void SetDisplay(Unit unit)
    {
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

        }
        else if (unit.unitType == Unit.UnitType.Attacker)
        {
            attackerStats.SetActive(true);
            towerStats.SetActive(false);

            Attacker attacker = unit.gameObject.GetComponent<Attacker>();
            AttackerStats stats = attacker.GetAttackerStats();

            healthText.SetText($"{stats.baseMaxHp}");
            moveSpeedText.SetText($"{stats.baseMoveSpeed}");
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
}
