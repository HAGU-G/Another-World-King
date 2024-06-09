using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    #region INSTANCE
    private static SceneLoadManager instance;
    public static SceneLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<SceneLoadManager>(Paths.resourcesSceneLoadManager));
                instance.Init();
            }

            return instance;
        }
    }
    #endregion

    private AsyncOperation asyncOperation = null;
    public Slider slider;

    private void Update()
    {
        slider.value = asyncOperation.progress;
        Debug.Log(slider.value);
        Debug.Log(asyncOperation.progress);
    }

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public void ChangeScene(string name)
    {
        gameObject.SetActive(true);
        SaveManager.GameSave();
        Time.timeScale = 1f;

        asyncOperation = SceneManager.LoadSceneAsync(name);
        slider.value = asyncOperation.progress;
        asyncOperation.completed += LoadComplete;
    }

    private void LoadComplete(AsyncOperation ao)
    {
        asyncOperation = null;
        gameObject.SetActive(false);
    }
}
