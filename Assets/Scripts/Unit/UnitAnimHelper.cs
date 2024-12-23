using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimHelper : MonoBehaviour
{

    private MeleeTower MeleeTowerScript;
    // Melee Animation Callback
    public void MeleeAnimationCallback()
    {
        if (MeleeTowerScript == null)
        {
            MeleeTowerScript = transform.parent.GetComponent<MeleeTower>();
        }

        MeleeTowerScript.AttackAnimationCallback();
    }

}
