using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSoundwave : HomingProjectile
{
    // The wisp soundwave increases in damage based on distance traveled
    int baseDamage;
    [SerializeField] float maxDistance;
    float maxDamageMul;
    float distanceTraveled;
    Vector3 prevPos;

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Color baseColor;
    [SerializeField] Color endColor;

    private void Start()
    {
        prevPos = transform.position;
        distanceTraveled = 0f;
    }
    public void InitWispSoundwave(Tower SourceTower, int damage, Transform target, float maxiumDamageMul)
    {
        InitProjectile(SourceTower, damage, target);
        baseDamage = damage;
        this.maxDamageMul = maxiumDamageMul;
    }


    public override void Update()
    {
        base.Update();


        // Update Distance Traveled
        distanceTraveled += Vector3.Distance(transform.position, prevPos);
        prevPos = transform.position;

        // Update Damage
        float ratio = Mathf.Min(1f, distanceTraveled / maxDistance);
        if (IsServer)
        {
            float damageMul = Mathf.Lerp(1f, maxDamageMul, ratio);
            damage = (int)((float)baseDamage * damageMul);
        }

        // Update Color
        meshRenderer.material.color = Color.Lerp(baseColor, endColor, ratio);
    }
}
