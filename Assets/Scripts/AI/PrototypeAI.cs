using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
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

        [SerializeField] bool isAlert;
        [SerializeField] private bool alive = true;

        [SerializeField] private float detectionRadius = 5f;
        private Vector3 _playerDistanceRaw;

        [SerializeField] private StudioEventEmitter idleUnalertSound;
        [SerializeField] private StudioEventEmitter idleAlertSound;

        private void Start()
        {
            health = maxHealth;
            player = FindObjectOfType<CharacterController>().transform;
            _gibThreshold = (int)(-maxHealth * 1.5f);
            _rigidbody = transform.GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.isStopped = true;
            _playerController = player.gameObject.GetComponent<PlayerScript.PlayerController>();
            _playerCamera = player.GetComponentInChildren<Camera>();
            _animator = GetComponent<Animator>();
            idleAlertSound = GetComponents<StudioEventEmitter>()[0];  // yes this means the components' order is important
            idleUnalertSound = GetComponents<StudioEventEmitter>()[1];
        }

        private void Update()
        {
            DecideIdleSound();

            if (alive)
            {
                NavMeshUpdates();
                CooldownTick();
                Seek();
                IsWalking();
                DecideIdleState();
                
                if (isAlert)
                {
                    ShouldDodge();
                    ShouldAttack();
                    ShouldCharge();
                    Charge();
                    IsDodging();
                    Dodge();
                    ChargeCollisonCheck();
                }
            }
        }

        private void NavMeshUpdates()
        {
            _playerDistanceRaw = (player.position - transform.position);
            _agent.destination = player.position;
        }

        /// <summary>
        /// Scanning if player is close and in sight
        /// If successful, turn alert
        /// </summary>
        private void Seek()
        {
            if (isAlert) return;

            if (_playerDistanceRaw.magnitude < detectionRadius)
            {
                if (Physics.Raycast(transform.position, _playerDistanceRaw, detectionRadius, 1 << 7))
                    SetAlert(true);
            }
        }

        /// <summary>
        /// Start chasing player
        /// </summary>
        /// <param name="param_isAlert">New alert state</param>
        private void SetAlert(bool param_isAlert)
        {
            this.isAlert = param_isAlert;
            _agent.isStopped = !param_isAlert;

            if (param_isAlert)
            {
                RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Alerted", transform.gameObject);

                if (!_animator.GetBool(IsHurt)) // getting logic from the animation logic is cringe but i forgive myself
                    ShouldTaunt();
                else
                    enemyIsBusy = false;
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
            if (!isAlert)
                SetAlert(true);

            if (!alive) return;    // TODO remove the gib logic, is obsolete and not working with this new logic
            
            health -= damage;
            Object.Instantiate(bloodGush, position, _playerCamera.transform.rotation);
            CreateBloodDecal();
            CallHurtState();
        
            if (health <= 0)
                Die(bulletDirection);
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

        private void Die(Vector3 bulletDirection)
        {
            alive = false;
            // stop all current attacks
            StopAllCoroutines();
            _chargeIsActive = false;
            
            _playerController.CallHitStop(0.25f);
        
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Death", gameObject);
        
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