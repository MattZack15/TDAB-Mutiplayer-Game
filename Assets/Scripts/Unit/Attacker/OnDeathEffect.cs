using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class OnDeathEffect : NetworkBehaviour
{
    public abstract void TriggerEffect();
}
