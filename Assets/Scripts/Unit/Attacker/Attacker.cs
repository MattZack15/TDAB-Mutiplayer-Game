using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;

public struct AttackerStats
{
    public int baseMaxHp;
    public int maxHp;
    public int hp;
    public float baseMoveSpeed;
    public float moveSpeed;

}

public class Attacker : NetworkBehaviour
{
    [Header("Design Attacker Stats")]
    [SerializeField] protected int health = 1;
    [SerializeField] protected float speed = 1;

    [Header("Graphical")]
    [SerializeField] Color deathParticleColor;

    // Network Varibles for Sync
    public NetworkVariable<int> maxHp = new NetworkVariable<int>();
    public NetworkVariable<int> hp = new NetworkVariable<int>();

    List<int> maxHpAugments = new List<int>();
    List<float> flatMoveSpeedAugments = new List<float>();
    List<float> percentMoveSpeedAugments = new List<float>();
    public List<OnDeathEffect> OnDeathEffects = new List<OnDeathEffect>();

    [SerializeField] AttackerMovement AttackerMovement;

    // Stores The last tower that dealt damage to this attacker
    private Tower lastTowerHitMe;

    public void Init(List<Vector3> pathPointPostions, bool callOnEntry = true)
    {
        // Called When Attacker Is Spawned Into the battle
        
        if (!IsServer) { return; }

        bool hasWalkAnim()
        {
            Animator Animator = GetComponent<Unit>().Animator;
            if (Animator == null)
            {
                print("Animator == null");
                return false;
            }

            if (Animator.runtimeAnimatorController == null)
            {
                return false;
            }
            
            foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "walk")
                {
                    return true;
                }
            }

            return false;
        }

        if (hasWalkAnim())
        {
            FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "walk");
        }

        AttackerMovement.SetPath(pathPointPostions);
        if (callOnEntry)
        {
            OnEntry();
        }
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            maxHp.Value = health;
            hp.Value = health;
        }
    }

    public virtual void OnEntry()
    {
        return;
    }


    public virtual void Update()
    {
        
        if (!IsServer) return;
        CheckDeath();
    }



    private void CheckDeath()
    {
        if (hp.Value <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath()
    {
        if (!IsServer) return;

        FindObjectOfType<VFXManager>().PlayDeathParticlesRPC(transform.position, new Vector3(deathParticleColor.r, deathParticleColor.g, deathParticleColor.b));

        FindObjectOfType<ServerUnitData>().RegisterDeath(gameObject);

        GetComponent<NetworkObject>().Despawn(true);
        // Trigger Death Effects
        foreach (OnDeathEffect deathEffect in OnDeathEffects)
        {
            deathEffect.TriggerEffect();
        }

        // Assign Kill Credit
        if (lastTowerHitMe != null)
        {
            lastTowerHitMe.ReciveKillCredit(gameObject);
        }
        else
        {
            print("Tower is gone? (Kill)");
        }
    }

    public virtual void TakeHit(int damage, Tower damageSource)
    {
        if (!IsServer) return;

        if (!this.enabled) return;

        // Track tower damage (We take min because we don't count overkill damage)
        if (damageSource != null)
        {
            damageSource.ReciveDamageCredit(Mathf.Min(hp.Value, damage));
            lastTowerHitMe = damageSource;
        }
        
        hp.Value -= damage;
    }

    public void RemoveHp(int amount)
    {
        // Call TakeHit instead if this is taking damage
        hp.Value -= amount;
    }

    public void AddMaxHp(int amount)
    {
        maxHpAugments.Add(amount);
        hp.Value += amount;
        maxHp.Value += amount;
    }

    public void AddFlatMoveSpeed(float moveSpeedBuff)
    {
        flatMoveSpeedAugments.Add(moveSpeedBuff);
    }

    public Tuple<List<int>, List<float>> GetBonusStats()
    {
        // Returns twos lists of bonus max hp and bonus flat ms
        // Used for copy bonus stats of an attacker on the side board to the copy of it on the main board
        List<int> bonusMaxHp = new List<int>(maxHpAugments);
        List<float> bonusFlatMS = new List<float>(flatMoveSpeedAugments);

        return new Tuple<List<int>, List<float>>(bonusMaxHp, bonusFlatMS);
    }

    public void RemoveFlatMoveSpeed(float moveSpeedBuff)
    {
        if (!flatMoveSpeedAugments.Contains(moveSpeedBuff))
        {
            print("This does not contain that Buff");
            return;
        }

        flatMoveSpeedAugments.Remove(moveSpeedBuff);
    }

    public void AddPercentMoveSpeed(float moveSpeedBuff)
    {
        percentMoveSpeedAugments.Add(moveSpeedBuff);
    }

    public void RemovePercentMoveSpeed(float moveSpeedBuff)
    {
        if (!percentMoveSpeedAugments.Contains(moveSpeedBuff))
        {
            print("This does not contain that Buff");
            return;
        }

        percentMoveSpeedAugments.Remove(moveSpeedBuff);
    }


    // Use this to read a units stats
    public AttackerStats GetAttackerStats()
    {
        AttackerStats CalcedStats = new AttackerStats();

        CalcedStats.baseMaxHp = health;

        CalcedStats.maxHp = health;
        foreach (int buff in maxHpAugments)
        {
            CalcedStats.maxHp += buff;
        }
        
        
        CalcedStats.hp = hp.Value;
        CalcedStats.baseMoveSpeed = speed;
        
        CalcedStats.moveSpeed = speed;
        foreach (float buff in flatMoveSpeedAugments)
        {
            CalcedStats.moveSpeed += buff;
        }
        foreach (float buff in percentMoveSpeedAugments)
        {
            CalcedStats.moveSpeed *= buff;
        }

        return CalcedStats;
    }

    public static void CopyOverBonusStats(Attacker attacker1, Attacker attacker2)
    {
        
        // Takes Attacker 1's Bonus Stats and Copys them onto Attacker 2
        Tuple<List<int>, List<float>> bonusStats = attacker1.GetBonusStats();
        foreach (int bonusHp in bonusStats.Item1)
        {
            attacker2.AddMaxHp(bonusHp);
        }
        foreach (float bonusMS in bonusStats.Item2)
        {
            attacker2.AddFlatMoveSpeed(bonusMS);
        }
    }


}
