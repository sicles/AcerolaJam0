using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
        [FormerlySerializedAs("isSubmerged")] [SerializeField] private bool isSubmergedAtStart = true;
        [SerializeField] private bool alive = true;
        [SerializeField] private SkinnedMeshRenderer thisSkinnedMeshRenderer;
        [SerializeField] private BoxCollider thisBoxCollider;

        [SerializeField] private float detectionRadius = 5f;
        private Vector3 _playerDistanceRaw;
        [SerializeField] private TrailRenderer chargeTrailRenderer;

        [SerializeField] private EventInstance idleUnalertSound;
        [SerializeField] private EventInstance idleAlertSound;
        private Coroutine _walkToRoutine1;
        private bool _isOrderedToWalk;
        private static readonly int IsClimbing = Animator.StringToHash("IsClimbing");
        private EventInstance _footstepSound;

        public bool Alive => alive;

        private void Start()
        {
            if (isSubmergedAtStart)
                Submerge(true);
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

        private void Submerge(bool isSubmerged)
        {
            thisSkinnedMeshRenderer.enabled = !isSubmerged;
            thisBoxCollider.enabled = !isSubmerged;
        }

        private void Update()
        {
            if (Alive && !_isOrderedToWalk)
            {
                DecideIdleSound();
                DecideFootstepSound();

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
            
            KillTrigger();
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
            _footstepSound = RuntimeManager.CreateInstance("event:/OnEnemyEvents/Footsteps");
            RuntimeManager.AttachInstanceToGameObject(idleAlertSound,  transform);  // does this really need to be set every frame? seems weird
            RuntimeManager.AttachInstanceToGameObject(idleUnalertSound,  transform);
            RuntimeManager.AttachInstanceToGameObject(_footstepSound,  transform);
            
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
                    SetAlert();
            }
        }

        /// <summary>
        /// Start chasing player
        /// </summary>
        /// <param name="paramIsAlert">New alert state</param>
        public void SetAlert()
        {
            if (isAlert) return;
            
            Submerge(false);
            this.isAlert = false;
            _agent.isStopped = true;
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Alerted", transform.gameObject);
            RuntimeManager.PlayOneShotAttached("event:/OnEnemyEvents/Spawn", transform.gameObject);

            _animator.SetBool(IsClimbing, true);
            
            StartCoroutine(AlertRoutine());

        }

        private IEnumerator AlertRoutine()
        {
            yield return new WaitForEndOfFrame(); 
            yield return new WaitForEndOfFrame(); 
            
            _animator.SetBool(IsClimbing, false);

            yield return new WaitForSeconds(5f);    // as long as climbing anim takes

            if (!_animator.GetBool(IsHurt)) // getting logic from the animation logic is cringe but i forgive myself
                ShouldTaunt();
            else
                enemyIsBusy = false;
            
            // all of this should happen after delay
            
            this.isAlert = true;
            _agent.isStopped = false;

            _chargeTicker = chargeReadyOnAlert ? chargeCooldown : 0;
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
                SetAlert();

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

            LayerMask bloodMask = LayerMask.GetMask("Default");
            if (Physics.Raycast(transform.position, _playerCamera.transform.forward, out hit, 50, bloodMask))
            {
                GameObject newDecal  = Instantiate(bloodDecal, 
                                        hit.point + hit.normal * 0.01f,
                                                     Quaternion.LookRotation(hit.normal));  // THIS IS WORKING FINE - if decals are rotated wrong, the prefab is at fault
            }
        }

        private void Die(Vector3 bulletDirection)
        {
            _footstepSound.stop(STOP_MODE.ALLOWFADEOUT);
            idleAlertSound.stop(STOP_MODE.ALLOWFADEOUT);
            idleUnalertSound.stop(STOP_MODE.ALLOWFADEOUT);
            
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

        public void CallWalkTo(Vector3 position, bool destroyOnArrival)
        {
            if (_walkToRoutine1 != null)
                StopCoroutine(_walkToRoutine1);
            
            _walkToRoutine1 = StartCoroutine(WalkToRoutine(position, destroyOnArrival));
        }

        private void KillTrigger()
        {
            if (transform.position.y < -550)
            {
                if (_playerController.Ammunition == 0)
                    _playerController.BulletRecall();
                Debug.Log(this.gameObject + " has been destroyed via KillTrigger()");
                Destroy(this.gameObject);
            }
        }

        private IEnumerator WalkToRoutine(Vector3 position, bool destroyOnArrival)
        {
            _animator.SetBool(Walking, true);
            _isOrderedToWalk = true;
            isAlert = false;
            _agent.isStopped = false;
            _agent.destination = position;
            yield return new WaitUntil(() => CheckWalkToDistance(position));
            Debug.Log("agent has reached destination");
            _agent.isStopped = true;
            _isOrderedToWalk = false;
            _animator.SetBool(Walking, false);
            if (destroyOnArrival) Destroy(this.gameObject);
        }

        private bool CheckWalkToDistance(Vector3 targetPosition)
        {
            return ((transform.position - targetPosition).magnitude < 0.5f);
        }
    }
}