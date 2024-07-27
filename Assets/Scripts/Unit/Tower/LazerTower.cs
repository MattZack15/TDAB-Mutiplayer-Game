using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LazerTower : Tower
{
    [SerializeField] private int chargedDamage;
    
    [SerializeField] GameObject ParticleSystemPrefab;
    private GameObject Beam;

    Transform lastTarget;
    private bool changedTarget;


    private float beamParticleStartingSize;

    private NetworkVariable<float> activeBeamTimer = new NetworkVariable<float>();
    public float chargeTime = 3f;
    private Transform localTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        Beam = Instantiate(ParticleSystemPrefab, transform);

        Material material = Beam.GetComponent<ParticleSystemRenderer>().material;

        material.color = Color.red;

        beamParticleStartingSize = Beam.GetComponent<ParticleSystem>().main.startSize.constantMax;

        Beam.GetComponent<ParticleSystemRenderer>().material = material;
    }

    // Update is called once per frame
    protected override void Update()
    {
        DrawBeam();

        base.Update();

        if (!IsServer) { return; }

        TrackBeamTime();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncTargetClientRPC(ulong targetID)
    {

        if (targetID == 0)
        {
            localTarget = null;
            return;
        }

        changedTarget = true;

        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetID, out networkObject);

        localTarget = networkObject.gameObject.transform;
    }

    private void TrackBeamTime()
    {
        activeBeamTimer.Value += Time.deltaTime;

        // When Target Changes or dies reset
        if (lastTarget != currentTarget)
        {
            activeBeamTimer.Value = 0f;
            lastTarget = currentTarget;
            changedTarget = true;
            
            if (currentTarget == null)
            {
                SyncTargetClientRPC(0);
            }
            else
            {
                SyncTargetClientRPC(currentTarget.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
            }
            
        }

        // Dont Charge while no target
        if (currentTarget == null)
        {
            activeBeamTimer.Value = 0f;
        }
    }

    protected override void FindTarget()
    {
        // Target Selection:
        // If we have no target find the target closest to end
        // If we have a target, remain on that target until it leaves range

        List<Transform> attackerTransforms = AttackersInRange();

        if (currentTarget == null)
        {
            currentTarget = GetClosestUnit(attackerTransforms.ToArray());
        }
        else
        {
            if (!attackerTransforms.Contains(currentTarget))
            {
                currentTarget = GetClosestUnit(attackerTransforms.ToArray());
            }
        }

    }

    protected override IEnumerator Attack()
    {
        int beamDamage = GetBeamDamage();

        currentTarget.gameObject.GetComponent<Attacker>().TakeHit(beamDamage);

        yield return new WaitForSeconds(attackSpeed);
    }

    private int GetBeamDamage()
    {
        return (int)Mathf.Lerp((float)damage, (float)chargedDamage, activeBeamTimer.Value / chargeTime);
    }

    private void DrawBeam()
    {
        
        // Beam On or Off
        if (localTarget == null)
        {
            Beam.SetActive(false);
            return;
        }
        Beam.SetActive(true);

        // Beam Starting Pos
        Beam.transform.position = projectileSourceLocation.position;

        ParticleSystem beamParticles = Beam.GetComponent<ParticleSystem>();

        // Restart Anim if new targtet
        if (changedTarget)
        {
            beamParticles.Clear();
            beamParticles.Play();

            changedTarget = false;
        }

        // Calculate startLifetime to make beam end at target
        Vector3 targetPos = localTarget.position + new Vector3(0f, .5f, 0f);
        float distanceToTarget = Vector3.Distance(targetPos, projectileSourceLocation.position);
        float particleSpeed = beamParticles.main.startSpeed.constantMax;
        float requiredLifetime = distanceToTarget / particleSpeed;
        var main = beamParticles.main;
        main.startLifetime = requiredLifetime;

        // Aim At target
        Beam.transform.LookAt(targetPos);

        // Beam Size
        main.startSize = beamParticleStartingSize * Mathf.Lerp(1f, 3f, activeBeamTimer.Value / chargeTime);


    }

    private void OnDisable()
    {
        if (Beam)
        {
            Beam.SetActive(false);
        }
    }
}
