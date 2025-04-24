using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldritchTowerAttribute : TowerAttribute
{
    // Eldrich Towers, when killing a unit, have a chance to gain their blood lust effect
    // Base - 25%
    // 2 Eldrich Towers - 40%
    // 3 Eldrich Towers - 60%
    // 4 Eldrich Towers - 100%

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

    protected virtual void TriggerBloodlust(GameObject KillTarget)
    {
        // Override must call base for VFX and Sound
        // Find Where to Spawn
        Transform overLayTransform = transform.GetChild(0).GetChild(1);
        if (overLayTransform.gameObject.name != "Overlay")
        {
            print(overLayTransform.gameObject.name);
        }
        Vector3 SpawnPos = overLayTransform.position + new Vector3(0, 0.5f, 0f);
        FindObjectOfType<VFXManager>().SpawnBloodlustVFXRPC(SpawnPos);
        //Play Sound
        AudioManager.Instance.PlayForBoardRPC("bloodlustproc", GetComponent<Unit>().GetBoard(), true);
    }

    public void GetFreeBloodlust()
    {
        // Called by Wisp
        TriggerBloodlust(null);
    }
}
