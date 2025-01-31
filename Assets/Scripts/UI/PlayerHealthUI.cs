using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject PlayerSlot;

    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] private Transform layoutGroup;

    List<GameObject> playerHealthSlots = new List<GameObject>();

    public void Init()
    {
        ResetPlayerHealthUI();
        GeneratePlayerHealthUI();
    }

    private void Update()
    {
        if (playerHealthSlots.Count != ServerPlayerDataManager.GetAllPlayerData().Count)
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
        List<ServerPlayerData> playerDatas = ServerPlayerDataManager.GetAllPlayerData();
        foreach (ServerPlayerData playerData in playerDatas)
        {
            GameObject newPlayerSlot = Instantiate(PlayerSlot, layoutGroup);
            newPlayerSlot.GetComponent<PlayerHealthSlot>().PopulateSlot(playerData);

            playerHealthSlots.Add(newPlayerSlot);
        }
    }
}
