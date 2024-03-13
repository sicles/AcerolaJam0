using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SFXProximityStarter : MonoBehaviour
{
    private bool hasEntered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (hasEntered) return;
        
        RuntimeManager.PlayOneShot("event:/Ascension");
        hasEntered = true;
    }
}
