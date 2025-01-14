using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChronoBallProjectile : Projectile
{
    // This projecttiles gets shot out and travels in a straight line.
    // Then It travels back to its start location

    [SerializeField] private float travelDistance = 5f;
    [SerializeField] Transform model;
    [SerializeField] Vector3 endRotation = new Vector3(0f, 720f, 0f);

    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;
    private Vector3 forwardDirection = Vector3.zero;

    private bool headingForward = true;
    private float timer;

    // Can only hit each attacker once per swing
    private List<GameObject> hitAttackers = new List<GameObject>();


    public override void InitProjectile(Tower SourceTower, int damage, Transform target)
    {
        if (target == null)
        {
            print("Created ChronoBall with no Target");
            return;
        }
        base.InitProjectile(SourceTower, damage, target);
        
        startPos = transform.position;
        // Calculate End Pos
        forwardDirection = target.transform.position - startPos;
        forwardDirection = new Vector3 (forwardDirection.x, 0f, forwardDirection.z).normalized;

        endPos = startPos + forwardDirection * travelDistance;

        InitProjectileClientRPC(startPos, endPos, forwardDirection);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitProjectileClientRPC(Vector3 startPos, Vector3 endPos, Vector3 forwardDirection)
    {
        transform.position = startPos;
        this.startPos = startPos;
        this.endPos = endPos;
        this.forwardDirection = forwardDirection;
        headingForward = true;

        // Set Look Direction
        transform.LookAt(endPos);
    }

    public override void Movement()
    {
        base.Movement();

        timer += Time.deltaTime;

        if (headingForward)
        {
            float progress = timer / speed;
            progress = easeOutSine(progress);

            transform.position = Vector3.Lerp(startPos, endPos, progress);
            model.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, endRotation, progress));

            //transform.position += forwardDirection * speed * Time.deltaTime;

            if (timer / speed >= 1f)
            {
                ChangeDirection();
            }
        }
        else
        {
            float progress = timer / speed;
            // Invert for going backwards
            progress = easeOutSine(1 - progress);

            transform.position = Vector3.Lerp(startPos, endPos, progress);
            model.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, endRotation, progress));
            //transform.position += -forwardDirection * speed * Time.deltaTime;
            if (timer / speed >= 1f)
            {
                Die();
            }
        }
    }

    private void ChangeDirection()
    {
        headingForward = false;
        timer = 0f;
        // When changing directions we can hit any attacker again
        if (IsServer)
        {
            hitAttackers = new List<GameObject>();
        }
    }

    private float easeOutSine(float x) 
    {
        return Mathf.Sin((x* Mathf.PI) / 2);
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }

        if (collision.CompareTag("Attacker"))
        {
            if (hitAttackers.Contains(collision.gameObject)) { return; }

            collision.gameObject.GetComponent<Attacker>().TakeHit(damage, SourceTower);
            hitAttackers.Add(collision.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Do this to make sure we do damage to an enemy if they were inside us when we changed direction
        if (!IsServer) { return; }
        OnTriggerEnter(other);
    }

    public override void Die()
    {
        if (IsServer)
        {
            SourceTower.DestroyProjectile(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


}
