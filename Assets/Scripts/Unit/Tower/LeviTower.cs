using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeviTower : Tower
{
    // Levi Switches between 3 different projectiles each dealing different damage
    [SerializeField] List<GameObject> projectileSeq = new List<GameObject>();
    [SerializeField] List<float> damageMultipliers = new List<float>();

    private float baseAttackSpeed;
    private int baseDamage;


    private int postionInSequence;

    private void Start()
    {
        postionInSequence = 0;
        baseAttackSpeed = attackSpeed;
        baseDamage = damage;
    }

    public override IEnumerator Attack()
    {
        // Switch Projectiles
        projectile = projectileSeq[postionInSequence];
        damage = (int)((float)baseDamage * damageMultipliers[postionInSequence]);

        postionInSequence += 1;
        if (postionInSequence > projectileSeq.Count - 1)
        {
            postionInSequence = 0;
        }

        // Play animation
        FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "attack");

        yield return base.Attack();
    }

    public void GainBloodlustEffect(float attackCooldownReduction)
    {
        // Called by blood lust script to gain attack speed
        attackSpeed -= attackCooldownReduction;

        if (attackSpeed < .1f)
        {
            attackSpeed = .1f;
        }
    }

    public override void OnRoundEnd()
    {
        base.OnRoundEnd();

        // Reset Attack Speed;
        attackSpeed = baseAttackSpeed;
        postionInSequence = 0;
        // Do this so it shows on tool tip correctly
        damage = (int)((float)baseDamage * damageMultipliers[postionInSequence]);
    }
}
