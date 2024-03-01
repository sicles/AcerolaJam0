using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public partial class PlayerController
{
    private Vector3 _dashDirection;
    private bool _isDashing;
    private bool _dashIsActive;
    [SerializeField] private float _dashForce = 100;
    private readonly float _dashDuration = 0.25f;

    /// <summary>
    /// 1. Player calls dash, correct vector is calculated.
    /// 2. Dash is executed in Update().
    /// </summary>
    private void CallDash()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift)) return;

        if (Input.GetAxis("Vertical") > 0)
            _dashDirection += transform.forward;
        else if (Input.GetAxis("Vertical") < 0)
            _dashDirection += -transform.forward;

        if (Input.GetAxis("Horizontal") > 0)
            _dashDirection += transform.right;
        else if (Input.GetAxis("Horizontal") < 0)
            _dashDirection += -transform.right;

        if (_dashDirection == Vector3.zero)
            _dashDirection = transform.forward;
        else
            _dashDirection = _dashDirection.normalized;
        
        _isDashing = true;
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
        _isDashing = false;
        _dashIsActive = false;
        _dashDirection = Vector3.zero;
    }
}

