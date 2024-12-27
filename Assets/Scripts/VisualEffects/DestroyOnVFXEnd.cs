using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnVFXEnd : MonoBehaviour
{

    // If all of the children (Particle Effects) are gone then we should Destory this object

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
