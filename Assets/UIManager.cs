using System;
using System.Collections;
using PlayerScript;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform healthLossBar;
    [SerializeField] private float healthLossDelay = 2;
    private bool _healthlossbarUpdateIsRunning;

    public void UpdateHealthbar(int lastHealth)
    {
        healthBar.localScale = new Vector3((playerController.GetPlayerHealth() / 100f), healthBar.localScale.y, healthBar.localScale.z);
        if (_healthlossbarUpdateIsRunning)  // fast forward update if additional damage happens
            healthLossBar.localScale = new Vector3(playerController.GetPlayerHealth() / 100f, 
                                                    healthLossBar.localScale.y,
                                                    healthLossBar.localScale.z);
        
        StopAllCoroutines();
        StartCoroutine(UpdateHealthlossbar(lastHealth));
    }

    private IEnumerator UpdateHealthlossbar(int damageAmount)
    {
        _healthlossbarUpdateIsRunning = true;
        yield return new WaitForSeconds(healthLossDelay);

        float steps = 50;   // is float to avoid having to explicit cast, but must be int
        float healthStep = (damageAmount / steps) * 0.01f ;   // first float for time taken, second is reduction to number between 0 and 1 (to fit scale dimension)

        for (int i = 0; i < steps; i++)
        {
            healthLossBar.localScale = new Vector3(healthLossBar.localScale.x - healthStep, healthLossBar.localScale.y, healthLossBar.localScale.z);
            yield return new WaitForSeconds(1 / steps);
        }
        
        _healthlossbarUpdateIsRunning = false;
    }
}
