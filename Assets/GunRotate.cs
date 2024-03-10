using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunRotate : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 _startPosition;
    private float _playerDistanceFactor;
    [SerializeField] private float shakeFactor = 1;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        GetPlayerDistance();
        Vibrate();
    }

    private void GetPlayerDistance()
    {
        _playerDistanceFactor = 1 / (transform.position - player.position).magnitude * shakeFactor;
    }

    private void Vibrate()
    {
        transform.position = _startPosition + new Vector3(Random.Range(-0.005f, 0.005f),
            Random.Range(-0.005f, 0.005f),
            Random.Range(-0.005f, 0.005f))
            * _playerDistanceFactor;
    }
}
