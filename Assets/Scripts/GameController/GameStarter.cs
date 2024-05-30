using UnityEngine;
using UnityEngine.Audio;

public class GameStarter : MonoBehaviour
{
    public AudioMixer audioMixer;

    private void Awake()
    {
#if UNITY_ANDROID_API
        //Application.targetFrameRate = 60;
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
