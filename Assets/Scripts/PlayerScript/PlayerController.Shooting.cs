using UnityEngine;

namespace PlayerScript
{
    public partial class PlayerController
    {
        private readonly float _shootRange = 50;
        [SerializeField] private int ammunition;
        [SerializeField] private int maxAmmunition = 1;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float bulletBackcallThresholdDistance = 0.1f;
        [SerializeField] private float bulletTravelSpeed = 3;
        private bool _bulletIsTraveling;
        private readonly float _bulletRotationSpeed = 1000;
    
        [SerializeField] private bool _gunIsRacked;
        private float _rackTargetCharge = 2f;
        [SerializeField] private float _rackCharge;
        private bool _rackIsReady = true;
        [SerializeField] private float spreadAmount = 1;
        public Vector3 BulletBackcallDirection { get; private set; }
    
        partial void Shoot();
        partial void Reload();
        partial void AbortInput();  // not implemented
        partial void PauseGame();   // not checked in update **yet** because this is really annoying while editing

        partial void Shoot()
        {
            if (ammunition == 0) return;
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            if (!_gunIsRacked)
            {
                //TODO logic if gun fails to shoot
                Debug.Log("Shoot attempt was made, but gun is not racked");
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position + playerCamera.transform.forward, CalculateShotDirection());
        
            if (Physics.Raycast(ray, out RaycastHit hit, _shootRange))
            {
                bullet.GetComponent<BulletCollision>().LastEnemy = hit.transform.gameObject;
            
                bullet.transform.SetParent(null, true);
                bullet.transform.GetComponent<MeshRenderer>().enabled = true;
                ammunition--;
                _rackCharge = 0;
                _rackIsReady = false;
                _gunIsRacked = false;
            
                AttachBullet(hit);

                if (hit.collider.gameObject.TryGetComponent<AI.PrototypeAI>(out AI.PrototypeAI prototypeAI))
                    prototypeAI.TakeDamage(50, hit.transform.position, hit.normal, playerCamera.transform.forward);
            }
        }

        /// <summary>
        /// Calculate movement dependent spread.
        /// </summary>
        private Vector3 CalculateShotDirection()
        {
            float upRng = Random.Range(-1f, 1f);
            float rightRng = Random.Range(-1f, 1f);

            Vector3 normalizedSpread = (playerCamera.transform.up * upRng * playerInput.y 
                                        + playerCamera.transform.right * rightRng * playerInput.x).normalized;

            return (playerCamera.transform.forward + normalizedSpread * (spreadAmount * Mathf.Clamp(Mathf.Abs(gravity.y), 1, 2 )));
        }

        /// <summary>
        /// Attach bullet to hit object.
        /// </summary>
        /// <param name="hit"></param>
        private void AttachBullet(RaycastHit hit)
        {
            bullet.transform.SetParent(hit.transform.gameObject.transform, true);
            bullet.transform.rotation = playerCamera.transform.rotation;
            bullet.transform.position = hit.point;
        }

        partial void Reload()
        {
            if (ammunition >= maxAmmunition) return;
            if (!Input.GetKeyDown(KeyCode.R)) return;

            BulletRecall();
        }

        private void RackGun()
        {
            if (_gunIsRacked || !_rackIsReady) return;

            if (Input.GetKey(KeyCode.Mouse1))
                _rackCharge += Time.deltaTime;
            else
                _rackCharge = 0;    
        
            if (_rackCharge >= _rackTargetCharge)
                _gunIsRacked = true;
        }

        /// <summary>
        /// Called in update and forces player to let go of the button after shooting once.
        /// </summary>
        private void LetGoOfRack()
        {
            if (!Input.GetKey(KeyCode.Mouse1))
                _rackIsReady = true;
        }

        /// <summary>
        /// Called once to start bullet travel back to player.
        /// </summary>
        private void BulletRecall()
        {
            bullet.transform.GetComponent<BulletCollision>().IsActive = true;
            bullet.transform.SetParent(null, true);
            _bulletIsTraveling = true;
        }

        /// <summary>
        /// Called every frame while recalling, moving bullet in increments.
        /// </summary>
        private void BulletTravel()
        {
            if (!_bulletIsTraveling) return;

            BulletBackcallDirection = -(bullet.transform.position - playerCamera.transform.position);
            if (BulletBackcallDirection.magnitude < bulletBackcallThresholdDistance)
            {
                _bulletIsTraveling = false;
                CatchBullet();
                return;
            }

            bullet.transform.position += BulletBackcallDirection.normalized * (bulletTravelSpeed * Time.deltaTime);
        
            float bulletRotation = bullet.transform.localRotation.eulerAngles.z;
            bulletRotation += _bulletRotationSpeed * Time.deltaTime;
            bullet.transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, bulletRotation);
        }

        /// <summary>
        /// On bullet with contact -> player gets ammunition, bullet goes inactive
        /// </summary>
        private void CatchBullet()
        {
            Debug.Log("player caught bullet!");
            ammunition++;
            bullet.transform.SetParent(transform, true);
            bullet.transform.GetComponent<MeshRenderer>().enabled = false;
            bullet.transform.GetComponent<BulletCollision>().IsActive = false;
        }

        /// <summary>
        ///     /// Unparent bullet and activate gravity.
        /// </summary>
        public void SetBulletFree()
        {
            bullet.transform.parent = null;
        }
    }
}

