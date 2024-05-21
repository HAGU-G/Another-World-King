using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWindowStagePause : UIWindow
{
    public TextMeshProUGUI flags;
    public TextMeshProUGUI title;
    public GameObject win;
    public Button back;
    public Button restart;
    public Button goMain;
    public Toggle[] stars;

    private void Awake()
    {
        back.onClick.AddListener(() => { Time.timeScale = 1f; Close(); });
        restart.onClick.AddListener(() => { Time.timeScale = 1f; GameManager.Instance.LoadingScene(Scenes.devStage); });
        goMain.onClick.AddListener(() => { Time.timeScale = 1f; GameManager.Instance.LoadingScene(Scenes.devMain); });
    }

    public void Victory(int starCount, int flag)
    {
        Time.timeScale = 0f;
        Open();
        back.gameObject.SetActive(false);
        win.SetActive(true);
        restart.gameObject.SetActive(false);
        foreach (var star in stars)
        {
            star.gameObject.SetActive(true);
            star.isOn = starCount > 0;
            starCount--;
        }
        flags.text = flag.ToString();
        title.text = Defines.victory;
    }
    public void Defeat()
    {
        Time.timeScale = 0f;
        Open();
        back.gameObject.SetActive(false);
        win.SetActive(false);
        restart.gameObject.SetActive(true);
        foreach (var star in stars)
        {
            star.gameObject.SetActive(true);
            star.isOn = false;
        }
        title.text = Defines.defeat;
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        Open();
        win.SetActive(false);
        back.gameObject.SetActive(true);
        restart.gameObject.SetActive(true);
        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
        }
        title.text = Defines.pause;
    }
}
