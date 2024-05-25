using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Canvas canvas;

    public GameObject textMessageBox;
    public TextMeshProUGUI textMessage;
    public GameObject textNext;
    public GameObject touchBlocker;
    public RectTransform highlightRect;
    public Image highlight;

    private System.Action targetEvent;

    public GameObject storyRoot;
    public Image story;
    public Sprite[] stories;
    public AudioClip bgmTitle;
    public AudioClip bgmStory;

    private bool wait;
    private bool isWatching;
    private CameraManager cameraManager;


    private void Awake()
    {
#if UNITY_ANDROID_API
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        SaveManager.GameLoad();
    }
    private void Start()
    {
        GameManager.PlayMusic(bgmTitle);
    }

    private void Update()
    {
        if (GameManager.Instance.touchManager.Tap)
        {
            if (!isWatching)
                CheckSkip();
            else
                Next();
        }
    }

    private bool isConditionSatisfied;
    private void ConditionSatisfied()
    {
        isConditionSatisfied = true;
    }
    private IEnumerator CoWaitCondition()
    {
        isConditionSatisfied = false;
        while (!isConditionSatisfied)
            yield return null;
    }


    public void CheckSkip()
    {
        isWatching = true;

        if (GameManager.Instance.DoneTutorial)
        {
            GameManager.Instance.SelectedStageID++;
            GameManager.Instance.LoadingScene(Scenes.main);
            return;
        }

        StartCoroutine(Co_ViewStory());
    }

    private void ViewMessage(string message)
    {
        textMessageBox.SetActive(true);
        textMessage.text = message;
    }

    private void CloseMessage()
    {
        textMessageBox.SetActive(false);
    }

    private IEnumerator CoWaitClick()
    {
        wait = true;
        touchBlocker.gameObject.SetActive(true);
        textNext.gameObject.SetActive(true);
        while (wait)
            yield return null;
    }

    private void Next()
    {
        wait = false;
        touchBlocker.gameObject.SetActive(false);
    }

    private void SetHighlightColor(Color color)
    {
        highlight.color = color;
    }
    private void HighlightOn(string name)
    {
        HighlightOn(GameObject.Find(name).GetComponent<RectTransform>());
    }
    private void HighlightOn(RectTransform rect)
    {
        highlightRect.gameObject.SetActive(true);
        highlightRect.pivot = rect.pivot;
        highlightRect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
        highlightRect.position = rect.position;
    }
    private void HighlightOff()
    {
        highlightRect.gameObject.SetActive(false);
    }

    private void LoadTutorialStage()
    {
        //�ʱ� ĳ���� ����
        int[] defaultCharacter =
            {
            1101,
            1102,
            1201
        };

        for (int i = 0; i < GameManager.Instance.Expedition.Length; i++)
        {
            if (i < defaultCharacter.Length)
            {
                if (!GameManager.Instance.UnlockedID.Contains(defaultCharacter[i]))
                    GameManager.Instance.UnlockedID.Add(defaultCharacter[i]);
                if (!GameManager.Instance.PurchasedID.Contains(defaultCharacter[i]))
                    GameManager.Instance.PurchasedID.Add(defaultCharacter[i]);
                GameManager.Instance.SetExpedition(defaultCharacter[i], i);
            }
            else
            {
                GameManager.Instance.SetExpedition(null, i);
            }
        }

        //Ʃ�丮�� �������� �ε�
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.SelectedStageID = 101;
        GameManager.Instance.LoadingScene(Scenes.stage);
        SceneManager.sceneLoaded += StartTutorial;
    }

    private void StartTutorial(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.stage)
            return;

        SceneManager.sceneLoaded -= StartTutorial;
        StartCoroutine(Co_ViewTutorial());
    }

    private IEnumerator Co_ViewStory()
    {
        GameManager.PlayMusic(bgmStory);
        storyRoot.SetActive(true);
        foreach (var s in stories)
        {
            story.sprite = s;
            yield return StartCoroutine(CoWaitClick());
        }
        LoadTutorialStage();
    }
    private IEnumerator Co_ViewTutorial()
    {
        //�� ��ȯ ����
        StageManager.Instance.enemyTower.SetStopSpawn(true);
        //�� ����
        StageManager.Instance.playerTower.SetInvincibility(true);
        StageManager.Instance.enemyTower.SetInvincibility(true);
        //�������� ��ư ��Ȱ��ȭ
        StageManager.Instance.uiOnStage.pause.gameObject.SetActive(false);
        //�ð� ����
        Time.timeScale = 0f;
        //ī�޶� ������ ����
        cameraManager = Camera.main.GetComponent<CameraManager>();
        SetCanMoveCamera(false);
        //��ȭ��ư ��Ȱ��ȭ
        StageManager.Instance.uiOnStage.toggleUpgardeDamage.interactable = false;
        StageManager.Instance.uiOnStage.toggleUpgardeHP.interactable = false;

        //Ʃ�丮�� ����
        ViewMessage("���� ���̽��ϱ�, �� ���ش� �ȵ����� ������ �ϴ� ����� ������� �� �ϴ� ���� �ٽ� �ѹ� �˷��帮�ڽ��ϴ�.\n�� �� ������ �ð� �ۿ� ���� �� �ϴ� Ȯ���� ���ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�켱 ������� �ٷ�� �� ���� �˷� �帮�ڽ��ϴ�.\n�ϴ��� ��ư�� ������ ���� ������� �������� ��ȯ �� �� �ֽ��ϴ�.");
        HighlightOn("Summon");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("������� ��ȯ �� ������ \"���\" ��� ��ȭ�� ���Դϴ�.\n���ϲ��� ������ ��差�� ���� ��ܿ��� Ȯ���� �����Ͻʴϴ�.");
        HighlightOn("CostRoot");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("���縦 ���� ��ȯ�غ��ðڽ��ϱ�?");
        textNext.gameObject.SetActive(false);
        SetHighlightColor(Color.yellow);
        HighlightOn("Summon");
        while (StageManager.Instance.playerTower.units.Count == 0)
            yield return null;
        HighlightOff();
        SetHighlightColor(Color.white);

        ViewMessage("�� �ϼ̽��ϴ�. ��ư�� ������ ���簡 �����ô� �� Ȯ�� �ϼ��� �� �Դϴ�.\n���縦 ��ȯ �ϸ�, ���� ���縦 ��ȯ �ϱ� ���� �ð��� �ʿ��ϴ� ���ϰ� �����Ͻ� ���� �ð��� �� �й� �ϼż� ��ȯ �Ͻñ� �ٶ�ڽ��ϴ�.");
        cameraManager.SetCameraPosition(StageManager.Instance.playerTower.units[0].transform.position);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();

        Time.timeScale = 1f;
        StageManager.Instance.enemyTower.SetStopSpawn(false);
        while (StageManager.Instance.enemyTower.units.Count == 0)
            yield return null;

        ViewMessage("���ݰ� ���� �Ʊ� ���縦 ��ȯ�ϰ� ���� �ð��� ������ �� ���������� ���� ��ȯ �˴ϴ�.");
        Time.timeScale = 0f;
        StageManager.Instance.enemyTower.SetStopSpawn(true);
        cameraManager.SetCameraPosition(StageManager.Instance.enemyTower.units[0].transform.position);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;
        SetCanMoveCamera(true);

        StageManager.Instance.enemyTower.units[0].OnDamaged += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        StageManager.Instance.enemyTower.units[0].OnDamaged -= ConditionSatisfied;
        ViewMessage("������� �ο�� ���� ���ø� ���� ������� �ʰ� 1:1 ������ �Ͼ�� �˴ϴ�.");
        cameraManager.SetCameraPosition(StageManager.Instance.enemyTower.units[0].transform.position);
        Time.timeScale = 0f;
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        StageManager.Instance.enemyTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        Time.timeScale = 0f;
        ViewMessage("�Ʊ��� ���� �����߷Ƚ��ϴ�. ���� �����߸��� ����ǰ���� \"���\"�� \"EXP\"�� ���� �� �ֽ��ϴ�.\nEXP�� ��� ����ϴ��� �ñ��Ͻð�����? ���� �帮�ڽ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�ϴ� ������ \"���ݷ� ��ȭ\" ��ư�� ���̽ô�����? �� ��ư���� �츮 ������� �� �����ϰ� ���׷��̵� �� �� �ֽ��ϴ�.");
        HighlightOn("Upgrade");
        yield return StartCoroutine(CoWaitClick());
        
        ViewMessage("������ ������ ����� ���Ͻô� ���縦 �����ϰ�, ü�� Ȥ�� ����ġ�� ���׷��̵� �� �� �ֽ��ϴ�.\n��ħ ���� �׿��� EXP�� �������, �ѹ� ���縦 ��ȭ�� �� ���ô� �� ��õ �帳�ϴ�.");
        SetHighlightColor(Color.yellow);
        textNext.gameObject.SetActive(false);
        StageManager.Instance.uiOnStage.toggleUpgardeDamage.interactable = true;
        StageManager.Instance.uiOnStage.toggleUpgardeHP.interactable = true;
        while (!StageManager.Instance.uiOnStage.toggleUpgardeDamage.isOn
            && !StageManager.Instance.uiOnStage.toggleUpgardeHP.isOn)
            yield return null;
        StageManager.Instance.uiOnStage.toggleUpgardeDamage.interactable = false;
        StageManager.Instance.uiOnStage.toggleUpgardeHP.interactable = false;

        HighlightOn("Summon");
        foreach (var b in StageManager.Instance.uiOnStage.buttonSummons)
        {
            b.button.onClick.AddListener(ConditionSatisfied);
        }
        yield return StartCoroutine(CoWaitCondition());
        foreach (var b in StageManager.Instance.uiOnStage.buttonSummons)
        {
            b.button.onClick.RemoveListener(ConditionSatisfied);
        }

        ViewMessage("�� �ϼ̽��ϴ�! \"��ȭ\"��ư�� ������ ������ �ɷ�ġ �� ��ȭ�� �˴ϴ�.\n��, ��ȭ�� �� �� EXP�� �Ҹ�Ǵ� ������ ���� �Ͻð� �����ϰ� ���縦 ��ȭ�Ͻñ� �ٶ��ϴ�.");
        SetHighlightColor(Color.white);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        HighlightOff();
        StageManager.Instance.uiOnStage.toggleUpgardeDamage.interactable = true;
        StageManager.Instance.uiOnStage.toggleUpgardeHP.interactable = true;
        Time.timeScale = 1f;

        StageManager.Instance.playerTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        Time.timeScale = 0f;
        cameraManager.SetCameraPosition(StageManager.Instance.enemyTower.transform.position);
        ViewMessage("�Ʊ� ������� �� ���� �����ϸ� ū �������� ������ ������� �˴ϴ�.\n������� ������ ��� ������.\n�̰��� �� ����鵵 ���������̴�, ���� �Ͻñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());


        //HighlightOn(GameObject.Find("TowerEnemy").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        ViewMessage("����, ������ ��ǥ�� �� ���� �����߸��� ���Դϴ�.\n�� ���� �μ��� �ʴ´ٸ� ������ ����ؼ� ��ȯ�˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�� ���� ���ø� ���� ��ܿ� ������ ������ �ٰ� ���̽� ���Դϴ�.\n�װ��� �� �� ������ ü���� ǥ���ϴ� ���Դϴ�.");
        yield return StartCoroutine(CoWaitClick());

        //HighlightOn(GameObject.Find("TowerPlayer").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        cameraManager.SetCameraPosition(StageManager.Instance.playerTower.transform.position);
        ViewMessage("���� �� ü���� �� ������ �̹� ������ ������ �¸��̰�, �ݴ�� ������ ü���� ��� ���̸� �ձ��� ����ϰ� �˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        HighlightOff();
        ViewMessage("�� ������ ������� �Դϴ�. ������ �͸����δ� ������ �� ������ ���� ������ �غ��ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        StageManager.Instance.enemyTower.SetStopSpawn(false);
        Time.timeScale = 1f;

        while (StageManager.Instance.enemyTower.HP > StageManager.Instance.enemyTower.MaxHP / 2f)
            yield return null;
        if (StageManager.Instance.playerTower.units.Count == 0)
        { 
            StageManager.Instance.playerTower.SpawnUnit(GameManager.Instance.Expedition[0]);
            StageManager.Instance.playerTower.units[0].Knockback();
        }
        cameraManager.SetCameraPosition(StageManager.Instance.playerTower.units[0].transform.position);
        ViewMessage(" ���� �Ʊ� ���簡 ���� �з� �� �� ���̽ʴϱ�?\n�� ���� ü���� 50% �̸��� �Ǹ� �� ���� ��������� �� �� ������ ����Ͽ� �Ʊ� ���簡 ���� �ָ� �з����ϴ�. �����Ͻñ� �ٶ��ϴ�.");
        StageManager.Instance.enemyTower.SetStopSpawn(true);
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("����. ���������� ���� �����ϴ�. �������ʹ� ���ձ��� ħ���ϰ� �ִ� �ձ��� ���ϱ� ���ؼ� ������ ���� �����Դϴ�.\n����� ���� ��Ȳ������..���� �Ѹ��ߴ� ������ �ɷ��� �Ͻ��ϴ�.\n�� �ձ��� �����ϰ� �� ������ �鼺���� ���ϴ� ������ �ǽÿɼҼ�.");
        yield return StartCoroutine(CoWaitClick());
        GameManager.Instance.DoneTutorial = true;
        StageManager.Instance.Victory();
        GameManager.Instance.SelectedStageID++;
        Destroy(gameObject);

    }

    public void SetCanMoveCamera(bool value)
    {
        GameManager.Instance.touchManager.receiver.gameObject.SetActive(value);
    }
}