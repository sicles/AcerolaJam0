using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CallOfDuty : MonoBehaviour
{
    private RawImage callOfDuty;

    private void Start()
    {
        callOfDuty = GetComponent<RawImage>();
    }

    public void FlashScreen()
    {
        StartCoroutine(FlashScreenRoutine());
    }

    private IEnumerator FlashScreenRoutine()
    {
        float currentAlpha = 0;

        // flashes screen over 0.25f seconds
        for (int i = 0; i < 49; i++)
        {
            currentAlpha += 0.02f;
            callOfDuty.color = new Color(callOfDuty.color.r, callOfDuty.color.g, callOfDuty.color.b, currentAlpha);
            yield return new WaitForSeconds(0.0025f);
        }
        
        for (int i = 0; i < 49; i++)
        {
            currentAlpha -= 0.02f;
            callOfDuty.color = new Color(callOfDuty.color.r, callOfDuty.color.g, callOfDuty.color.b, currentAlpha);
            yield return new WaitForSeconds(0.0025f);
        }
        
        // reset for safety
        callOfDuty.color = new Color(callOfDuty.color.r, callOfDuty.color.g, callOfDuty.color.b, 0);
    }
}
