using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class SuceBloodustEffect : NetworkBehaviour
{
    // Track
    private int procsReamining = 0;
    private List<GameObject> ignoreTargets = new List<GameObject>();

    private Tower tower;
    public float FreezeDuration;

    private void Start()
    {
        if (!IsServer) { return; }
        
        tower = GetComponent<Tower>();
        StartCoroutine(EffectLoop());
    }
    
    public void TriggerEffect(GameObject KilledAttacker)
    {
        if (!IsServer) { return; }
        
        procsReamining += 1;
        ignoreTargets.Add(KilledAttacker);
    }

    IEnumerator EffectLoop() 
    {
        // Used To Buffer Attacks Until there is a Valid Target
        while (true)
        {
            if (procsReamining > 0) 
            {
                if (AttemptEffect())
                {
                    procsReamining -= 1;
                }
            }


            yield return null;
        }
    }

    private bool AttemptEffect()
    {
        // Find a Unit, Show Eye of Suce, Freeze em and Deal damage
        // Find a random unit in range
        GameObject attacker = FindTarget();
        if (attacker == null) { return false;}

        // Show Eye of Suce
        Transform overLayTransform  = attacker.transform.GetChild(0).GetChild(1);
        if (overLayTransform.gameObject.name != "Overlay")
        {
            print(overLayTransform.gameObject.name);
        }
        Vector3 EyePos = overLayTransform.position + new Vector3(0, 0.5f, 0f);

        FindObjectOfType<VFXManager>().SpawnEyeOfSuceRPC(EyePos, FreezeDuration);

        // Freeze Target
        TempMoveSpeedBuff FreezeEffect = attacker.AddComponent<TempMoveSpeedBuff>();
        FreezeEffect.Init(0, FreezeDuration);

        // Deal Damage
        attacker.GetComponent<Attacker>().TakeHit(tower.damage, tower);

        ignoreTargets.Add(attacker);
        StartCoroutine(RemoveTargetFromIgnoreList(FreezeDuration, attacker));
        return true;
    }
    IEnumerator RemoveTargetFromIgnoreList(float freezeDuration, GameObject target) 
    {
        float timer = freezeDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            ignoreTargets.Remove(target);
        }
    }

    private GameObject FindTarget()
    {
        List<Transform> attackerTransforms = tower.AttackersInRange();
        foreach (GameObject ignoreTarget in ignoreTargets)
        {
            if (ignoreTarget != null)
            {
                if (attackerTransforms.Contains(ignoreTarget.transform))
                {
                    attackerTransforms.Remove(ignoreTarget.transform);
                }
            }
        }

        if (attackerTransforms.Count == 0)
        {
            return null;
        }
        GameObject attacker = attackerTransforms[Random.Range(0, attackerTransforms.Count)].gameObject;

        return attacker;
    }
}
