using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using PlayerScript;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private float _deepConfusion;
    private FMOD.Studio.Bus _master;
    [SerializeField] private float currentSensitivity = 0.5f;
    [SerializeField] private float currentVolume = 1;
    [FormerlySerializedAs("fullscreen")] [SerializeField] private int fullscreenValue = 1;
    [SerializeField] private Scrollbar sensUI;
    [SerializeField] private Scrollbar volUI;
    private string _sensitivity = "sensitivity";
    private string _volume = "volume";
    private string _fullscreen = "fullscreen";


    private void Start()
    {
        _master = RuntimeManager.GetBus("bus:/");
        LoadSettings(ref currentSensitivity, ref currentVolume, ref fullscreenValue);
        sensUI.value = currentSensitivity;
        volUI.value = currentVolume;
    }

    
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(_sensitivity, currentSensitivity);
        PlayerPrefs.SetFloat(_volume, currentVolume);
        PlayerPrefs.SetInt(_fullscreen, fullscreenValue);
    }
    
    public void LoadSettings(ref float sensitivity, ref float volume, ref int isFullscreen)
    {
        sensitivity = PlayerPrefs.GetFloat(_sensitivity);
        volume = PlayerPrefs.GetFloat(_volume);
        isFullscreen = PlayerPrefs.GetInt(_fullscreen);
    }

    public void StartGame()
    {
        SaveSettings();
        
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GetNewSensitivity()
    {
        if (sensUI.value == 0)
            sensUI.value = 0.01f;
        else
            currentSensitivity = sensUI.value * 1;
        
        SaveSettings();
        Debug.Log("new sens is " + currentSensitivity);
    }

    public void GetNewVolume()
    {
        currentVolume = volUI.value * 100;
        
        SaveSettings();
        Debug.Log("new vol is " + currentVolume);
    }

    public void GetIfFullscreen()
    {
        fullscreenValue = Screen.fullScreen ? 1 : 0;

        Screen.fullScreen = !Screen.fullScreen;
        
        SaveSettings();
        Debug.Log("new fullscreen value is " + fullscreenValue);
    }
}
