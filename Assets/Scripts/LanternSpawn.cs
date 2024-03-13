using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class LanternSpawn : MonoBehaviour
{
    private Animator _lanternSpawn;
    private MeshRenderer _lanternRenderer;
    private static readonly int Spawns = Animator.StringToHash("Spawns");
    private bool _hasBeenTriggered;

    private void Start()
    {
        _lanternRenderer = GetComponentInChildren<MeshRenderer>();
        _lanternSpawn = GetComponentInChildren<Animator>();
        _lanternRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenTriggered) return;
        
        RuntimeManager.PlayOneShotAttached("event:/L02/LanternSpawn", transform.gameObject);
        _lanternRenderer.enabled = true;
        _lanternSpawn.SetBool(Spawns, true);
        _hasBeenTriggered = true;
    }
}
