using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerMovement : NetworkBehaviour
{

    public float moveSpeed = 3f;

    List<Vector3> path;
    Vector3 nextPoint; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
        {
            return;
        }

        // Get Path
        path = FindObjectOfType<PathCreator>().GetPathPoints();
        nextPoint = path[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }


        if (path.Count > 0)
        {
            Move();
            UpdateNextPoint();
        }

    }

    private void Move()
    {
        if (IsServer)
        {
            Vector3 dir = (nextPoint - transform.position).normalized;
            dir.y = 0f;

            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }
    private void UpdateNextPoint()
    {
        if (Vector3.Distance(nextPoint, transform.position) < .1f)
        {
            path.RemoveAt(0);

            if(path.Count > 0)
            {
                nextPoint = path[0];
            }
            
        }
    }
}
