using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    public float range;
    public float attackSpeed;

    [SerializeField] private GameObject projectile;

    public Transform trackEndPoint;

    private Transform currentTarget;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitObject());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            FindTarget();
        }
    }

    IEnumerator InitObject()
    {
        while (true)
        { 
            if (IsServer)
            {
                StartCoroutine(AttackCycle());
                yield break;
            }
            if (IsClient && !IsServer)
            {
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator AttackCycle()
    {
        while (true)
        {
            
            if (currentTarget == null)
            {
                yield return null;
                continue;
            }

            yield return Attack();
        }
    }

    IEnumerator Attack()
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        newProjectile.GetComponent<NetworkObject>().Spawn();


        newProjectile.GetComponent<HomingProjectile>().InitProjectile(currentTarget);

        yield return new WaitForSeconds(attackSpeed);
    }



    private void FindTarget()
    {
        // Target Closest To end Point
        List<Transform> attackerTransforms = AttackersInRange();

        currentTarget = GetClosestUnit(attackerTransforms.ToArray());
    }

    public List<Transform> AttackersInRange()
    {
        List<Transform> attackerTransforms = new List<Transform>();

        RaycastHit[] castStar = Physics.SphereCastAll(transform.position, range, transform.up);

        foreach (RaycastHit raycastHit in castStar)
        {
            if (raycastHit.collider.gameObject.tag != "Attacker") continue;

            attackerTransforms.Add(raycastHit.collider.gameObject.transform);

        }

        return attackerTransforms;
    }

    Transform GetClosestUnit(Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        
        Vector3 basePosition = trackEndPoint.position;
        
        foreach (Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - basePosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
