using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerMovement : NetworkBehaviour
{
    [SerializeField] private Transform trackEndPoint;

    public float moveSpeed = 3f;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }
        trackEndPoint = GameObject.FindGameObjectWithTag("TrackEnd").transform;

        Move();
    }

    private void Move()
    {
        if (IsServer)
        {
            Vector3 dir = (trackEndPoint.position - transform.position).normalized;
            dir.y = 0f;

            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }
}
