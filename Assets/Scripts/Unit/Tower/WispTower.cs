using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class WispTower : TowerWithAnim
{
    [SerializeField] float maxDamageMul;
    [SerializeField] float screamRange;
    [SerializeField] int screamDamage;

    [Header("Scream Animation")]
    [SerializeField] GameObject screamSoundWavePrefab;
    [SerializeField] int numSoundWaves = 4;
    [SerializeField] float soundwaveSpeed = 3f;


    private int screams = 0;

    public void GainBloodLustEffect()
    {
        screams += 1;
        // Capping the number of bloodlusts stored a 1 because its too broken man
        if (screams > 1)
        {
            screams = 1;
        }
    }

    public override IEnumerator Attack()
    {
        if (screams > 0)
        {
            screams -= 1;
            yield return BloodlustAttack();
            yield break;
        }

        // Start Animation
        FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "attack");

        // Wait for apex of attack animation
        float timer = 0f;
        while (!attackAnimCallback && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        attackAnimCallback = false;

        // Attack
        if (currentTarget != null)
        {
            BaseAttackMethod();
            projectilePool[projectilePool.Count - 1].GetComponent<WispSoundwave>().InitWispSoundwave(this, damage, currentTarget, maxDamageMul);
        }

        // Wait Cooldown
        yield return new WaitForSeconds(attackSpeed);
    }
    public IEnumerator BloodlustAttack()
    {
        // Big Scream Attack
        // Play Animation, Send sound waves in every direction
        ScreamAnimationRPC();
        yield return new WaitForSeconds(3f);

        // Get all units in an area
        RaycastHit[] castStar = Physics.SphereCastAll(transform.position, screamRange, transform.up);
        foreach (RaycastHit raycastHit in castStar)
        {
            GameObject hitObj = raycastHit.collider.gameObject;
            Unit hitUnit = hitObj.GetComponent<Unit>();
            // Ingore if not a unit
            if (hitUnit == null) continue;
            // Ignore if on a different Board
            if (hitUnit.GetBoard() != GetComponent<Unit>().GetBoard()) continue;

            // Deal damage to attackers
            if (hitUnit.isAttacker())
            {
                hitUnit.GetComponent<Attacker>().TakeHit(screamDamage, this);
            }
            // Trigger bloodlust of eldritch towers
            else if (hitUnit.isTower())
            {
                // Must have eldritch tower Attribute
                EldritchTowerAttribute eta = hitUnit.GetComponent<EldritchTowerAttribute>();
                if (eta == null) { continue; }
                // Cannot be a wisp tower
                if (hitUnit.UnitName == "Wisp") continue;
                
                eta.GetFreeBloodlust();
            }

        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ScreamAnimationRPC()
    {
        StartCoroutine(ScreamAnimation());
    }

    IEnumerator ScreamAnimation()
    {
        GetComponent<Unit>().Animator.Play("bloodlust");
        yield return new WaitForSeconds(3f);

        float travelTime = screamRange/soundwaveSpeed;

        for (int i = 0; i < numSoundWaves; i++)
        {
            GameObject newSoundWave = Instantiate(screamSoundWavePrefab, transform.position, Quaternion.identity);
            float angle = i * Mathf.PI * 2f / numSoundWaves;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
            StartCoroutine(ScreamSoundWave(newSoundWave, dir, travelTime));
        }
    }

    IEnumerator ScreamSoundWave(GameObject soundwave, Vector3 dir, float aliveTime)
    {
        float timer = 0f;
        
        soundwave.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        while (timer < aliveTime)
        {
            soundwave.transform.position += dir * soundwaveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(soundwave);
    }
}
