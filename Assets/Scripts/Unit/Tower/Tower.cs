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

    List<GameObject> projectilePool = new List<GameObject>();


    // Update is called once per frame
    void Update()
    {
        if (!IsServer) { return; }
        if (trackEndPoint == null)
        {
            print("Tower must not have been Init");
            return;
        }


        FindTarget();
    }

    public void Init(int boardID)
    {
        trackEndPoint = FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(boardID).EndZone.transform;
        
        StartCoroutine(AttackCycle());
    }

    IEnumerator AttackCycle()
    {
        while (this.enabled)
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


        newProjectile.GetComponent<HomingProjectile>().InitProjectile(this, currentTarget);
        projectilePool.Add(newProjectile);

        yield return new WaitForSeconds(attackSpeed);
    }

    public void DestoryProjectile(GameObject projectile)
    {
        if (!IsServer) { return; }
        
        projectilePool.Remove(projectile);

        projectile.SetActive(false);
        StartCoroutine(DelayDespawn(projectile));
    }

    IEnumerator DelayDespawn(GameObject go)
    {
        yield return new WaitForSeconds(1f);

        go.GetComponent<NetworkObject>().Despawn();
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
