using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject PlayerSlot;

    [SerializeField] private PlayerHealthManager PlayerHealthManager;
    [SerializeField] private Transform layoutGroup;

    List<GameObject> playerHealthSlots = new List<GameObject>();

    public void Init()
    {
        ResetPlayerHealthUI();
        GeneratePlayerHealthUI();
    }

    private void Update()
    {
        if (playerHealthSlots.Count != PlayerHealthManager.playerIds.Count)
        {
            ResetPlayerHealthUI();
            GeneratePlayerHealthUI();
        }
    }

    private void ResetPlayerHealthUI()
    {
        foreach (GameObject playerHealthObj in playerHealthSlots)
        {
            Destroy(playerHealthObj);
        }
    }

    public void GeneratePlayerHealthUI()
    {
        int i = 0;
        while (i < PlayerHealthManager.playerIds.Count)
        {
            GameObject newPlayerSlot = Instantiate(PlayerSlot, layoutGroup);
            newPlayerSlot.GetComponent<PlayerHealthSlot>().PopulateSlot(PlayerHealthManager.playerIds[i], PlayerHealthManager.playerHps[i], i, PlayerHealthManager);

            playerHealthSlots.Add(newPlayerSlot);
            i++;
        }
    }
}
