using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerPlayerDataManager : NetworkBehaviour
{
    [SerializeField] GameObject PlayerDataPrefab;
    public Dictionary<ulong, ServerPlayerData> ServerPlayerDataTable = new Dictionary<ulong, ServerPlayerData>();

    private ServerPlayerData myPlayerData;
    public void Init(List<ulong> clientIDs)
    {
        // Create Data Container Object for all Players
        foreach (ulong clientID in clientIDs)
        {
            
            GameObject newPlayerDataObj = Instantiate(PlayerDataPrefab, transform);
            newPlayerDataObj.GetComponent<NetworkObject>().Spawn();
            newPlayerDataObj.transform.parent = transform;

            ServerPlayerData newServerPlayerData = newPlayerDataObj.GetComponent<ServerPlayerData>();
            newServerPlayerData.Init(clientID);
            newPlayerDataObj.name = $"ServerPlayerData: {clientID}";

            ServerPlayerDataTable.Add(clientID, newServerPlayerData);

        }
    }

    public ServerPlayerData GetPlayerData(ulong clientID)
    {
        if (!IsServer) return null;
        return ServerPlayerDataTable[clientID];
    }

    public ServerPlayerData GetMyPlayerData()
    {
        // Client Side | Find a reference to its own Player Data
        if (myPlayerData) return myPlayerData;
        
        ulong myClientID = NetworkManager.Singleton.LocalClientId;
        // Search Cildren
        foreach (Transform child in transform)
        {
            ServerPlayerData data = child.gameObject.GetComponent<ServerPlayerData>();
            if (data.clientID.Value == myClientID)
            {
                myPlayerData = data;
                return data;
            }
        }

        return null;
    }

    public List<ServerPlayerData> GetAllPlayerData()
    {
        List<ServerPlayerData> datas = new List<ServerPlayerData>();
        foreach (Transform child in transform)
        {
            ServerPlayerData data = child.gameObject.GetComponent<ServerPlayerData>();
            datas.Add(data);
        }
        
        return datas;
    }
}
