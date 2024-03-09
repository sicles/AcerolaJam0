using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using FMODUnity;
using PlayerScript;
using UnityEngine;

public class Lover : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerController playerController; 
    
    public void End()
    {
        StartCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        uiManager.Blackout();
        uiManager.StopAllCoroutines();
        playerController.IsEnd = true;  // is end will (hopefully) handle all player triggered sounds except the last shot
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
}
