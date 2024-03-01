using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private PrototypeAI _prototypeAI;
    private PlayerController _playerController;

    public GameObject LastEnemy { get; set; }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet collision with " + other.GetComponent<Collider>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<PrototypeAI>() != null && other.gameObject != LastEnemy)
        {
            other.transform.GetComponent<PrototypeAI>().TakeDamage(50, other.transform.position, other.transform.rotation.eulerAngles, _playerController.BulletBackcallDirection);
        }

        LastEnemy = other.gameObject;
    }
}
