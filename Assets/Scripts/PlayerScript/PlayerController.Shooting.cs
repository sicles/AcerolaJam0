using System.Collections;
using System.Numerics;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Vector3 = UnityEngine.Vector3;

namespace PlayerScript
{
    public partial class PlayerController
    {
        private readonly float _shootRange = 100;
        [SerializeField] private int ammunition;
        [SerializeField] private int maxAmmunition = 1;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float bulletBackcallThresholdDistance = 0.1f;
        [SerializeField] private float bulletTravelSpeed = 3;
        private bool _bulletIsTraveling;
        private readonly float _bulletRotationSpeed = 2000;
    
        [SerializeField] private bool _gunIsRacked;
        private float _rackTargetCharge = 2f;
        [SerializeField] private float _rackCharge;
        private bool _rackIsReady = true;
        [SerializeField] private float spreadAmount = 1;
        private float _recallTicker;
        private readonly float _recallCooldown = 0.75f; // set at same length of shooting animation
        public Vector3 BulletBackcallDirection { get; private set; }
        [SerializeField] private LineRenderer gunshotLineRenderer;
        [SerializeField] private Transform gunTip;
        [SerializeField] private ParticleSystem _shotParticle;
        [SerializeField] GameObject _bulletTrail;

        public int Ammunition => ammunition;

        public bool GunIsRacked => _gunIsRacked;

        private void Shoot()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            if (_reloadIsPlaying) return;

            _recallTicker = 0;
            _rackCharge = 0;
            _rackIsReady = false;
            StopRackAnimation();

            if (!GunIsRacked)
                return;
            
            CallShootAnimation();
            
            if (Ammunition == 0)
            {
                RuntimeManager.PlayOneShot("event:/OnPlayerEvents/UnloadedShoot");
                _gunIsRacked = false;
                return;
            }
            
            Ray ray = new Ray(playerCamera.transform.position, CalculateShotDirection());

            ShotParticle();
            SetBulletReadyParticleState(false);
            StartCoroutine(CameraShake(0.12f, 15f));
            
            //FX
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/Shoot");
            LayerMask layerMask = LayerMask.GetMask("Blood",
                                                                        "Default",
                                                                        "Enemy",
                                                                        "Water");
            
            
            if (Physics.Raycast(ray, out RaycastHit hit, _shootRange))
            {
                CastGunshotLine(hit);

                bullet.GetComponent<Bullet>().LastEnemy = hit.transform.root.gameObject;
                bullet.GetComponent<MeshRenderer>().enabled = true;
                bullet.transform.GetChild(0).gameObject.SetActive(true);    // huh?
                
                bullet.transform.SetParent(null, true);
                bullet.transform.GetComponent<MeshRenderer>().enabled = true;
                _rackIsReady = false;
                _gunIsRacked = false;
                
                AttachBullet(hit);

                // this code isn't terrible if i don't look at it :)
                if (hit.collider.transform.TryGetComponent<EnemyHitboxPart>(out EnemyHitboxPart hitboxPart))
                {
                    hitboxPart.HasBeenHit(50, hit.point, hit.normal, playerCamera.transform.forward);
                }
                else if (hit.collider.gameObject.TryGetComponent<Destructible>(out Destructible destructible))
                    destructible.DestroyAnimation();
                else if (hit.collider.gameObject.TryGetComponent<BedroomDoor>(out BedroomDoor bedroomDoor))
                    bedroomDoor.DestroyAnimation();
                else if (hit.collider.gameObject.TryGetComponent<Lover>(out Lover lover))
                    lover.End();
                else if (hit.collider.gameObject.TryGetComponent<NotaryDoor>(out NotaryDoor notaryDoor))
                    notaryDoor.OpenDoor();
            }
        }

        private void ShotParticle()
        {
            StartCoroutine(ShotParticleRoutine());
        }

        private IEnumerator ShotParticleRoutine()
        {
            _shotParticle.gameObject.SetActive(true);
            _shotParticle.Play();
            yield return new WaitForSeconds(0.5f);
            _shotParticle.Stop();
        }

        void CastGunshotLine(RaycastHit hit)
        {
            LineRenderer newGunshotLineRenderer = Instantiate(gunshotLineRenderer, Vector3.zero, Quaternion.identity);
            newGunshotLineRenderer.transform.parent = gunTip.transform;
            newGunshotLineRenderer.transform.localPosition = Vector3.zero;
            newGunshotLineRenderer.transform.localRotation = Quaternion.identity;
            StartCoroutine(DeactivateGunTrail(newGunshotLineRenderer));
        }
        
        private IEnumerator DeactivateGunTrail(LineRenderer lineRenderer)
        {
            yield return new WaitForSeconds(0.03f);
            if (lineRenderer.gameObject != null)
                Destroy(lineRenderer.gameObject);
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

        private void Reload()
        {
            if (Ammunition >= maxAmmunition) return;
            if (!Input.GetKeyDown(KeyCode.R)) return;
            if (_reloadIsPlaying) return;

            BulletRecall();
        }

        private void TickRecallTicker()
        {
            if (_recallTicker <= _recallCooldown)
                _recallTicker += Time.deltaTime;
        }

        private void ResetRackOnReload()
        {
            if (_reloadIsPlaying && !GunIsRacked)
            {
                _rackCharge = 0;
            }
        }
        
        private void RackGun()
        {
            if (GunIsRacked)
            {
                StopRackAnimation();
                return;
            }
            if (!_rackIsReady) return;
            if (_reloadIsPlaying) return;
            if (_recallTicker < _recallCooldown) return;

            PLAYBACK_STATE rackGunSoundPlaybackState;
            
            if (Input.GetKey(KeyCode.Mouse1))
            {
                _rackGunSound.getPlaybackState(out rackGunSoundPlaybackState);

                if (rackGunSoundPlaybackState != PLAYBACK_STATE.PLAYING)
                {
                    _rackGunSound.setTimelinePosition(0);
                    _rackGunSound.start();
                }
                
                _rackCharge += Time.deltaTime;
                StartRackAnimation();
            }
            else
            {
                _rackGunSound.getPlaybackState(out rackGunSoundPlaybackState);
                if (rackGunSoundPlaybackState == PLAYBACK_STATE.PLAYING)
                {
                    _rackGunSound.stop(STOP_MODE.ALLOWFADEOUT);
                }

                _rackCharge = 0;
                StopRackAnimation();
            }

            if (_rackCharge >= _rackTargetCharge)
            {
                _gunIsRacked = true;
                RuntimeManager.PlayOneShot("event:/OnPlayerEvents/RackFinish");
            }
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
        public void BulletRecall()
        {
            bullet.transform.GetComponent<Bullet>().IsActive = true;
            bullet.transform.SetParent(null, true);
            _bulletTrail.gameObject.SetActive(true);
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/Recall");
            CallRecallAnimation();
            _bulletIsTraveling = true;
        }

        /// <summary>
        /// Called every frame while recalling, moving bullet in increments.
        /// </summary>
        private void BulletTravel()
        {
            if (!_bulletIsTraveling)
            {
                if (bullet.GetComponent<Bullet>().BulletFlight.IsPlaying())
                    bullet.GetComponent<Bullet>().SetFlightSound(false);
                return;
            }

            if (!bullet.GetComponent<Bullet>().BulletFlight.IsPlaying())
                bullet.GetComponent<Bullet>().SetFlightSound(true);

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
            bullet.GetComponent<MeshRenderer>().enabled = false;
            bullet.transform.GetChild(0).gameObject.SetActive(false);   // TODO what the heck is this
            StartCatchReloadAnimation();
            RuntimeManager.PlayOneShot("event:/OnPlayerEvents/CatchReload");
            bullet.transform.SetParent(transform, true);
            bullet.transform.GetComponent<MeshRenderer>().enabled = false;
            bullet.transform.GetComponent<Bullet>().IsActive = false;
            _bulletTrail.gameObject.SetActive(false);
        }

        /// <summary>
        ///     /// Unparent bullet and activate gravity.
        /// </summary>
        public void SetBulletFree()
        {
            bullet.transform.parent = null;
        }

        public void ActivateGun(bool isActive)
        {
            isArmed = isActive; 
            
            foreach (var obj in firstPersonMeshes)
            {
                obj.SetActive(isActive);
            }
        }
    }
}

