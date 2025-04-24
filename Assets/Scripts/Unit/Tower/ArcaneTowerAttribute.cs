using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneTowerAttribute : TowerAttribute
{
    // Arcane Towers have a chance of double casting their attack
    // Base - 10%
    // 2 Arcane - 25%
    // 3 Arcane - 50%

    public static List<float> effectChances = new List<float> { 10f, 25f, 40f};

    public override void OnAttack()
    {
        
        // On Attack Roll 

        float roll = Random.Range(1f, 100f);

        float effectChance = effectChances[GetLevel("Arcane")-1];

        if (roll < effectChance)
        {
            StartCoroutine(Effect());
        }
    }

    IEnumerator Effect()
    {
        yield return new WaitForSeconds(.1f);

        FindObjectOfType<VFXManager>().PlayArcaneProcParticlesRPC(Tower.projectileSourceLocation.transform.position);
        AudioManager.Instance.PlayForBoardRPC("arcaneproc", GetComponent<Unit>().GetBoard(), true);

        StartCoroutine(Tower.Attack());

        yield return null;
    }
}
