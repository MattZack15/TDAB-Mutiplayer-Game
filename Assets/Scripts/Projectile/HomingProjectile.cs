using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HomingProjectile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    private Transform target;

    private Vector3 currentDir;
    private bool willDie = false;

    public Tower SourceTower;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
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
            

            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f;
            currentDir = dir;
            transform.position += (Vector3)dir * speed * Time.deltaTime;
        }
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
        SourceTower.DestoryProjectile(gameObject);
    }


}
