using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackerLimitUI : MonoBehaviour
{
    [SerializeField] SideBoard sideboard;

    [SerializeField] GameObject attackerLimitLabelPrefab;
    List<GameObject> attackerLimitLabels = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (sideboard.GetAttackerLimit() > attackerLimitLabels.Count)
        {
            Transform tileTrans = sideboard.SideBoardGrid.GetTileById(new Vector2(1f, (attackerLimitLabels.Count * 2) + 1)).transform;

            GameObject newattackerLimit = Instantiate(attackerLimitLabelPrefab, tileTrans);
            attackerLimitLabels.Add(newattackerLimit);
            newattackerLimit.transform.GetChild(0).GetComponent<TMP_Text>().text = attackerLimitLabels.Count.ToString();

        }
    }
}
