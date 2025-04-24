using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroyVFX : MonoBehaviour
{
    public float defaultLifeTime;
    private float lifeTime;
    private float timer;
    
    // Life time determined at runtime because eye of suce freeze duration can change
    public void InitVFX(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
