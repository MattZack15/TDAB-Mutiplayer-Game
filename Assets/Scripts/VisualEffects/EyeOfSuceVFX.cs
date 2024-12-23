using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOfSuceVFX : MonoBehaviour
{
    private float despawnTime = 1f;
    private float timer;
    
    public void InitVFX(float despawnTime)
    {
        this.despawnTime = despawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > despawnTime)
        {
            Destroy(gameObject);
        }
    }
}
