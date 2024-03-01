using System;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.AI;


public class PrototypeAI : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private ParticleSystem bloodGush;
    [SerializeField] private Transform player;
    private NavMeshAgent _agent;
    private Rigidbody _rigidbody;
    private bool _isAlert;
    private readonly float _detectionRadius = 5f;
    private Vector3 _playerDistance;

    private void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
    }

    private void Update()
    {
        Seek();

        _agent.destination = player.position;
    }

    private void Seek()
    {
        if (_isAlert) return;

        _playerDistance = (player.position - transform.position);

        if (_playerDistance.magnitude < _detectionRadius)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _playerDistance, out hit, _detectionRadius, 1 << 7))
                Alert(true);
        }
    }

    private void Alert(bool isAlert)
    {
        _isAlert = isAlert;
        _agent.isStopped = !isAlert;

        if (isAlert)
        {
            EventInstance yodaAlert = FMODUnity.RuntimeManager.CreateInstance("event:/prototypeAlert");
            yodaAlert.start();
        }
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
        EventInstance yodaDeath = FMODUnity.RuntimeManager.CreateInstance("event:/prototypeDeath");
        yodaDeath.start();
        
        Debug.Log("Ouch!");
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(bulletDirection * 1000);
    }
}
