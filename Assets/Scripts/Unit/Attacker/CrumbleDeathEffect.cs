using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleDeathEffect : OnDeathEffect
{
    [SerializeField] GameObject handlerPrefab;

    public override void TriggerEffect()
    {
        CrumbleDeathEffectHandler handler = Instantiate(handlerPrefab, transform.position, transform.rotation).GetComponent<CrumbleDeathEffectHandler>();
        
        handler.Init(GetComponent<AttackerMovement>().GetCurrentPath(), GetComponent<Unit>().GetBoard());

    }
}
