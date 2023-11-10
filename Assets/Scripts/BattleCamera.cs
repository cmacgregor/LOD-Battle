using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    public void FocusSelectedTarget(GameObject target)
    {
        Debug.Log($"focsuing target {target.name}");
        //TODO: implement camera target focus
    }

    public IEnumerator PerformBattleIntro(bool isBossEncounter)
    {
        //TODO: Pan camera around and then show UI
        //  pan camera around
        //  wait for the amount of time (frames?) the camera animation will take

        if (isBossEncounter)
        {
            return BossIntroPan();
        }
        else {
            return BattleIntro();
        }
    }

    private IEnumerator BattleIntro()
    {
        Debug.Log("Battle intro pan");
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator BossIntroPan()
    {
        //TODO: Pan across each character then pan across the bost
        //  wait for the amount of time (frames?) the camera animation will take
        Debug.Log("Boss into pan");
        yield return new WaitForSeconds(2f);
    }
}
