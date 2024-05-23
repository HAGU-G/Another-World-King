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
    public Button wait;
    public RectTransform highlight;
    private System.Action targetEvent;

    private void Awake()
    {
        SaveManager.GameLoad();
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


    public void StartTutorial()
    {
        if (GameManager.Instance.DoneTutorial)
        {
            GameManager.Instance.LoadingScene(Scenes.main);
            return;
        }


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
        SceneManager.sceneLoaded += Tutorial_01;
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
        wait.gameObject.SetActive(true);
        while (wait.gameObject.activeSelf)
            yield return null;
    }

    private void Next()
    {
        wait.gameObject.SetActive(false);
    }

    private void HighlightOn(string name)
    {
        HighlightOn(GameObject.Find(name).GetComponent<RectTransform>());
    }
    private void HighlightOn(RectTransform rect)
    {
        highlight.gameObject.SetActive(true);
        highlight.pivot = rect.pivot;
        highlight.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
        highlight.position = rect.position;
    }
    private void HighlightOff()
    {
        highlight.gameObject.SetActive(false);
    }

    private void Tutorial_01(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.stage)
            return;

        SceneManager.sceneLoaded -= Tutorial_01;
        StartCoroutine(Co_01());
    }


    private IEnumerator Co_01()
    {
        //�� ��ȯ ����, �÷��̾� ����
        StageManager.Instance.enemyTower.SetStopSpawn(true);
        StageManager.Instance.playerTower.SetInvincibility(true);
        StageManager.Instance.enemyTower.SetInvincibility(true);
        Time.timeScale = 0f;

        //Ʃ�丮�� ����
        ViewMessage("���� ���̽��ϱ�, �� ���ش� �ȵ����� ������ �ϴ� ����� ������� �� �ϴ� ���� �ٽ� �ѹ� �˷��帮�ڽ��ϴ�. �� �� ������ �ð� �ۿ� ���� �� �ϴ� Ȯ���� ���ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("�켱 ������� �ٷ�� �� ���� �˷� �帮�ڽ��ϴ�. �ϴ��� ��ư�� ������ ���� ������� �������� ��ȯ �� �� �ֽ��ϴ�.");
        HighlightOn("Summon");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("������� ��ȯ �� ������ \"���\" ��� ��ȭ�� ���Դϴ�. ���ϲ��� ������ ��差�� ���� ��ܿ��� Ȯ���� �����Ͻʴϴ�. ���� �� ���ðڽ��ϱ�?");
        HighlightOn("CostRoot");
        yield return StartCoroutine(CoWaitClick());

        HighlightOn("Summon");
        while (StageManager.Instance.playerTower.units.Count == 0)
            yield return null;
        HighlightOff();

        ViewMessage("�� �ϼ̽��ϴ�. ��ư�� ������ ���簡 �����ô� �� Ȯ�� �ϼ��� �� �Դϴ� ���縦 ��ȯ �ϸ�, ���� ���縦 ��ȯ �ϱ� ���� �ð��� �ʿ��ϴ� ���ϰ� �����Ͻ� ���� �ð��� �� �й� �ϼż� ��ȯ �Ͻñ� �ٶ�ڽ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();

        Time.timeScale = 1f;
        StageManager.Instance.enemyTower.SetStopSpawn(false);
        while (StageManager.Instance.enemyTower.units.Count == 0)
            yield return null;

        Time.timeScale = 0f;
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.enemyTower.units[0].transform.position);
        ViewMessage("���ݰ� ���� �Ʊ� ���縦 ��ȯ�ϰ� ���� �ð��� ������ �� ���������� ���� ��ȯ �˴ϴ�. ������� �ο�� ���� ���ø� ���� ������� �ʰ� 1:1 ������ �Ͼ�� �˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        StageManager.Instance.enemyTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        targetEvent -= ConditionSatisfied;
        Time.timeScale = 0f;

        ViewMessage("�Ʊ��� ���� �����߷Ƚ��ϴ�. ���� �����߸��� ����ǰ���� \"���\"�� \"EXP\"�� ���� �� �ֽ��ϴ�. EXP�� ��� ����ϴ��� �ñ��Ͻð�����? ���� �帮�ڽ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 0f;

        ViewMessage("�ϴ� ������ �����ݷ� ��ȭ�� ��ư�� ���̽ô�����? �� ��ư���� �츮 ������� �� �����ϰ� ���׷��̵� �� �� �ֽ��ϴ�. ������ ������ ����� ���Ͻô� ���縦 �����ϰ�, ü�� Ȥ�� ����ġ�� ���׷��̵� �� �� �ֽ��ϴ�. ��ħ ���� �׿��� EXP�� �������, �ѹ� ���縦 ��ȭ�� �� ���ô� �� ��õ �帳�ϴ�.");
        HighlightOn("Upgrade");
        while (!StageManager.Instance.uiOnStage.toggleUpgardeDamage.isOn
            && !StageManager.Instance.uiOnStage.toggleUpgardeHP.isOn)
            yield return null;

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

        ViewMessage("�� �ϼ̽��ϴ�! ����ȭ����ư�� ������ ������ �ɷ�ġ �� ��ȭ�� �˴ϴ�. ��, ��ȭ�� �� �� EXP�� �Ҹ�Ǵ� ������ ���� �Ͻð� �����ϰ� ���縦 ��ȭ�Ͻñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        HighlightOff();

        ViewMessage("�Ʊ� ������� �� ���� �����ϸ� ū �������� ������ ������� �˴ϴ�. ������� ������ ��� ������. �̰��� �� ����鵵 ���������̴�, ���� �Ͻñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        //HighlightOn(GameObject.Find("TowerEnemy").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.enemyTower.transform.position);
        ViewMessage("����, ������ ��ǥ�� �� ���� �����߸��� ���Դϴ�. �� ���� �μ��� �ʴ´ٸ� ������ ����ؼ� ��ȯ�˴ϴ�. �� ���� ���ø� ���� ��ܿ� ������ ������ �ٰ� ���̽� ���Դϴ�. �װ��� �� �� ������ ü���� ǥ���ϴ� ���Դϴ�.");
        yield return StartCoroutine(CoWaitClick());


        //HighlightOn(GameObject.Find("TowerPlayer").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.playerTower.transform.position);
        ViewMessage("���� �� ü���� �� ������ �̹� ������ ������ �¸��̰�, �ݴ�� ������ ü���� ��� ���̸� �ձ��� ����ϰ� �˴ϴ�.");
        yield return StartCoroutine(CoWaitClick());

        HighlightOff();
        ViewMessage("�� ������ ������� �Դϴ�. ������ �͸����δ� ������ �� ������ ���� ������ �غ��ñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        while (StageManager.Instance.playerTower.HP > StageManager.Instance.playerTower.MaxHP / 2f
            && StageManager.Instance.enemyTower.HP > StageManager.Instance.enemyTower.MaxHP / 2f)
            yield return null;
        ViewMessage(" ���� �Ʊ� ���簡 ���� �з� �� �� ���̽ʴϱ�? �� ���� ü���� 50% �̸��� �Ǹ� �� ���� ��������� �� �� ������ ����Ͽ� �Ʊ� ���簡 ���� �ָ� �з����ϴ�. �����Ͻñ� �ٶ��ϴ�.");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 0f;

        ViewMessage("����. ���������� ���� �����ϴ�. �������ʹ� ���ձ��� ħ���ϰ� �ִ� �ձ��� ���ϱ� ���ؼ� ������ ���� �����Դϴ�.\r\n����� ���� ��Ȳ������..���� �Ѹ��ߴ� ������ �ɷ��� �Ͻ��ϴ�. �� �ձ��� �����ϰ� �� ������ �鼺���� ���ϴ� ������ �ǽÿɼҼ�.\r\n");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 1f;
        GameManager.Instance.DoneTutorial = true;
        StageManager.Instance.Victory();
        GameManager.Instance.SelectedStageID++;
        Destroy(gameObject);

    }
}