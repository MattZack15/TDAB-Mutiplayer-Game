using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BattleDamageSummary : NetworkBehaviour
{

    [SerializeField] TMP_Text p1NameText;
    [SerializeField] TMP_Text p2NameText;
    [SerializeField] TMP_Text p1ScoreText;
    [SerializeField] TMP_Text p2ScoreText;
    [SerializeField] TMP_Text totalDamageText;
    [SerializeField] TMP_Text loserNameText;
    [SerializeField] TMP_Text loserHealthText;
    [SerializeField] GameObject totalDamageSection;
    [SerializeField] GameObject loserhealthSection;
    [SerializeField] GameObject BattleDamageSummaryContainer;

    ClientRpcParams clientRpcParams;

    private void Start()
    {
        clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { }
            }
        };
    }

    public void PlayOnClient(ulong p1, ulong p2, int p1Score, int p2Score, int damageMin, int loserHealthStart, int loserHealthEnd, ulong clientID)
    {
        clientRpcParams.Send.TargetClientIds = new ulong[] { clientID };
        PlayOnClientRpc(p1, p2, p1Score, p2Score, damageMin, loserHealthStart, loserHealthEnd, clientRpcParams);
    }

    [ClientRpc]
    private void PlayOnClientRpc(ulong p1, ulong p2, int p1Score, int p2Score, int damageMin, int loserHealthStart, int loserHealthEnd, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(Play(p1, p2, p1Score, p2Score, damageMin, loserHealthStart, loserHealthEnd));
    }

    IEnumerator Play(ulong p1, ulong p2, int p1Score, int p2Score, int damageMin, int loserHealthStart, int loserHealthEnd)
    {
        // Assumes P1 is the winner
        // Show breaches for each player // takes 1 second
        float sec1Dur = 1f;
        float sec2Dur = .75f;
        float sec3Dur = .75f;
        float sec4Dur = .75f;
        float timer = 0f;

        string p1ScoreString;
        string p2ScoreString;

        BattleDamageSummaryContainer.SetActive(true);
        totalDamageSection.SetActive(false);
        loserhealthSection.SetActive(false);

        p1ScoreText.SetText("0");
        p2ScoreText.SetText("0");

        // Set Names
        p1NameText.SetText(p1.ToString());
        p2NameText.SetText(p2.ToString());

        yield return new WaitForSeconds(.5f);

        // Load Scores
        while (timer < sec1Dur)
        {
            p1ScoreString = ((int)Mathf.Lerp(0f, (float)p1Score, timer/ sec1Dur)).ToString();
            p2ScoreString = ((int)Mathf.Lerp(0f, (float)p2Score, timer / sec1Dur)).ToString();

            p1ScoreText.SetText(p1ScoreString);
            p2ScoreText.SetText(p2ScoreString);

            timer += Time.deltaTime;
            yield return null;
        }
        p1ScoreString = p1Score.ToString();
        p2ScoreString = p2Score.ToString();
        p1ScoreText.SetText(p1ScoreString);
        p2ScoreText.SetText(p2ScoreString);

        yield return new WaitForSeconds(1f);

        // Tie
        if (p1Score == p2Score)
        {
            BattleDamageSummaryContainer.SetActive(false);
            yield break;
        }

        // Add total Damage
        totalDamageSection.SetActive(true);
        totalDamageText.SetText(damageMin.ToString());

        yield return new WaitForSeconds(.25f);

        timer = 0f;
        // P1 Win
        int maxTotalDamage = damageMin + p1Score;
        while (timer < sec2Dur)
        {
            // Increment Total Damage
            string totalDamageString = ((int)Mathf.Lerp((float)damageMin, (float)maxTotalDamage, timer / sec2Dur)).ToString();
            totalDamageText.SetText(totalDamageString);
            
            // Decrement Player Score
            p1ScoreString = ((int)Mathf.Lerp((float)p1Score,0, timer / sec2Dur)).ToString();
            p1ScoreText.SetText(p1ScoreString);

            timer += Time.deltaTime;
            yield return null;
        }
        totalDamageText.SetText(maxTotalDamage.ToString());
        p1ScoreText.SetText("0");

        yield return new WaitForSeconds(.25f);


        timer = 0f;
        int totalDamage = maxTotalDamage - p2Score;
        while (timer < sec3Dur)
        {
            // Decrement Total Damage
            string totalDamageString = ((int)Mathf.Lerp((float)maxTotalDamage, (float)totalDamage, timer / sec3Dur)).ToString();
            totalDamageText.SetText(totalDamageString);

            // Decrement Player Score
            p2ScoreString = ((int)Mathf.Lerp((float)p2Score, 0, timer / sec3Dur)).ToString();
            p2ScoreText.SetText(p2ScoreString);

            timer += Time.deltaTime;
            yield return null;
        }
        totalDamageText.SetText(totalDamage.ToString());
        p2ScoreText.SetText("0");

        yield return new WaitForSeconds(.5f);
        // Decrement Loser's Health
        loserhealthSection.SetActive(true);
        loserNameText.SetText(p2.ToString() + "'s Health");
        loserHealthText.SetText(loserHealthStart.ToString());
        yield return new WaitForSeconds(.25f);

        timer = 0f;
        string loserHealthString;
        while (timer < sec4Dur)
        {
            // Decrement Hp
            loserHealthString = ((int)Mathf.Lerp((float)loserHealthStart, (float)loserHealthEnd, timer / sec4Dur)).ToString();
            loserHealthText.SetText(loserHealthString);

            // Decrement Total Damage
            string totalDamageString = ((int)Mathf.Lerp((float)totalDamage, (float)0, timer / sec4Dur)).ToString();
            totalDamageText.SetText(totalDamageString);

            timer += Time.deltaTime;
            yield return null;
        }

        loserHealthString = loserHealthEnd.ToString();
        loserHealthText.SetText(loserHealthString);

        yield return new WaitForSeconds(1.5f);
        BattleDamageSummaryContainer.SetActive(false);
    }
}
