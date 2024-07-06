using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CrumbleDeathEffectHandler : NetworkBehaviour
{
    [SerializeField] float spawnDelay = .5f;    
    
    [SerializeField] List<GameObject> tokenPrefabs = new List<GameObject>();


    public void Init(List<Vector3> path, int boardID)
    {
        StartCoroutine(SpawnTokens(path, boardID));
    }

    IEnumerator SpawnTokens(List<Vector3> path, int boardID)
    {
        
        // Create Tokens
        List<GameObject> tokens = new List<GameObject>();
        foreach (GameObject token in tokenPrefabs) 
        {
            Attacker newAttacker = Instantiate(token, transform.position, transform.rotation).GetComponent<Attacker>();

            newAttacker.gameObject.SetActive(false);

            // Track it
            FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(boardID).AttackerSpawner.TrackNewAttacker(newAttacker.gameObject);

            tokens.Add(newAttacker.gameObject);
        }
        // Release Tokens
        foreach (GameObject token in tokens)
        {

            token.SetActive(true);
            token.GetComponent<NetworkObject>().Spawn();
            token.GetComponent<Attacker>().Init(path);

            yield return new WaitForSeconds(spawnDelay);
        }

        
        Destroy(gameObject);
    }
}
