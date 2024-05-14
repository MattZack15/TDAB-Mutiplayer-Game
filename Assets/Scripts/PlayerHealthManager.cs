using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthManager : NetworkBehaviour
{

    [SerializeField] public int baseMaxHealth;

    public NetworkList<ulong> playerIds;
    public NetworkList<int> playerHps;

    void Awake()
    {
        playerIds = new NetworkList<ulong>();
        playerHps = new NetworkList<int>();
    }


    public void InitPlayerHealth()
    {
        if (!IsServer) return;

        List<ulong> ids = (List<ulong>)NetworkManager.ConnectedClientsIds;

        //ulong[] playerIds = new ulong[ids.Count];
        //int[] playerHps = new int[ids.Count];
        
        int i = 0; 
        foreach (ulong id in ids)
        {
            playerIds.Add(id);
            playerHps.Add(baseMaxHealth);
            i++;
        }
    }
}
