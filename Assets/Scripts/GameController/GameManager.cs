using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region INSTANCE
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindWithTag(Tags.gameManager).GetComponent<GameManager>();

            return instance;
        }
    }
    #endregion

    public CharacterInfos[] Expedition { get; private set; } = new CharacterInfos[5];

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetExpedition(CharacterInfos characterInfos, int index = -1)
    {
        if (index < 0)
        {
            for (int i = 0; i < Expedition.Length; i++)
            {
                if (Expedition[i] != null)
                    continue;
                index = i;
                break;
            }
        }

        if (index < 0)
            return;

        Expedition[index] = characterInfos;
    }
    public CharacterInfos GetExpedition(int index)
    {
        return Expedition[index];
    }
}
