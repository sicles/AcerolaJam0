using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternSpawn : MonoBehaviour
{
    private Animator _lanternSpawn;
    private MeshRenderer _lanternRenderer;
    private static readonly int Spawns = Animator.StringToHash("Spawns");

    private void Start()
    {
        _lanternRenderer = GetComponentInChildren<MeshRenderer>();
        _lanternSpawn = GetComponentInChildren<Animator>();
        _lanternRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _lanternRenderer.enabled = true;
        _lanternSpawn.SetBool(Spawns, true);
    }
}
