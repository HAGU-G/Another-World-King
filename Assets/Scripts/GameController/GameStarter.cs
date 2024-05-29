using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class GameStarter : MonoBehaviour
{
    public AudioMixer audioMixer;

    private void Awake()
    {
#if UNITY_ANDROID_API
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        SaveManager.GameLoad();
    }


    private void Start()
    {
        if (PlayerPrefs.GetInt(AudioParameters.musicVolume, 1) == 0)
            audioMixer.SetFloat(AudioParameters.musicVolume, -80f);

        if (PlayerPrefs.GetInt(AudioParameters.sfxVolume, 1) == 0)
        {
            audioMixer.SetFloat(AudioParameters.sfxVolume,  -80f);
            audioMixer.SetFloat(AudioParameters.uiVolume, -80f);
        }
    }
}
