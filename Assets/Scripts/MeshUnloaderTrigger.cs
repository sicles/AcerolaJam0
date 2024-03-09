using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUnloaderTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> stuffToUnload;
    [SerializeField] private bool hasBeenTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        hasBeenTriggered = true;
        
        foreach (var stuff in stuffToUnload)
        {
            stuff.SetActive(false);
        }
    }
}
