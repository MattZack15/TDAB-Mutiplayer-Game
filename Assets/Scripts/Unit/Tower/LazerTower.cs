using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerTower : Tower
{
    [SerializeField] GameObject ParticleSystemPrefab;
    private GameObject Beam;

    Transform lastTarget;

    private bool beamIsActive;
    
    // Start is called before the first frame update
    void Start()
    {
        Beam = Instantiate(ParticleSystemPrefab);

        Material material = Beam.GetComponent<ParticleSystemRenderer>().material;

        print(material.color);

        material.color = Color.red;


        Beam.GetComponent<ParticleSystemRenderer>().material = material;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        DrawBeam();
    }

    protected override IEnumerator Attack()
    {

        currentTarget.gameObject.GetComponent<Attacker>().TakeHit();

        yield return new WaitForSeconds(attackSpeed);
    }

    private void DrawBeam()
    {
        // Beam On or Off
        if (currentTarget == null)
        {
            Beam.SetActive(false);
            return;
        }
        Beam.SetActive(true);

        // Beam Starting Pos
        Beam.transform.position = transform.position;

        // Calculate startLifetime to make beam end at target
        float distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);
        ParticleSystem beamParticles = Beam.GetComponent<ParticleSystem>();
        float particleSpeed = beamParticles.main.startSpeed.constantMax;
        float requiredLifetime = distanceToTarget / particleSpeed;
        var main = beamParticles.main;
        main.startLifetime = requiredLifetime;

        // Aim At target
        Beam.transform.LookAt(currentTarget.position);

        // Color it

        // Restart Anim if new targtet
        if (lastTarget != currentTarget)
        {
            beamParticles.Clear();
            beamParticles.Play();
            lastTarget = currentTarget;
        }
    }
}
