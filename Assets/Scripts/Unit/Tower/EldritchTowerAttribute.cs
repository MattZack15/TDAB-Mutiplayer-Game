using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldritchTowerAttribute : TowerAttribute
{
    // Eldrich Towers, when killing a unit, have a chance to gain their blood lust effect
    // Base - 20%
    // 2 Eldrich Towers - 40%
    // 3 Eldrich Towers - 60%
    // 4 Eldrich Towers - 100%

    [SerializeField] SuceBloodustEffect SuceBloodustEffect;
    
    public static List<float> effectChances = new List<float> { 25f, 40f, 60f, 100f };

    public override void OnReciveKillCredit(GameObject KillTarget)
    {
        float roll = Random.Range(1f, 100f);

        float effectChance = effectChances[GetLevel("Eldritch")-1];

        if (roll < effectChance)
        {
            TriggerBloodlust(KillTarget);
        }
    }

    private void TriggerBloodlust(GameObject KillTarget)
    {
        if (SuceBloodustEffect != null)
        {
            SuceBloodustEffect.TriggerEffect(KillTarget);
        }
    }
}
