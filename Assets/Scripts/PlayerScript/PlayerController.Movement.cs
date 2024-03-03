using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace PlayerScript
{
    public partial class PlayerController
    {
        private Vector3 _dashDirection;
        private bool _isDashing;
        private bool _dashIsActive;
        private float _dashForce = 10;
        private readonly float _dashDuration = 0.25f;
        private float _dashTicker = 1f;
        private float _dashCooldown = 1f;
        private readonly float _dizzyTime = 0.5f;
        
        [FormerlySerializedAs("_camDashTiltDirection")]
        [Header("Camera tilt")]
        [SerializeField] Vector3 _camDashTiltTarget;
        [SerializeField, Range(0f, 1f)] private float _tiltAcceleration = 1f;
        [FormerlySerializedAs("_tiltResetAcceleration")] [SerializeField, Range(0f, 1f)]private  float _tiltResetAccelerationAfterDash= 0.01f;
        [SerializeField] float _dashTiltAmount = 10f;
        [SerializeField] private float dashSteps;
        private bool _dashRecovery;

        /// <summary>
        /// 1. Player calls dash, correct vector is calculated.
        /// 2. Dash is executed in Update().
        /// </summary>
        private void CallDash()
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
            if (_dashTicker < _dashCooldown) return;
            
            if (Input.GetAxis("Vertical") > 0)   // TODO this should really take direct input instead of a velocity representation since this can lead to unresponsive dash directions
            {
                _dashDirection += transform.forward;
                _camDashTiltTarget += new Vector3(_dashTiltAmount, 0, 0);
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                _dashDirection += -transform.forward;
                _camDashTiltTarget += new Vector3(-_dashTiltAmount, 0, 0);
            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                _dashDirection += transform.right;
                _camDashTiltTarget += new Vector3(0, 0, -_dashTiltAmount);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                _dashDirection += -transform.right;
                _camDashTiltTarget += new Vector3(0, 0, _dashTiltAmount);
            }

            if (_dashDirection == Vector3.zero)
            {
                _dashDirection = transform.forward;
                _camDashTiltTarget = new Vector3(_dashTiltAmount, 0, 0);
            }
            else
                _dashDirection = _dashDirection.normalized;

            _dashTicker = 0;
            _isDashing = true;
            _playerAcceleration = 0.5f * defaultPlayerAcceleration;
            Debug.Log("Dash is starting, z value at: " + playerCamera.transform.localRotation.eulerAngles.z);
        }

        /// <summary>
        /// Execute player displacement while dash is active.
        /// While dash is active, WASD movement is disabled.
        /// </summary>
        private void Dash()
        {
            if (!_isDashing) return;
            if (!_dashIsActive)
            {
                Invoke(nameof(StopDash), _dashDuration);
                _dashIsActive = true;
            }

            controller.Move(_dashDirection * (_dashForce * Time.deltaTime));
        }

        private void StopDash()
        {
            Debug.Log("Dash is stopping, z value at: " + playerCamera.transform.localRotation.eulerAngles.z);
            _isDashing = false;
            _dashIsActive = false;
            _dashDirection = Vector3.zero;
            _camDashTiltTarget = Vector3.zero;
            _dashRecovery = true;
            Invoke(nameof(RestorePlayerSpeed), _dizzyTime);
        }

        /// <summary>
        /// Set current player tilt corresponding to movement
        /// Note that this tilts the camera's parent to avoid race conditions with camera script
        /// Dash tilt completely overrides normal movement tilt
        /// </summary>
        private void PlayerTilt()
        {
            float xLerp;
            float zLerp;

            if (!_isDashing)
            {
                _camDashTiltTarget = new Vector3(Input.GetAxis("Vertical"),
                                                0,
                                                -Input.GetAxis("Horizontal"));
            }
            
            if (_camDashTiltTarget != Vector3.zero && !_dashRecovery)
            {
                xLerp = Mathf.LerpAngle(transform.rotation.eulerAngles.x, _camDashTiltTarget.x, _tiltAcceleration);
                zLerp = Mathf.LerpAngle(transform.rotation.eulerAngles.z, _camDashTiltTarget.z, _tiltAcceleration);
            }
            else
            {
                xLerp = Mathf.LerpAngle(transform.rotation.eulerAngles.x, _camDashTiltTarget.x, _tiltResetAccelerationAfterDash);
                zLerp = Mathf.LerpAngle(transform.rotation.eulerAngles.z, _camDashTiltTarget.z, _tiltResetAccelerationAfterDash);
            }
           
            transform.localRotation = Quaternion.Euler(new Vector3(xLerp, 
                                                                    transform.rotation.eulerAngles.y, // do not change y
                                                                    zLerp));
        }

        private void RestorePlayerSpeed()
        {
            _playerAcceleration = defaultPlayerAcceleration;
            _dashRecovery = false;
            // playerCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        private void DashTick()
        {
            if (_dashIsActive) return;

            if (_dashTicker < _dashCooldown)
                _dashTicker += Time.deltaTime;
        }
    }
}

