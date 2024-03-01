using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public partial class PlayerController
{
    private Vector3 _dashDirection;
    private bool _isDashing;
    private bool _dashIsActive;
    private float _dashForce = 10;
    private readonly float _dashDuration = 0.25f;
    private float _dashTicker = 2.5f;
    private float _dashCooldown = 2.5f;

    /// <summary>
    /// 1. Player calls dash, correct vector is calculated.
    /// 2. Dash is executed in Update().
    /// </summary>
    private void CallDash()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
        if (_dashTicker < _dashCooldown) return;

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

        _dashTicker = 0;
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

    private void DashTick()
    {
        if (_dashIsActive) return;

        if (_dashTicker < _dashCooldown)
            _dashTicker += Time.deltaTime;
    }
}


