using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerDataManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerDataPrefab;
    public Dictionary<ulong, ServerPlayerData> ServerPlayerDataTable = new Dictionary<ulong, ServerPlayerData>();


    public void Init(List<ulong> clientIDs)
    {
        foreach (ulong clientID in clientIDs)
        {
            GameObject newPlayerDataObj = Instantiate(PlayerDataPrefab, transform);
            ServerPlayerData newServerPlayerData = newPlayerDataObj.GetComponent<ServerPlayerData>();
            newServerPlayerData.Init(clientID);
            newPlayerDataObj.name = $"ServerPlayerData: {clientID}";

            ServerPlayerDataTable.Add(clientID, newServerPlayerData);

        }
    }

    public ServerPlayerData GetPlayerData(ulong clientID)
    {
        return ServerPlayerDataTable[clientID];
    }
}
