using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class EndingChant : MonoBehaviour
{
    private EventInstance _endingChant;

    private void Start()
    {
        _endingChant = RuntimeManager.CreateInstance("event:/EndingChant");
    }

    private void OnTriggerEnter(Collider other)
    {
        _endingChant.start();
    }

    public void StopChant()
    {
        _endingChant.stop(STOP_MODE.IMMEDIATE);
    }
}
