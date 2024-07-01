using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class Reborn : OnDeathEffect
{
    UnitDex unitDex;

    private void Awake()
    {
        unitDex = FindObjectOfType<UnitDex>();
    }


    public override void TriggerEffect()
    {
        GameObject UnitPrefab = unitDex.Dex[gameObject.GetComponent<Unit>().UnitID];

        GameObject rebornUnit = Instantiate(UnitPrefab, transform.position, transform.rotation);
        rebornUnit.GetComponent<NetworkObject>().Spawn();

        // Only Reborn Once Reborn
        if (rebornUnit.GetComponent<Reborn>() != null)
        {
            rebornUnit.GetComponent<Reborn>().enabled = false;
        }

        Attacker rebornAttacker = GetComponent<Attacker>();
        rebornAttacker.Init(GetComponent<AttackerMovement>().GetCurrentPath(), false);

        int hpLoss = rebornAttacker.GetAttackerStats().baseMaxHp - 1;

        rebornAttacker.RemoveHp(hpLoss);


    }
}
