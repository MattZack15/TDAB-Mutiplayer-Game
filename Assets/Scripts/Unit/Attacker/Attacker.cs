using System.Collections;
using System.Collections.Generic;
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

    // Network Varibles for Sync
    public NetworkVariable<int> maxHp = new NetworkVariable<int>();
    public NetworkVariable<int> hp = new NetworkVariable<int>();

    List<int> maxHpAugments = new List<int>();
    List<float> moveSpeedAugments = new List<float>();
    public List<OnDeathEffect> OnDeathEffects = new List<OnDeathEffect>();

    [SerializeField] AttackerMovement AttackerMovement;


    public void Init(List<Vector3> pathPointPostions, bool callOnEntry = true)
    {
        // Called When Attacker Is Spawned Into the battle
        
        if (!IsServer) { return; }
        
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
        GetComponent<NetworkObject>().Despawn(true);

        foreach (OnDeathEffect deathEffect in OnDeathEffects)
        {
            deathEffect.TriggerEffect();
        }
    }

    public void TakeHit()
    {
        if (!IsServer) return;

        hp.Value -= 1;
    }

    public void RemoveHp(int amount)
    {
        // Call take hit if this is taking damage
        hp.Value -= amount;
    }

    public void AddMaxHp(int amount)
    {
        maxHpAugments.Add(amount);
        hp.Value += amount;
        maxHp.Value += amount;
    }

    public void AddMoveSpeed(float moveSpeedBuff)
    {
        moveSpeedAugments.Add(moveSpeedBuff);
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
        foreach (float buff in moveSpeedAugments)
        {
            CalcedStats.moveSpeed += buff;
        }

        return CalcedStats;
    }


}
