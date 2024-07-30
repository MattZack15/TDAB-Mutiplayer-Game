using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IceBallProjectile : Projectile
{

    [SerializeField] float gravity = -9.8f;

    Vector3 EndPos;
    private float yVel;

    float explosionRadius;
    float slowPercent;
    float slowDuration;

    public void Init(Tower SourceTower, int damage, float slowPercent, float slowDuration, float explosionRadius, Transform target)
    {
        base.InitProjectile(SourceTower, damage, target);
        this.explosionRadius = explosionRadius;
        this.slowPercent = slowPercent;
        this.slowDuration = slowDuration;

        Vector3 EndPos = target.position;

        float timeToReachTarget = Vector3.Distance(EndPos, new Vector3(transform.position.x, 0f, transform.position.z)) /speed;

        // y = 0 when t = ttrt a = gravity v = ?

        float StartYVel =  -(transform.position.y/ timeToReachTarget) - (1f/2f)* gravity * timeToReachTarget;

        InitProjectileClientRPC(StartYVel, transform.position, EndPos);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitProjectileClientRPC(float StartYVel,Vector3 StartPos, Vector3 EndPos)
    {
        transform.position = StartPos;
        yVel = StartYVel;
        this.EndPos = EndPos;
        
    }

    public override void Update()
    {
        base.Update();

        if (transform.position.y <= 0f)
        {
            AOE();
            Die();
        }
    }

    public override void Movement()
    {
        Vector3 horizontalDir = (EndPos - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
        Vector3 horizontalMovement = new Vector3(horizontalDir.x, 0, horizontalDir.z) * speed * Time.deltaTime;

        yVel += gravity * Time.deltaTime;

        Vector3 verticalMovement = new Vector3(0f, yVel * Time.deltaTime, 0f);
        //Vector3 verticalMovement = Vector3.zero;

        transform.position += verticalMovement + horizontalMovement;
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }

        if (collision.CompareTag("Attacker"))
        {


            AOE();
            
            Die();

        }
    }

    private void AOE()
    {
        if (!IsServer) { return; }

        Vector3 explodePoint = new Vector3(transform.position.x, 0f, transform.position.z);
        RaycastHit[] castStar = Physics.SphereCastAll(explodePoint, explosionRadius, transform.up);

        foreach (RaycastHit raycastHit in castStar)
        {
            if (raycastHit.collider.gameObject.tag != "Attacker") continue;

            GameObject Attacker = raycastHit.collider.gameObject;
            // Ignore if on a different Board

            Attacker.GetComponent<Attacker>().TakeHit(damage);
            Attacker.AddComponent<TempMoveSpeedBuff>().Init(slowPercent, slowDuration);

        }

    }

    public override void Die()
    {

        if (IsServer)
        {
            FindObjectOfType<VFXManager>().PlayIceBallParticlesRPC(transform.position);
            SourceTower.DestoryProjectile(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

}
