using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeviOrb : HomingProjectile
{
    [SerializeField] float bonusHeight = 3f;
    float maxHeight;
    float arcTime = 1f;
    float timer;
    float acceleration = 1f;

    float timeAlive = 0f;

    public override void Update()
    {
        base.Update();
        timeAlive += Time.deltaTime;
    }

    public override void InitProjectile(Tower SourceTower, int damage, Transform target)
    {
        base.InitProjectile(SourceTower, damage, target);

        maxHeight = transform.position.y + bonusHeight;
    }


    public override void Movement()
    {
        Vector3 dir = ((target.position + heightOffset) - transform.position).normalized;
        speed += acceleration * Time.deltaTime;
        Travel(dir);
    }

    protected override void Travel(Vector3 dir)
    {
        timer += Time.deltaTime;
        float progress = timer / arcTime;

        float yPos;

        progress = easeSin(progress);
        yPos = Mathf.Lerp(heightOffset.y, maxHeight, progress);

        transform.position += (Vector3)dir * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        currentDir = dir;
    }

    private float easeSin(float x)
    {
        return Mathf.Sin(x * Mathf.PI);
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }
        if (timeAlive < .25f) { return; }

        if (collision.CompareTag("Attacker"))
        {

            collision.gameObject.GetComponent<Attacker>().TakeHit(damage, SourceTower);

            Die();

        }
    }
}
