using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class HomingProjectile : Projectile
{

    protected Transform target;

    protected Vector3 currentDir;

    public override void InitProjectile(Tower SourceTower, int damage, Transform target)
    {
        base.InitProjectile(SourceTower, damage, target);
        
        this.target = target;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!IsServer) { return; }

        if(target == null)
        {
            TravelToNullTarget();
            return;
        }

        base.Update();
    }

    private void TravelToNullTarget()
    {
        transform.position += (Vector3)currentDir * speed * Time.deltaTime;
        if (!willDie)
        {
            Invoke("Die", 3f);
            willDie = true;
        }
    }

    public override void Movement()
    {
        Vector3 dir = ((target.position + heightOffset) - transform.position).normalized;
        Travel(dir);
        dir.y = 0f;
        RotateToDirection(dir);
    }

    protected virtual void Travel(Vector3 dir)
    {
        currentDir = dir;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }









}
