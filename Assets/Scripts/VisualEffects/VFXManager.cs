using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VFXManager : NetworkBehaviour
{
    [SerializeField] GameObject RebornVFX;
    [SerializeField] GameObject deathParticlesPrefab;
    [SerializeField] GameObject iceBallParticlesPrefab;
    [SerializeField] GameObject arcaneProcParticlesPrefab;
    [SerializeField] GameObject greedyTempestProcParticlesPrefab;
    [SerializeField] GameObject EyeOfSucePrefab;
    [SerializeField] GameObject bleedParticles;

    // Responsible for creating VFX locally

    // FindObjectOfType<VFXManager>().Play()
    // GetComponent<NetworkObject>().NetworkObjectId

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnRebornVFXRPC(ulong networkObjectId)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        if (networkObject == null)
        {
            print("Object Not Found");
        }

        Instantiate(RebornVFX, networkObject.transform);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayUnitAnimRPC(ulong networkObjectId, string animName)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        if (networkObject == null)
        {
            print("Object Not Found");
        }

        Animator animator = networkObject.gameObject.GetComponent<Unit>().Animator;
        
        if (animator == null)
        {
            print("Unit has no animator");
        }

        animator.Play(animName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayDeathParticlesRPC(Vector3 spawnPos, Vector3 colorCode)
    {
        Color color = new Color(colorCode.x, colorCode.y, colorCode.z);
        
        GameObject particlesObj = Instantiate(deathParticlesPrefab, spawnPos, Quaternion.identity);
        particlesObj.transform.rotation = Quaternion.LookRotation(Vector3.up);
        ParticleSystem particleSystem = particlesObj.GetComponent<ParticleSystem>();

        particleSystem.GetComponent<ParticleSystemRenderer>().material.color = color;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayIceBallParticlesRPC(Vector3 spawnPos)
    {

        GameObject particlesObj = Instantiate(iceBallParticlesPrefab, spawnPos, Quaternion.identity);
        particlesObj.transform.rotation = Quaternion.LookRotation(Vector3.up);

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayArcaneProcParticlesRPC(Vector3 spawnPos)
    {

        GameObject particlesObj = Instantiate(arcaneProcParticlesPrefab, spawnPos, Quaternion.identity);
        particlesObj.transform.rotation = Quaternion.LookRotation(Vector3.up);

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayGreedyTempestProcParticlesRPC(Vector3 spawnPos)
    {

        GameObject particlesObj = Instantiate(greedyTempestProcParticlesPrefab, spawnPos, Quaternion.identity);
        particlesObj.transform.rotation = Quaternion.LookRotation(Vector3.up);

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayBleedParticlesRPC(Vector3 spawnPos)
    {
        GameObject particlesObj = Instantiate(bleedParticles, spawnPos, Quaternion.identity);
        particlesObj.transform.rotation = Quaternion.LookRotation(Vector3.up);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnEyeOfSuceRPC(Vector3 spawnPos, float despawnTime)
    {

        GameObject eyeOfSuce = Instantiate(EyeOfSucePrefab, spawnPos, Quaternion.identity);
        eyeOfSuce.GetComponent<EyeOfSuceVFX>().InitVFX(despawnTime);

    }

}
