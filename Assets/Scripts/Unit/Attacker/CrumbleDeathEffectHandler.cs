using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CrumbleDeathEffectHandler : NetworkBehaviour
{
    [SerializeField] float spawnDelay = .5f;    
    
    [SerializeField] List<GameObject> tokens = new List<GameObject>();


    public void Init(List<Vector3> path)
    {
        StartCoroutine(SpawnTokens(path));
    }

    IEnumerator SpawnTokens(List<Vector3> path)
    {
        
        foreach (GameObject token in tokens) 
        {
            Attacker newAttacker = Instantiate(token, transform.position, transform.rotation).GetComponent<Attacker>();
            newAttacker.GetComponent<NetworkObject>().Spawn();
            newAttacker.Init(path);

            yield return new WaitForSeconds(spawnDelay);
        }
        
        Destroy(gameObject);
    }
}
