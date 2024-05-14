using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject PlayerSlot;

    [SerializeField] private PlayerHealthManager PlayerHealthManager;
    [SerializeField]
    private Transform layoutGroup;



    public void GeneratePlayerHealthUI()
    {
        int i = 0;
        while (i < PlayerHealthManager.playerIds.Count)
        {
            GameObject newPlayerSlot = Instantiate(PlayerSlot, layoutGroup);
            newPlayerSlot.GetComponent<PlayerHealthSlot>().PopulateSlot(PlayerHealthManager.playerIds[i], PlayerHealthManager.playerHps[i], i, PlayerHealthManager);
            i++;
        }
    }
}
