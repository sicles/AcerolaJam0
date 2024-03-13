using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class SFXProximityStarter : MonoBehaviour
{
    private bool _hasEntered;
    private EventInstance _ascensionSfx;

    private void Start()
    {
        _ascensionSfx = RuntimeManager.CreateInstance("event:/L01/Ascension");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasEntered) return;

        _ascensionSfx.start();
        _hasEntered = true;
    }

    public void StopAscensionSFX()
    {
        _ascensionSfx.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
