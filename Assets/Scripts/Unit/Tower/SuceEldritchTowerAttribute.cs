using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuceEldritchTowerAttribute : EldritchTowerAttribute
{
    
    [SerializeField] SuceBloodustEffect SuceBloodustEffect;
    
    protected override void TriggerBloodlust(GameObject KillTarget)
    {
        base.TriggerBloodlust(KillTarget);
        if (SuceBloodustEffect != null)
        {
            SuceBloodustEffect.TriggerEffect(KillTarget);
        }
    }
}
