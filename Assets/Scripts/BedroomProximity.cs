using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

public class BedroomProximity : MonoBehaviour
{
    private static readonly int IsBroken = Animator.StringToHash("IsBroken");
    [SerializeField] private PlayerController playerController;
    
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetBool(IsBroken, true);
        StartCoroutine(PushPlayer());
    }

    private IEnumerator PushPlayer()
    {
        for (int i = 0; i < 400; i++)
        {
            playerController.PushPlayer(-transform.forward, 0.1f);
            yield return new WaitForSeconds(0.01f);   // TODO i just realized that WaitForEndOfFrame() waits for update, not for fixedupdate, which makes iframes framerate dependent, which is cringe
        }
    }
}
