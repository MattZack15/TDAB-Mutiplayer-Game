using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Projectile : NetworkBehaviour
{

    [SerializeField] protected float speed;

    // Defined by Source Tower
    protected int damage;

    protected static Vector3 heightOffset = new Vector3(0f, .5f, 0f);
    

    protected bool willDie = false;

    [HideInInspector] public Tower SourceTower;


    public virtual void Update()
    {
        // Not Server only on purpose
        Movement();
    }

    public virtual void InitProjectile(Tower SourceTower, int damage, Transform target = null)
    {
        this.SourceTower = SourceTower;
        this.damage = damage;
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }

        if (collision.CompareTag("Attacker"))
        {

            collision.gameObject.GetComponent<Attacker>().TakeHit(damage, SourceTower);

            Die();

        }
    }

    public virtual void Movement()
    {
        if (!IsServer) { return; }
    }

    protected void RotateToDirection(Vector3 dir)
    {
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public virtual void Die()
    {
        if (!IsServer) { return; }

        DestoryProjectileClientRPC(transform.position);

        if (SourceTower != null)
        {
            SourceTower.DestroyProjectile(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestoryProjectileClientRPC(Vector3 finalLocation)
    {
        // Clients Only
        if (IsServer) { return; }

        GetComponent<NetworkTransform>().enabled = false;
        StartCoroutine(LerpToLocation(finalLocation));
    }


    IEnumerator LerpToLocation(Vector3 finalLocation)
    {

        Vector3 originalPos = transform.position;

        float distance = (finalLocation - originalPos).magnitude;
        float timeToReach = distance / speed;

        float timer = 0f;
        while (timer < timeToReach)
        {
            transform.position = Vector3.Lerp(originalPos, finalLocation, timer / timeToReach);
            RotateToDirection((finalLocation - originalPos).normalized);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = finalLocation;

        gameObject.SetActive(false);
    }
}
