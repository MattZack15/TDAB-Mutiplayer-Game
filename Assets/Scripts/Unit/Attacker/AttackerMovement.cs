using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerMovement : NetworkBehaviour
{

    public float moveSpeed = 3f;

    List<Vector3> path = new List<Vector3>();
    Vector3 nextPoint; 
    

    public void SetPath(List<Vector3> pathPointPostions)
    {
        path = pathPointPostions;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;
        
        // if no path Destory object
        if (path == null || path.Count == 0)
        {
            print("Error: Unit Was Spawned With no Path");
            GetComponent<NetworkObject>().Despawn();
            return;
        }
        
        nextPoint = path[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;


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
