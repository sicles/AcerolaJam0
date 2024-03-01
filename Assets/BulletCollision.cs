using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private EnemyHealth _enemyHealth;
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
        if (other.transform.GetComponent<EnemyHealth>() != null && other.gameObject != LastEnemy)
        {
            other.transform.GetComponent<EnemyHealth>().TakeDamage(50, other.transform.position, other.transform.rotation.eulerAngles, _playerController.BulletBackcallDirection);
        }

        LastEnemy = other.gameObject;
    }
}
