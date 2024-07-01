using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EternalKnight : Attacker
{
    // Amount of Buffs recived per Eternal Knight
    public int hpBuff = 1;
    public float moveSpeedBuff = .1f;
    
    ServerUnitData ServerUnitData;
    private int lastEkdCount = 0;
    
    public override void OnDeath()
    {

        ServerUnitData.ekdCount += 1;

        base.OnDeath();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ServerUnitData = FindObjectOfType<ServerUnitData>();
        UpdateStats();
    }

    public override void Update()
    {
        base.Update();

        UpdateStats();
    }

    private void UpdateStats()
    {
        int newBuffs = ServerUnitData.ekdCount - lastEkdCount;

        for (int i = 0; i < newBuffs; i++)
        {
            AddMaxHp(hpBuff);
            AddMoveSpeed(moveSpeedBuff);
        }

        lastEkdCount = ServerUnitData.ekdCount;
    }

}
