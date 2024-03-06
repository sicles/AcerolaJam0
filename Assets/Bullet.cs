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
    [SerializeField] private int testField;
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

    private void OnTriggerExit(Collider other)
    {
        if (!IsActive) return;
        
        if (other.transform.GetComponent<AI.PrototypeAI>() != null && other.gameObject != LastEnemy)
        {
            other.transform.GetComponent<AI.PrototypeAI>().TakeDamage(50, other.transform.position, other.transform.rotation.eulerAngles, _playerController.BulletBackcallDirection);
        }

        LastEnemy = other.gameObject;
    }
}