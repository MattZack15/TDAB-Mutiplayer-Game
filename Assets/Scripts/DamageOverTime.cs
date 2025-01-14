using System.Collections;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    static float tickRate = .6f;
    private Attacker attackerScript;
    
    // Gets added to an attacker and constantly applies damage overy tick
    
    public void Init(int totalDamage, float duration, Tower source)
    {
        attackerScript = GetComponent<Attacker>();
        StartCoroutine(DoT(totalDamage, duration, source));
    }

    private IEnumerator DoT(int totalDamage, float duration, Tower source)
    {
        int tickDamage = (int)((float)totalDamage / (duration / tickRate));
        int totalDamageDealt = 0;
        while (totalDamageDealt < totalDamage)
        {
            if (attackerScript.hp.Value <= 0) { yield break; }
            // Deal Damage
            attackerScript.TakeHit(tickDamage, source);
            // Play Particles
            FindObjectOfType<VFXManager>().PlayBleedParticlesRPC(transform.position + new Vector3(0f ,0.5f, 0f));
            totalDamageDealt += tickDamage;
            yield return new WaitForSeconds(tickRate);
        }

        
    }
}
