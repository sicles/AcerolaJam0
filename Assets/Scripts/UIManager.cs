using System.Collections;
using FMOD.Studio;
using PlayerScript;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform healthLossBar;
    [SerializeField] private float healthLossDelay = 2;
    private bool _healthlossbarUpdateIsRunning;
    private Coroutine _currentHealthlossBarUpdate;
    private Coroutine _currentSendMessage;
    private TextMeshProUGUI _messageBoard;

    private void Start()
    {
        _messageBoard = GetComponentInChildren<TextMeshProUGUI>();
        // CallSendMessage();
    }

    public void UpdateHealthbar(int lastHealth)
    {
        healthBar.localScale = new Vector3((playerController.GetPlayerHealth() / 100f), healthBar.localScale.y, healthBar.localScale.z);
        if (_healthlossbarUpdateIsRunning)  // fast forward update if additional damage happens
            healthLossBar.localScale = new Vector3(playerController.GetPlayerHealth() / 100f, 
                                                    healthLossBar.localScale.y,
                                                    healthLossBar.localScale.z);
        
        if (_currentHealthlossBarUpdate != null)
            StopCoroutine(_currentHealthlossBarUpdate);     // don't use without safety
        _currentHealthlossBarUpdate = StartCoroutine(UpdateHealthlossbar(lastHealth));
    }

    private IEnumerator UpdateHealthlossbar(int damageAmount)
    {
        _healthlossbarUpdateIsRunning = true;
        yield return new WaitForSeconds(healthLossDelay);

        float steps = 50;   // is float to avoid having to explicit cast, but value must be int
        float healthStep = (damageAmount / steps) * 0.01f ;   // first float for time taken, second is reduction to number between 0 and 1 (to fit scale dimension)

        for (int i = 0; i < steps; i++)
        {
            healthLossBar.localScale = new Vector3(healthLossBar.localScale.x - healthStep, healthLossBar.localScale.y, healthLossBar.localScale.z);
            yield return new WaitForSeconds(1 / steps);
        }
        
        _healthlossbarUpdateIsRunning = false;
    }

    public void CallSendMessage(string message, float lifetime)
    {
        if (_currentSendMessage != null)
            StopCoroutine(_currentSendMessage);     // don't use without safety
        _currentSendMessage = StartCoroutine(SendMessageRoutine(message, lifetime));
    }

    private IEnumerator SendMessageRoutine(string message, float lifetime)
    {
        _messageBoard.text = "";
        
        // set this more elegantly when method is done
        const int shuffleFrames = 5;
        const float shuffleTimePer = 0.02f;
        EventInstance shuffleSound = FMODUnity.RuntimeManager.CreateInstance("event:/scroll");
        
        for (int i = 0; i < message.Length; i++)
        {
            for (int j = 0; j < shuffleFrames; j++)
            {
                var asciiRange = Random.Range(64, 122);
                string randomChar = System.Convert.ToChar(asciiRange).ToString();
                
                _messageBoard.text = _messageBoard.text.Insert(i, randomChar);
                yield return new WaitForSeconds(shuffleTimePer);
                _messageBoard.text = _messageBoard.text.Remove(i, 1);
            }

            PLAYBACK_STATE playbackState;
            shuffleSound.getPlaybackState(out playbackState);
            if (playbackState == PLAYBACK_STATE.PLAYING)
                shuffleSound.stop(STOP_MODE.ALLOWFADEOUT);
                
            shuffleSound.start();
            
            _messageBoard.text += message[i];
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(lifetime);
        _messageBoard.text = "";
    }
}
