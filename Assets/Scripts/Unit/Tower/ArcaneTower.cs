using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneTower : TowerAttribute
{
    // Arcane Towers have a chance of double casting their attack
    // Base - 10%
    // 2 Arcane - 25%
    // 3 Arcane - 50%

    static List<float> effectChances = new List<float> { 10f, 25f, 50f};

    public int level = 0;

    public override void OnAttack()
    {
        // On Attack Roll 

        float roll = Random.Range(1f, 100f);

        float effectChance = effectChances[level];

        if (roll < effectChance)
        {
            StartCoroutine(Effect());
        }
    }

    IEnumerator Effect()
    {
        yield return new WaitForSeconds(.1f);

        FindObjectOfType<VFXManager>().PlayArcaneProcParticlesRPC(Tower.projectileSourceLocation.transform.position);

        StartCoroutine(Tower.Attack());

        yield return null;
    }
}
