using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private ParticleSystem bloodGush;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage, Vector3 position, Vector3 rotation, Vector3 bulletDirection)
    {
        if (health - damage > 0)
        {
            health -= damage;
            Instantiate(bloodGush, position, Quaternion.LookRotation(rotation));    // i have no idea why this works for the rotation ¯\_(ツ)_/¯
            Debug.Log("instantiated");
        }
        else
            Die(bulletDirection);
    }

    private void Die(Vector3 bulletDirection)
    {
        Debug.Log("Ouch!");
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(bulletDirection * 1000);
    }
}
