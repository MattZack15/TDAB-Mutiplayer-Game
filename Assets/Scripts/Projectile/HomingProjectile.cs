using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class HomingProjectile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    private static Vector3 heightOffset = new Vector3 (0f, .5f, 0f);

    private Transform target;

    private Vector3 currentDir;
    private bool willDie = false;

    public Tower SourceTower;
    
    // Update is called once per frame
    void Update()
    {
        if (!IsServer) { return; }

        if(target == null)
        {
            transform.position += (Vector3)currentDir * speed * Time.deltaTime;
            if (!willDie)
            {
                Invoke("Die", 3f);
                willDie = true;
            }
                
            return;
        }

        Vector3 dir = ((target.position+ heightOffset) - transform.position).normalized;
        Travel(dir);
        RotateToDirection(dir);

    }

    private void Travel(Vector3 dir)
    {
        
        dir.y = 0f;
        currentDir = dir;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    private void RotateToDirection(Vector3 dir)
    {
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void InitProjectile(Tower SourceTower, Transform target = null)
    {
        this.target = target;
        this.SourceTower = SourceTower;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }

        if (collision.CompareTag("Attacker"))
        {
            
            collision.gameObject.GetComponent<Attacker>().TakeHit();

            Die();

        }
    }

    private void Die()
    {
        if (!IsServer) { return; }

        DestoryProjectileClientRPC(transform.position);
        
        if (SourceTower != null)
        {
            SourceTower.DestoryProjectile(gameObject);
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
            transform.position = Vector3.Lerp(originalPos, finalLocation, timer/timeToReach);
            RotateToDirection((finalLocation - originalPos).normalized);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = finalLocation;

        gameObject.SetActive(false);
    }





}
