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

    public GameObject storyRoot;
    public Image story;
    public Sprite[] stories;
    public AudioClip bgmTitle;
    public AudioClip bgmStory;

    public GameObject tutorialUI;
    public RayReceiver tutorialRayReceiver;
    public GameObject popupSkipStory;
    public GameObject popupSkipTuto;

    private bool wait;
    private bool isSkipChecked;
    private bool isPlayingTutorial;
    private CameraManager cameraManager;

    private Coroutine coStory;
    private Coroutine coTutorial;

    private float prevTimeScale;

    private void Start()
    {
        GameManager.PlayMusic(bgmTitle);
        if (GameManager.Instance.IsSettingPlayTutorial)
        {
            isSkipChecked = true;
            LoadTutorialStage();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.touchManager.Tap)
        {
            if (!isSkipChecked)
            {
                CheckSkip();
            }
            else if (!isPlayingTutorial)
            {
                if (GameManager.Instance.touchManager.receiver.ReceivedLastFrame)
                    Next();
            }
            else if (tutorialRayReceiver.ReceivedLastFrame)
            {
                Next();
            }
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
        isSkipChecked = true;

        if (PlayerPrefs.GetInt(Defines.playerfrabsStorySkip, 0) == 0)
        {
            coStory = StartCoroutine(Co_ViewStory());
            return;
        }

        if (GameManager.Instance.IsDoneTutorial)
        {
            SceneLoadManager.Instance.ChangeScene(Scenes.main);
        }
        else
        {
            LoadTutorialStage();
        }

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
        tutorialRayReceiver.gameObject.SetActive(true);
        textNext.gameObject.SetActive(true);
        while (wait)
            yield return null;
    }

    private void Next()
    {
        wait = false;
        touchBlocker.gameObject.SetActive(false);
        tutorialRayReceiver.gameObject.SetActive(false);
    }

    private void SetHighlightColor(Color color)
    {
        highlight.color = color;
    }
    private void HighlightOnScreen(string name)
    {
        var rect = GameObject.Find(name).GetComponent<RectTransform>();

        var corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        HightlightOn(rect.pivot,
            rect.position,
            (corners[2] - corners[0]) / canvas.scaleFactor);
    }
    private void HighlightOnWorld(string name)
    {
        var rect = GameObject.Find(name).GetComponent<RectTransform>();

        var corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        HightlightOn(rect.pivot,
            Camera.main.WorldToScreenPoint(rect.position),
            (Camera.main.WorldToScreenPoint(corners[2]) - Camera.main.WorldToScreenPoint(corners[0])) / canvas.scaleFactor);
    }
    private void HightlightOn(Vector2 pivot, Vector3 position, Vector2 sizeDelta)
    {
        highlightRect.gameObject.SetActive(true);
        highlightRect.pivot = pivot;
        highlightRect.position = position;
        highlightRect.sizeDelta = sizeDelta;
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
        SceneLoadManager.Instance.ChangeScene(Scenes.stage);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += StartTutorial;
    }

    private void StartTutorial(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.stage)
            return;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= StartTutorial;
        coTutorial = StartCoroutine(Co_ViewTutorial());
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

        if (!GameManager.Instance.IsDoneTutorial)
            PlayerPrefs.SetInt(Defines.playerfrabsStorySkip, 1);

        if (GameManager.Instance.IsDoneTutorial)
        {
            GameManager.Instance.SelectedStageID++;
            SceneLoadManager.Instance.ChangeScene(Scenes.main);
        }
        else
        {
            LoadTutorialStage();
        }
    }

    public void SkipStory()
    {
        StopCoroutine(coStory);

        if (GameManager.Instance.IsDoneTutorial)
        {
            SceneLoadManager.Instance.ChangeScene(Scenes.main);
        }
        else
        {
            LoadTutorialStage();
        }
    }
    public void SkipTutorial()
    {
        StopCoroutine(coTutorial);
        DoneTutorial();
    }
    public void LoadPrevInfo()
    {
        GameManager.Instance.SelectedStageID = GameManager.Instance.PrevSelectedStageID;
        for (int i = 0; i < GameManager.Instance.PrevExpedition.Count; i++)
        {
            GameManager.Instance.SetExpedition(GameManager.Instance.PrevExpedition[i], i);
        }
    }

    private IEnumerator Co_ViewTutorial()
    {
        isPlayingTutorial = true;
        tutorialUI.SetActive(true);

        var gameManager = GameManager.Instance;
        var stageManager = StageManager.Instance;
        var uiOnStage = stageManager.uiOnStage;

        //�� ��ȯ ����
        stageManager.enemyTower.SetStopSpawn(true);
        //�� ����
        stageManager.playerTower.SetInvincibility(true);
        stageManager.enemyTower.SetInvincibility(true);
        //���� ��ư ��Ȱ��ȭ
        uiOnStage.pause.gameObject.SetActive(false);
        uiOnStage.toggleGameSpeedFast.gameObject.SetActive(false);
        uiOnStage.toggleGameSpeedNormal.gameObject.SetActive(false);
        uiOnStage.toggleUnitInfo.gameObject.SetActive(false);
        //�ð� ����
        Time.timeScale = 0f;
        //ī�޶� ������ ����
        cameraManager = Camera.main.GetComponent<CameraManager>();
        SetCanMoveCamera(false);
        //��ȭ��ư ��Ȱ��ȭ
        stageManager.IsTutorial = true;
        uiOnStage.toggleUpgardeDamage.interactable = false;
        uiOnStage.toggleUpgardeHP.interactable = false;

        //Ʃ�丮�� ����
        ViewMessage("���� ���̽��ϱ�, �� ���ش� �ȵ����� ������ �ϴ� ����� ������� �� �ϴ� ���� �ٽ� �ѹ� �˷��帮�ڽ��ϴ�.\n�� �� ������ �ð� �ۿ� ���� �� �ϴ� Ȯ���� ���ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�켱 ������� �ٷ�� �� ���� �˷� �帮�ڽ��ϴ�.\n�ϴ��� ��ư�� ������ ���� ������� �������� ��ȯ �� �� �ֽ��ϴ�.");
        HighlightOnScreen("Summon");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("������� ��ȯ �� ������ \"���\" ��� ��ȭ�� ���Դϴ�.\n���ϲ��� ������ ��差�� ���� ��ܿ��� Ȯ���� �����Ͻʴϴ�.");
        HighlightOnScreen("Cost Root");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("���縦 ���� ��ȯ�غ��ðڽ��ϱ�?");
        textNext.gameObject.SetActive(false);
        SetHighlightColor(Color.yellow);
        HighlightOnScreen("Summon");
        while (stageManager.playerTower.units.Count == 0)
            yield return null;
        HighlightOff();
        SetHighlightColor(Color.white);

        ViewMessage("�� �ϼ̽��ϴ�. ��ư�� ������ ���簡 �����ô� �� Ȯ�� �ϼ��� �� �Դϴ�.\n���縦 ��ȯ �ϸ�, ���� ���縦 ��ȯ �ϱ� ���� �ð��� �ʿ��ϴ� ���ϰ� �����Ͻ� ���� �ð��� �� �й� �ϼż� ��ȯ �Ͻñ� �ٶ�ڽ��ϴ�.");
        cameraManager.SetCameraPosition(stageManager.playerTower.units[0].transform.position);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();

        Time.timeScale = 1f;
        stageManager.enemyTower.SetStopSpawn(false);
        while (stageManager.enemyTower.units.Count == 0)
            yield return null;

        ViewMessage("���ݰ� ���� �Ʊ� ���縦 ��ȯ�ϰ� ���� �ð��� ������ �� ���������� ���� ��ȯ �˴ϴ�.");
        Time.timeScale = 0f;
        stageManager.enemyTower.SetStopSpawn(true);
        cameraManager.SetCameraPosition(stageManager.enemyTower.units[0].transform.position);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;
        SetCanMoveCamera(true);

        stageManager.enemyTower.units[0].OnDamaged += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        stageManager.enemyTower.units[0].OnDamaged -= ConditionSatisfied;
        ViewMessage("������� �ο�� ���� ���ø� ���� ������� �ʰ� 1:1 ������ �Ͼ�� �˴ϴ�.");
        cameraManager.SetCameraPosition(stageManager.enemyTower.units[0].transform.position);
        Time.timeScale = 0f;
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        stageManager.enemyTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        Time.timeScale = 0f;
        ViewMessage("�Ʊ��� ���� �����߷Ƚ��ϴ�. ���� �����߸��� ����ǰ���� \"���\"�� \"EXP\"�� ���� �� �ֽ��ϴ�.\nEXP�� ��� ����ϴ��� �ñ��Ͻð�����? ���� �帮�ڽ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�ϴ� ������ \"���ݷ� ��ȭ\" ��ư�� ���̽ô�����? �� ��ư���� �츮 ������� �� �����ϰ� ���׷��̵� �� �� �ֽ��ϴ�.");
        HighlightOnScreen("UpgradeRoot/Upgrade");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("������ ������ ����� ���Ͻô� ���縦 �����ϰ�, ���ݷ� Ȥ�� ü���� 5������ ���׷��̵� �� �� �ֽ��ϴ�.\n��ħ ���� �׿��� EXP�� �������, �ѹ� ���縦 ��ȭ�� �� ���ô� �� ��õ �帳�ϴ�.");
        SetHighlightColor(Color.yellow);
        textNext.gameObject.SetActive(false);
        uiOnStage.toggleUpgardeDamage.interactable = true;
        uiOnStage.toggleUpgardeHP.interactable = true;
        while (!uiOnStage.toggleUpgardeDamage.isOn
            && !uiOnStage.toggleUpgardeHP.isOn)
            yield return null;
        uiOnStage.toggleUpgardeDamage.interactable = false;
        uiOnStage.toggleUpgardeHP.interactable = false;

        HighlightOnScreen("Summon");
        foreach (var b in uiOnStage.buttonSummons)
        {
            b.button.onClick.AddListener(ConditionSatisfied);
        }
        yield return StartCoroutine(CoWaitCondition());
        foreach (var b in uiOnStage.buttonSummons)
        {
            b.button.onClick.RemoveListener(ConditionSatisfied);
        }

        ViewMessage("�� �ϼ̽��ϴ�! \"��ȭ\"��ư�� ������ ������ �ɷ�ġ �� ��ȭ�� �˴ϴ�.\n��, ��ȭ�� �� �� EXP�� �Ҹ�Ǵ� ������ ���� �Ͻð� �����ϰ� ���縦 ��ȭ�Ͻñ� �ٶ��ϴ�.");
        SetHighlightColor(Color.white);
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        HighlightOff();
        stageManager.IsTutorial = false;
        uiOnStage.toggleUpgardeDamage.interactable = true;
        uiOnStage.toggleUpgardeHP.interactable = true;
        Time.timeScale = 1f;

        stageManager.playerTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        Time.timeScale = 0f;
        cameraManager.SetCameraPosition(stageManager.enemyTower.transform.position);
        ViewMessage("�Ʊ� ������� �� ���� �����ϸ� ū �������� ������ ������� �˴ϴ�.\n������� ������ ��� ������.\n�̰��� �� ����鵵 ���������̴�, ���� �Ͻñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());


        ViewMessage("����, ������ ��ǥ�� �� ���� �����߸��� ���Դϴ�.\n�� ���� �μ��� �ʴ´ٸ� ������ ����ؼ� ��ȯ�˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�� ���� ���ø� ���� ��ܿ� ������ ������ �ٰ� ���̽� ���Դϴ�.\n�װ��� �� �� ������ ü���� ǥ���ϴ� ���Դϴ�.\n���� �� ü���� �� ������ �̹� ������ ������ �¸��̰�");
        HighlightOnWorld("TowerEnemy/Health Bar/Slider");
        yield return StartCoroutine(CoWaitClick());

        HighlightOnWorld("TowerPlayer/Health Bar/Slider");
        cameraManager.SetCameraPosition(stageManager.playerTower.transform.position);
        ViewMessage("�ݴ�� ������ ü���� ��� ���̸� �ձ��� ����ϰ� �˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        HighlightOff();
        ViewMessage("�� ������ ������� �Դϴ�. ������ �͸����δ� ������ �� ������ ���� ������ �غ��ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        stageManager.enemyTower.SetStopSpawn(false);
        Time.timeScale = 1f;

        while (stageManager.enemyTower.HP > stageManager.enemyTower.MaxHP / 2f)
            yield return null;
        if (stageManager.playerTower.units.Count == 0)
        {
            stageManager.playerTower.SpawnUnit(gameManager.Expedition[0]);
            stageManager.playerTower.units[0].Knockback();
        }
        touchBlocker.SetActive(true);
        cameraManager.SetCameraPosition(stageManager.playerTower.units[0].transform.position);

        while (stageManager.playerTower.units[0].UnitState == CharacterAI.UNIT_STATE.KNOCKBACK)
            yield return null;
        Time.timeScale = 0f;
        ViewMessage("���� �Ʊ� ���簡 ���� �з� �� �� ���̽ʴϱ�?\n�� ���� ü�� ���� ���� �̸��� �Ǹ� �� ���� ��������� �� �� ������ ����Ͽ� �Ʊ� ���簡 ���� �ָ� �з����ϴ�. �����Ͻñ� �ٶ��ϴ�.");
        stageManager.enemyTower.SetStopSpawn(true);
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("����. ���������� ���� �����ϴ�. �������ʹ� ���ձ��� ħ���ϰ� �ִ� �ձ��� ���ϱ� ���ؼ� ������ ���� �����Դϴ�.\n����� ���� ��Ȳ������..���� �Ѹ��ߴ� ������ �ɷ��� �Ͻ��ϴ�.\n�� �ձ��� �����ϰ� �� ������ �鼺���� ���ϴ� ������ �ǽÿɼҼ�.");
        yield return StartCoroutine(CoWaitClick());

        DoneTutorial();
    }

    private void DoneTutorial()
    {
        var gameManager = GameManager.Instance;
        if (!gameManager.IsDoneTutorial)
            PlayerPrefs.SetInt(Defines.playerfrabsStorySkip, 1);

        gameManager.IsDoneTutorial = true;
        StageManager.Instance.IsTutorial = true;
        StageManager.Instance.Victory();
        Time.timeScale = 1f;
        if (gameManager.IsSettingPlayTutorial)
        {
            gameManager.IsSettingPlayTutorial = false;
            LoadPrevInfo();
        }
        else
        {
            gameManager.SelectedStageID++;
        }
        SceneLoadManager.Instance.ChangeScene(Scenes.main);
        GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_tutorial);
        Destroy(gameObject);
    }

    public void SetCanMoveCamera(bool value)
    {
        GameManager.Instance.touchManager.receiver.gameObject.SetActive(value);
        StageManager.Instance.uiOnStage.mapSlider.gameObject.SetActive(value);
    }

    public void PopupSkipStoryOnOff(bool value)
    {
        popupSkipStory.SetActive(value);
    }
    public void PopupSkipTutoOnOff(bool value)
    {
        if (value)
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            popupSkipTuto.SetActive(true);
        }
        else
        {
            popupSkipTuto.SetActive(false);
            Time.timeScale = prevTimeScale;
        }
    }
}