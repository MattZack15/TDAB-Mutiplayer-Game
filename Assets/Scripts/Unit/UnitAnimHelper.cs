using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimHelper : MonoBehaviour
{
    // Script for the animator to call functions
    
    private Tower towerScript;
    private MeleeTower MeleeTowerScript;
    private TowerWithAnim TowerWithAnimScript;

    public List<string> soundEffectNames;

    private void Start()
    {
        // Unit may not be a tower
        towerScript = transform.parent.gameObject.GetComponent<Tower>();
    }

    // Melee Animation Callback
    public void MeleeAnimationCallback()
    {
        if (MeleeTowerScript == null)
        {
            MeleeTowerScript = transform.parent.GetComponent<MeleeTower>();
        }

        MeleeTowerScript.AttackAnimationCallback();
    }

    public void AttackAnimationCallback()
    {
        if (TowerWithAnimScript == null)
        {
            TowerWithAnimScript = transform.parent.GetComponent<TowerWithAnim>();
        }

        TowerWithAnimScript.AttackAnimationCallback();
    }

    public void AttackSoundTrigger()
    {
        AudioManager.Instance.PlayOnBoard(towerScript.attackSoundName, towerScript.gameObject.GetComponent<Unit>().GetBoard(), true);
    }

    public void PlayAnimationSound(int soundNameIndex)
    {
        AudioManager.Instance.PlayOnBoard(soundEffectNames[soundNameIndex], towerScript.gameObject.GetComponent<Unit>().GetBoard(), true);
    }

}
