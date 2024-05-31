using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    [SerializeField] public int maxHp = 5;
    public NetworkVariable<int> hp = new NetworkVariable<int>();

    
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            hp.Value = maxHp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (hp.Value <= 0)
        {
            GetComponent<NetworkObject>().Despawn(true);   
        }
    }


    public void TakeHit()
    {
        if (!IsServer) return;

        hp.Value -= 1;
    }


}
