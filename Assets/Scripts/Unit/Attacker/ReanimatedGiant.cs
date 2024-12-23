using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class ReanimatedGiant : NetworkBehaviour
{

    [SerializeField] int healthPerUndead = 100;
    [Header("Scale Size")]
    [SerializeField] float minScale = .5f;
    [SerializeField] float scaleIncrement = .05f;

    int orignalHp = 1;

    Attacker attacker;

    private void Awake()
    {
        attacker = GetComponent<Attacker>();
    }

    private void Start()
    {
        orignalHp = attacker.GetAttackerStats().baseMaxHp;
    }

    private void Update()
    {
        // Change Scaling

        int health = attacker.GetAttackerStats().hp;

        // hp = 1000 scale = 1, hp = 2000, scale = Scaling.y

        float scale = 1f;

        if (health >= orignalHp)
        {
            scale += ((float)(health - orignalHp) / (float)healthPerUndead) * scaleIncrement;
        }
        else
        {
            scale = Mathf.Lerp(minScale, 1f, (float)health / (float)orignalHp);
        }

        

        transform.localScale = new Vector3 (1f, 1f, 1f) * scale;
    }

    // When undead die they signal to ServerUnitData 
    // ServerUnitData then signals to applicable Reanimated Giants 

    public void UndeadDied()
    {
        if (!IsServer) { return; }    

        attacker.AddMaxHp(healthPerUndead);

    }
}
