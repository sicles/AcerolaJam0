using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    /// <summary>
    /// play destruction animation and sounds
    /// note that this does not actually destroy the object (and it shoudn't)
    /// </summary>
    public void DestroyAnimation()
    {
        transform.position += transform.right * 5;    // this is just for testing
    }
}
