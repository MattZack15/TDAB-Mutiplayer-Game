using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    public float range;
    public float attackSpeed;
    public int damage;
    [SerializeField] private bool lookAtTarget = true;

    // Tower Attributes add themselves to this list
    [HideInInspector] public List<TowerAttribute> TowerAttributes = new List<TowerAttribute>();

    [SerializeField] private GameObject projectile;
    [SerializeField] public Transform projectileSourceLocation;

    [HideInInspector] public Transform trackEndPoint;

    protected Transform currentTarget;

    protected List<GameObject> projectilePool = new List<GameObject>();


    // Update is called once per frame
    protected virtual void Update()
    {
        if (!IsServer) { return; }
        if (trackEndPoint == null)
        {
            print("Tower must not have been Init");
            return;
        }


        FindTarget();
        if (lookAtTarget)
        {
            LookAtTarget();
        }
    }

    private void LookAtTarget()
    {
        if (currentTarget == null) return;
        
        Vector3 rotation = transform.eulerAngles;

        transform.LookAt(currentTarget);

        transform.rotation = Quaternion.Euler(rotation.x, transform.rotation.eulerAngles.y, rotation.z);

    }

    public void Init(int boardID)
    {
        // Called at the start of every round
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

    public virtual IEnumerator Attack()
    {
        GameObject newProjectile = Instantiate(projectile, projectileSourceLocation.position, Quaternion.identity);
        newProjectile.GetComponent<NetworkObject>().Spawn();


        newProjectile.GetComponent<Projectile>().InitProjectile(this, damage, currentTarget);
        projectilePool.Add(newProjectile);

        TriggerOnAttackEffects();

        yield return new WaitForSeconds(attackSpeed);
    }

    protected void TriggerOnAttackEffects()
    {
        foreach (TowerAttribute TowerAttribute in TowerAttributes)
        {
            TowerAttribute.OnAttack();
        }
    }

    public void GiveKillCredit(GameObject KilledTarget)
    {
        // Called When an Attacker dies and this tower was the last to hit it
        foreach (TowerAttribute TowerAttribute in TowerAttributes)
        {
            TowerAttribute.OnReciveKillCredit(KilledTarget);
        }
    }

    public void DestroyProjectile(GameObject projectile)
    {
        if (!IsServer) { return; }

        projectilePool.Remove(projectile);

        projectile.SetActive(false);
        StartCoroutine(DelayDespawn(projectile));
    }

    IEnumerator DelayDespawn(GameObject go)
    {
        yield return new WaitForSeconds(1f);

        if (go != null)
        {
            go.GetComponent<NetworkObject>().Despawn();
        }
    }



    protected virtual void FindTarget()
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

            GameObject Attacker = raycastHit.collider.gameObject;
            // Ignore if on a different Board
            if (Attacker.GetComponent<Unit>().GetBoard() != GetComponent<Unit>().GetBoard()) continue;

            attackerTransforms.Add(Attacker.transform);

        }

        return attackerTransforms;
    }

    protected Transform GetClosestUnit(Transform[] enemies)
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
