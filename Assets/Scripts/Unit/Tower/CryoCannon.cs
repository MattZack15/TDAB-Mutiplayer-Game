using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CryoCannon : Tower
{

    [Header("Ice Ball Properties")]
    [SerializeField] float explosionRadius;
    [SerializeField] float slowPercent;
    [SerializeField] float slowDuration;

    [SerializeField] GameObject IceBallPrefab;
    GameObject IceBall;

    [SerializeField] float iceBallGrowTime;
    private bool isAnim;

    void Awake()
    {
        IceBall = Instantiate(IceBallPrefab, projectileSourceLocation);
    }

    public override IEnumerator Attack()
    {
        StartCoroutine(base.Attack());
        
        // Init Projectile with new Specifications
        projectilePool[projectilePool.Count - 1].GetComponent<IceBallProjectile>().Init(this, damage, slowPercent, slowDuration, explosionRadius, currentTarget);


        AnimateBallRPC();

        yield return new WaitForSeconds(attackSpeed);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void AnimateBallRPC()
    {
        if (!isAnim)
        {
            StartCoroutine(AnimBall());
        }
    }

    
    IEnumerator AnimBall()
    {
        isAnim = true;

        Vector3 fullScale = new Vector3(1f, 1f, 1f);

        float timer = 0f;

        while (timer < iceBallGrowTime)
        {
            IceBall.transform.localScale = Vector3.Lerp(Vector3.zero, fullScale, timer / iceBallGrowTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isAnim = false;
    }
}
