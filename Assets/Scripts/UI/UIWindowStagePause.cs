using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowStagePause : UIWindow
{
    public AudioSource audioSource;
    public TextMeshProUGUI flags;
    public TextMeshProUGUI title;
    public GameObject win;
    public Button back;
    public Button restart;
    public Button goMain;
    public Toggle[] stars;
    public readonly static string flagTextFormat = "+{0}";

    private float currentTimeScale;

    private void Awake()
    {
        back.onClick.AddListener(() => { Time.timeScale = currentTimeScale; Close(); });
        restart.onClick.AddListener(() => { Time.timeScale = currentTimeScale; GameManager.Instance.LoadingScene(Scenes.stage); });
        goMain.onClick.AddListener(() => { Time.timeScale = currentTimeScale; GameManager.Instance.LoadingScene(Scenes.main); });
    }

    public void Victory(int starCount, int flag)
    {
        currentTimeScale =Time.timeScale;
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
        audioSource.Play();
        flags.text = string.Format(flagTextFormat,flag);
        title.text = Defines.victory;
    }

    public void Defeat()
    {
        currentTimeScale = Time.timeScale;
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
        currentTimeScale = Time.timeScale;
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
