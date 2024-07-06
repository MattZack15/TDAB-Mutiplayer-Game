using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

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

        Attacker rebornAttacker = rebornUnit.GetComponent<Attacker>();
        List<Vector3> path = GetComponent<AttackerMovement>().GetCurrentPath();
        rebornAttacker.Init(path, false);

        FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(rebornUnit.GetComponent<Unit>().GetBoard()).AttackerSpawner.TrackNewAttacker(rebornUnit);

        // Only Reborn Once Reborn
        if (rebornUnit.GetComponent<Reborn>() != null)
        {
            rebornUnit.GetComponent<Reborn>().enabled = false;
            rebornUnit.GetComponent<Attacker>().OnDeathEffects.Remove(rebornUnit.GetComponent<Reborn>());
        }

        
        // Start Unit With 1 Hp
        int hpLoss = rebornAttacker.GetAttackerStats().baseMaxHp - 1;

        rebornAttacker.RemoveHp(hpLoss);


    }
}
