using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace PlayerScript
{
    public partial class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Camera playerCamera;
        [SerializeField] float playerRadius;

        public enum PlayerState { CanMove, CannotMove };
        public enum PlayerView { Freelook, Constrained };
        [Header("PlayerState")]
        [SerializeField] private PlayerState ePlayerState;
        [SerializeField] private PlayerView ePlayerView;
        [SerializeField] private float playerViewConstraints;

        [Header("Physics")]
        [SerializeField] bool isGrounded;
        [SerializeField] Vector3 gravity;   // this needs to be vector3 to avoid a runtime cast
        private readonly float _gravityVelocityMax = 3f;
        [SerializeField] private float gravityAcceleration = 1f;

        [Header("Controls")]
        public KeyCode sprintKey = KeyCode.LeftShift;
        [FormerlySerializedAs("interactionKey")] public KeyCode shootKey = KeyCode.Mouse0;
        public KeyCode abortKey = KeyCode.Space;
        public KeyCode pauseKey = KeyCode.Escape;
        public KeyCode wInput = KeyCode.W;
        public KeyCode sInput = KeyCode.S;
        public KeyCode dInput = KeyCode.A;
        public KeyCode aInput = KeyCode.D;
        public float mouseSensitivity = 1f;
        private float _playerAcceleration;
        [SerializeField] private float defaultPlayerAcceleration = 2.5f;

        [Header("Player Stats")]
        [SerializeField] private Vector2 playerInput;
        private readonly float _lowerLookLimit = 85.0f;
        private readonly float _upperLookLimit = 85.0f;
        private float _xRotation;
        private float _yRotation;
        private bool _overwriteInAction;

        Transform _transformCache;   // does this actually save performance? rider says so, and surely rider isn't lying
        private RaycastHit _viewDirection;
        private bool _thousandYardStare;
        private bool _reloadIsPlaying;

        private void Awake()
        {
            _transformCache = controller.transform;
        }

        void Start()
        {
            controller = this.GetComponent<CharacterController>();
            playerCamera = this.GetComponentInChildren<Camera>();
            _animator = playerCamera.GetComponent<Animator>();
            playerRadius = controller.radius;
            ePlayerState = PlayerState.CanMove;
            _playerAcceleration = defaultPlayerAcceleration;
            playerHealth = _playerMaxHealth;
            Cursor.lockState = CursorLockMode.Locked;
            SetBulletReadyParticleState(false);
        }

        private void Update()
        {
            //PauseGame(); not yet implemented

            CheckIfReloading();
            
            IsGroundedCheck();
            SetGravity();
            BulletTravel();
            
            MouseLook();
            PlayerTilt();
        
            DashTick();
            CallDash();
            Dash();
        
            WasdGetSet();
            Jump();
        
            LetGoOfRack();
            IsLoaded();

            
            ResetRackOnReload();
            Shoot();
            RackGun();
            Reload();

            TickRecallTicker();
            
            // Animation logic
            IsWalking();
            SetGunRacked();
        }

        private void MouseLook()
        {
            _xRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, -_upperLookLimit, _lowerLookLimit);
            _yRotation = Input.GetAxis("Mouse X") * mouseSensitivity;

            playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

            transform.rotation *= Quaternion.Euler(0, _yRotation, 0);

            if (ePlayerView == PlayerView.Constrained)
                if (transform.rotation.eulerAngles.y > playerViewConstraints + 60)
                    transform.rotation = Quaternion.Euler(0, playerViewConstraints + 60, 0);
                else if (transform.rotation.eulerAngles.y < playerViewConstraints - 60)
                    transform.rotation = Quaternion.Euler(0, playerViewConstraints - 60, 0);
        }

        public RaycastHit GetPlayerViewDirection()
        {
            return _viewDirection;
        }

        private void WasdGetSet()
        {
            if (ePlayerState != PlayerState.CanMove) return;
            if (_dashIsActive) return;
        
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            controller.Move(_playerAcceleration * Time.deltaTime * (playerInput.x * _transformCache.right + playerInput.y * _transformCache.forward + gravity.y * _transformCache.up));
        }

        private void IsGroundedCheck()
        {
            isGrounded = Physics.BoxCast(_transformCache.position,
                new Vector3(playerRadius * 0.5f, 0.05f, playerRadius * 0.5f),
                -_transformCache.up,
                _transformCache.rotation,
                (_transformCache.localScale.y * (controller.height / 2)) + 0.2f);
        }

        private void SetGravity()
        {
            if (isGrounded)
            {
                if (gravity.y < 0)
                    gravity.y = 0;
            
                return;
            }
        
            gravity.y -= gravityAcceleration * Time.deltaTime;
            gravity.y = Mathf.Clamp(gravity.y, -_gravityVelocityMax, _jumpForce);
        }
    
        /// <summary>
        /// Set timescale to 0 when pausing
        /// Set timescale to 1 when unpausing
        /// </summary>
        partial void PauseGame()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Time.timeScale = Time.timeScale < 1 ? 1 : 0;
        }

        #region Getters/Setters
        public int SetPlayerState(PlayerState newState)
        {
            if (ePlayerState == newState)
                return 0;

            ePlayerState = newState;
            return 1;
        }
    
        /// for "teleporting" player - will circumvent movement logic for a few frames
        public void SetNewPlayerPosition(Vector3 newPlayerPosition)
        {
            IEnumerator coroutine = MovePlayerWithOverwrite(2, newPlayerPosition); // yes, this needs to be called with 2 frames or else we get race conditions. no, i don't know why
            StartCoroutine(coroutine);
        }

        private IEnumerator MovePlayerWithOverwrite(int amountOfFrames, Vector3 newPlayerPosition)
        {
            _overwriteInAction = true;
            transform.position = newPlayerPosition;

            for (int i = 0;  i < amountOfFrames; i++)
                yield return null;

            transform.position = newPlayerPosition;
            _overwriteInAction = false;
        }

        // reminder that yRotation needs to be given as an absolute value, no matter what the transform shows
        public void SetPlayerView(PlayerView newView, float yRotation)
        {
            playerViewConstraints = yRotation;
            ePlayerView = newView;
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        #endregion
    }
}
