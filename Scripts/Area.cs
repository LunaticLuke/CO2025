using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area : MonoBehaviour
{

    public string AreaName;
    public int targetCo2;
    public int targetOxygenProd;
    public int currentCo2Level = 0;
    public int currentOxygenProd = 0;
    public int maxCo2Level = 0;
    public bool hq = false;
    public int tickAmount;
    public int tickTimeSeconds;
    [HideInInspector]
    public bool carbonNeutral = false;
    bool ticking = false;
    public int oxygenRegenTick = 2;
    public GameObject critical;
   public bool ableToDeduct = true;

    public void Update()
    {
        if (!ticking && ableToDeduct)
        {
            ticking = true;
            StartCoroutine(tickRemove());
        }

        if(currentCo2Level >= maxCo2Level)
        {
            GameOver(false);
        }
        if(currentCo2Level <= 0 && currentOxygenProd >= targetOxygenProd)
        {
            carbonNeutral = true;
        }
        if(currentCo2Level >= maxCo2Level - tickAmount && !hq)
        {
            critical.SetActive(true);
        }
        else
        {
            if (!hq)
            {
                critical.SetActive(false);
            }
        }
    }

    IEnumerator tickRemove()
    {
        yield return new WaitForSeconds(tickTimeSeconds);
        int currentTick = tickAmount - currentOxygenProd;
        currentCo2Level += currentTick;
        ticking = false;
    }
    public void GameOver(bool win)
    {
        if(win)
        {
            Debug.Log("Won");
        }
        else
        {
            critical.GetComponent<Text>().text = "YOU LOSE";
            critical.SetActive(true);
        }
    }

}
