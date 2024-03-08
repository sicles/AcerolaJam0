using PlayerScript;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    private void OnTriggerEnter(Collider other)
    {
        playerController.ActivateGun(true);
        // TODO add an animation where you grab the gun and it looks very very cool
    }
}
