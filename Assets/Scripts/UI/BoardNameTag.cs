using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardNameTag : MonoBehaviour
{
    [SerializeField] PlayerBoard playerBoard;
    [SerializeField] TMP_Text text;
    ServerPlayerDataManager ServerPlayerDataManager;


    private void Start()
    {
        ServerPlayerDataManager = FindObjectOfType<ServerPlayerDataManager>();
    }

    // Update is called once per frame
    void Update()
    {

        ServerPlayerData OwnerData = ServerPlayerDataManager.GetPlayerData(playerBoard.owner.Value);
        if (OwnerData == null) { return; }
            
        string ownerName = OwnerData.username.Value.ToString();
        text.SetText($"{ownerName}'s Board");

    }
}
