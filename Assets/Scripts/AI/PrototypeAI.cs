using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

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

        [SerializeField] private Transform player;
        private PlayerScript.PlayerController _playerController;
        private NavMeshAgent _agent;
        private Rigidbody _rigidbody;

        [SerializeField] private bool isAlert;
        [SerializeField] private bool manualAlert;
        [SerializeField] private bool alive = true;

        [SerializeField] private float detectionRadius = 5f;
        private Vector3 _playerDistanceRaw;

        [SerializeField] private EventInstance idleUnalertSound;
        [SerializeField] private EventInstance idleAlertSound;

        public bool Alive => alive;

        private void Start()
        {
            health = maxHealth;
            player = FindObjectOfType<CharacterController>().transform;
            _rigidbody = transform.GetComponent<Rigidbody>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.isStopped = true;
            _playerController = player.gameObject.GetComponent<PlayerScript.PlayerController>();
            _playerCamera = player.GetComponentInChildren<Camera>();
            _animator = GetComponent<Animator>();
            InitiateSoundEvents();
        }

        private void Update()
        {
            DecideIdleSound();
            
            if (Alive)
            {
                NavMeshUpdates();
                CooldownTick();
                if (!manualAlert)
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
        
        private void InitiateSoundEvents()
        {
            idleAlertSound = RuntimeManager.CreateInstance("event:/OnEnemyEvents/IdleAlert");
            idleUnalertSound = RuntimeManager.CreateInstance("event:/OnEnemyEvents/IdleUnalert");
            RuntimeManager.AttachInstanceToGameObject(idleAlertSound,  transform);
            RuntimeManager.AttachInstanceToGameObject(idleUnalertSound,  transform);
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
        /// <param name="paramIsAlert">New alert state</param>
        public void SetAlert(bool paramIsAlert)
        {
            this.isAlert = paramIsAlert;
            _agent.isStopped = !paramIsAlert;

            _chargeTicker = chargeReadyOnAlert ? chargeCooldown : 0;
            
            if (paramIsAlert)
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

            if (!Alive) return;    // TODO remove the gib logic, is obsolete and not working with this new logic
            
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