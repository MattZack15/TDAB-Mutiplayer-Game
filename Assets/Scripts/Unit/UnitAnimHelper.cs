using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimHelper : MonoBehaviour
{

    private MeleeTower MeleeTowerScript;
    private TowerWithAnim TowerWithAnimScript;

    // Melee Animation Callback
    public void MeleeAnimationCallback()
    {
        if (MeleeTowerScript == null)
        {
            MeleeTowerScript = transform.parent.GetComponent<MeleeTower>();
        }

        MeleeTowerScript.AttackAnimationCallback();
    }

    public void AttackAnimationCallback()
    {
        if (TowerWithAnimScript == null)
        {
            TowerWithAnimScript = transform.parent.GetComponent<TowerWithAnim>();
        }

        TowerWithAnimScript.AttackAnimationCallback();
    }

}
