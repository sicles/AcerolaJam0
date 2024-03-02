using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private AI.PrototypeAI _prototypeAI;
    private PlayerScript.PlayerController _playerController;
    public bool IsActive { get; set; }

    public GameObject LastEnemy { get; set; }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerScript.PlayerController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsActive) return;
        
        if (other.transform.GetComponent<AI.PrototypeAI>() != null && other.gameObject != LastEnemy)
        {
            other.transform.GetComponent<AI.PrototypeAI>().TakeDamage(50, other.transform.position, other.transform.rotation.eulerAngles, _playerController.BulletBackcallDirection);
        }

        LastEnemy = other.gameObject;
    }
}
