using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void Update()
    {
        if (Input.GetKeyDown("-"))
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }
    }

    public void SetVolume(float volume)
    {
       audioMixer.SetFloat("volume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

}
