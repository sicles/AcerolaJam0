using System;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace AI
{
    public partial class PrototypeAI : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int health = 100;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private ParticleSystem bloodGush;
        [SerializeField] private GameObject bloodDecal;
        private Camera _playerCamera;
        private int _gibThreshold;

        [SerializeField] private Transform player;
        private PlayerScript.PlayerController _playerController;
        private NavMeshAgent _agent;
        private Rigidbody _rigidbody;

        private bool _isAlert;
        private bool _alive = true;

        [SerializeField] private float detectionRadius = 5f;
        private Vector3 _playerDistance;

        private void Start()
        {
            health = maxHealth;
            _gibThreshold = (int)(-maxHealth * 1.5f);
            _rigidbody = transform.GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.isStopped = true;
            _playerController = player.gameObject.GetComponent<PlayerScript.PlayerController>();
            _playerCamera = player.GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            if (_alive)
            {
                NavMeshUpdates();
                CooldownTick();
                Seek();
                
                if (_isAlert)
                {
                    ShouldDodge();
                    ShouldAttack();
                    ShouldCharge();
                    Charge();
                    Dodge();
                    ChargeCollisonCheck();
                }
            }
        }

        private void NavMeshUpdates()
        {
            _playerDistance = (player.position - transform.position);
            _agent.destination = player.position;
        }

        /// <summary>
        /// Scanning if player is close and in sight
        /// If successful, turn alert
        /// </summary>
        private void Seek()
        {
            if (_isAlert) return;

            if (_playerDistance.magnitude < detectionRadius)
            {
                if (Physics.Raycast(transform.position, _playerDistance, detectionRadius, 1 << 7))
                    Alert(true);
            }
        }

        /// <summary>
        /// Start chasing player
        /// </summary>
        /// <param name="isAlert"></param>
        private void Alert(bool isAlert)
        {
            _isAlert = isAlert;
            _agent.isStopped = !isAlert;

            if (isAlert)
            {
                EventInstance yodaAlert = FMODUnity.RuntimeManager.CreateInstance("event:/prototypeAlert");
                yodaAlert.start();
                ShouldTaunt();
            }
        }

        /// <summary>
        /// Take damage and spawn blood particle.
        /// Die if health stoops below 0.
        /// Enemy is alerted.
        /// Gib if below gib threshold.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="position">Position where hit was applied.</param>
        /// <param name="rotation">Normal rotation of hit mesh face.</param>
        /// <param name="bulletDirection">Direction bullet was traveling at hit time.</param>
        public void TakeDamage(int damage, Vector3 position, Vector3 rotation, Vector3 bulletDirection)
        {
            if (!_isAlert)
                Alert(true);
        
            health -= damage;
            Object.Instantiate(bloodGush, position, _playerCamera.transform.rotation);
            CreateBloodDecal();
            CallHurtState();
        
            if (_alive && health <= 0)
                Die(bulletDirection);
        
            if (!_alive && health <= _gibThreshold)  // no else because overkills are possible 
                Gib();
        }

        private void CreateBloodDecal()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _playerCamera.transform.forward, out hit, 50))
            {
                GameObject newDecal  = Instantiate(bloodDecal, 
                                        hit.point + hit.normal * 0.01f,
                                                     Quaternion.LookRotation(hit.normal));  // THIS IS WORKING FINE - if decals are rotated wrong, the prefab is at fault
            }
        }

        private void Gib()
        {
            _playerController.SetBulletFree();
     
            // DO NOT DESTROY WITHOUT FREEING BULLET FIRST 
            Object.Destroy(transform.gameObject);
        }

        private void Die(Vector3 bulletDirection)
        {
            _alive = false;
            // stop all current attacks
            StopAllCoroutines();
            _chargeIsActive = false;
        
            EventInstance yodaDeath = FMODUnity.RuntimeManager.CreateInstance("event:/prototypeDeath");
            yodaDeath.start();
        
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(bulletDirection * 100);

            DeactivateEnemy();
        }

        private void DeactivateEnemy()
        {
            _agent.enabled = false;
        }
    }
}