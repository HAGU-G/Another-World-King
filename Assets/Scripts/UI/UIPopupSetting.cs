using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIPopupSetting : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Toggle toggleMusic;
    public Toggle toggleSfx;
    public Toggle toggleStorySkip;

    private void Start()
    {
        if (PlayerPrefs.GetInt(AudioParameters.musicVolume, 1) == 0)
            toggleMusic.isOn = false;
        else
            toggleMusic.isOn = true;

        if (PlayerPrefs.GetInt(AudioParameters.sfxVolume, 1) == 0)
            toggleSfx.isOn = false;
        else
            toggleSfx.isOn = true;

        if (PlayerPrefs.GetInt(Defines.playerfrabsStorySkip, 0) == 0)
            toggleStorySkip.isOn = false;
        else
            toggleStorySkip.isOn = true;
    }

    public void MusicOnOff(bool value)
    {
        audioMixer.SetFloat(AudioParameters.musicVolume, value ? 0f : -80f);
        PlayerPrefs.SetInt(AudioParameters.musicVolume, value ? 1 : 0);
    }
    public void SfxAndUIOnOff(bool value)
    {
        audioMixer.SetFloat(AudioParameters.sfxVolume, value ? 0f : -80f);
        audioMixer.SetFloat(AudioParameters.uiVolume, value ? 0f : -80f);
        PlayerPrefs.SetInt(AudioParameters.sfxVolume, value ? 1 : 0);
    }
    public void StorySkipOnoff(bool value)
    {
        PlayerPrefs.SetInt(Defines.playerfrabsStorySkip, value ? 1 : 0);
    }
    public void PlayTutorial()
    {
        var gm = GameManager.Instance;
        gm.IsSettingPlayTutorial = true;
        gm.PrevExpedition.Clear();
        foreach (var character in gm.Expedition)
        {
            if (character != null)
                gm.PrevExpedition.Add(character.unitData.id);
            else
                gm.PrevExpedition.Add(0);
        }
        gm.PrevSelectedStageID = gm.SelectedStageID;
        SceneLoadManager.Instance.ChangeScene(Scenes.title);
    }
    public void PopupOnOff(bool value)
    {
        gameObject.SetActive(value);
    }
}