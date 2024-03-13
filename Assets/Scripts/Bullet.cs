using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    private AI.PrototypeAI _prototypeAI;
    private PlayerScript.PlayerController _playerController;
    private PLAYBACK_STATE _bulletFlightPlaybackState;
    [SerializeField] private StudioEventEmitter bulletFlight;
    public bool IsActive { get; set; }

    public GameObject LastEnemy { get; set; }

    public StudioEventEmitter BulletFlight => bulletFlight;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerScript.PlayerController>();
        bulletFlight = GetComponent<StudioEventEmitter>();
    }

    public void SetFlightSound(bool shouldPlay)
    {
        if (shouldPlay)
            BulletFlight.Play();
        else
            BulletFlight.Stop();
    }

    // this is heavily bugged so i'm disabling it :(
    // private void OnTriggerExit(Collider other)
    // {
    //     if (!IsActive) return;
    //     
    //     // Debug.Log("bullet hit a " + other.transform.gameObject);
    //     
    //     if (other.transform.GetComponent<EnemyHitboxPart>() != null && other.transform.root.gameObject != LastEnemy)
    //     {
    //         Debug.Log("dealt collision damage!");
    //         other.transform.GetComponent<EnemyHitboxPart>().HasBeenHit(50, other.transform.position, other.transform.rotation.eulerAngles, _playerController.BulletBackcallDirection);
    //     }
    //
    //     LastEnemy = other.gameObject;
    // }
}
