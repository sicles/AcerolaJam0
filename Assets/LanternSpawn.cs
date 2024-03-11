using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternSpawn : MonoBehaviour
{
    private Animator _lanternSpawn;
    private static readonly int Spawns = Animator.StringToHash("Spawns");

    private void Start()
    {
        _lanternSpawn = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _lanternSpawn.SetBool(Spawns, true);
    }
}
