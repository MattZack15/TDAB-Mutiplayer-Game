using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMoveSpeedBuff : MonoBehaviour
{
    public void Init(float buffPercent, float duration)
    {
        StartCoroutine(Apply(buffPercent, duration));
    }

    IEnumerator Apply(float buffPercent, float duration)
    {
        GetComponent<Attacker>().AddPercentMoveSpeed(buffPercent);

        yield return new WaitForSeconds(duration);

        GetComponent<Attacker>().RemovePercentMoveSpeed(buffPercent);

        Destroy(this);
    }
}
