using PlayerScript;
using UnityEngine;

public class Bouncey : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // yes this is a hack but i sure as pineapples ain't rewriting the playercontroller at this point
        other.gameObject.GetComponent<PlayerController>().PushPlayer(transform.up, 0.01f);    
    }
}
