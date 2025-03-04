using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleIndicatorUI : MonoBehaviour
{

    [SerializeField] GameObject BattleIndicatorObj;
    [SerializeField] TMP_Text attackerNameText;
    [SerializeField] TMP_Text defenderNameText;
    [SerializeField] Animator animator;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;

    [SerializeField] float displayTime = 1f;

    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            BattleIndicatorObj.SetActive(false);
        }
    }

    public void DisplayBattle(ulong attackerID, ulong defenderID)
    {
        BattleIndicatorObj.SetActive(true);
        attackerNameText.SetText(ServerPlayerDataManager.GetPlayerData(attackerID).username.Value.ToString());
        defenderNameText.SetText(ServerPlayerDataManager.GetPlayerData(defenderID).username.Value.ToString());
        animator.Play("bounce");
        timer = displayTime;
    }

}
