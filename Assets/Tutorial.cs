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


        //초기 캐릭터 지급
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

        //튜토리얼 스테이지 로드
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
        //적 소환 정지, 플레이어 무적
        StageManager.Instance.enemyTower.SetStopSpawn(true);
        StageManager.Instance.playerTower.SetInvincibility(true);
        StageManager.Instance.enemyTower.SetInvincibility(true);
        Time.timeScale = 0f;

        //튜토리얼 시작
        ViewMessage("전하 오셨습니까, 잘 이해는 안되지만 전쟁을 하는 방법을 까먹으신 듯 하니 제가 다시 한번 알려드리겠습니다. 한 번 설명할 시간 밖에 없을 듯 하니 확실히 배우시기 바랍니다.");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("우선 병사들을 다루는 법 부터 알려 드리겠습니다. 하단의 버튼을 누르면 저희 병사들을 전장으로 소환 할 수 있습니다.");
        HighlightOn("Summon");
        yield return StartCoroutine(CoWaitClick());

        ViewMessage("병사들을 소환 할 때에는 \"골드\" 라는 재화가 쓰입니다. 전하께서 소유한 골드량은 좌측 상단에서 확인이 가능하십니다. 직접 해 보시겠습니까?");
        HighlightOn("CostRoot");
        yield return StartCoroutine(CoWaitClick());

        HighlightOn("Summon");
        while (StageManager.Instance.playerTower.units.Count == 0)
            yield return null;
        HighlightOff();

        ViewMessage("잘 하셨습니다. 버튼을 누르면 병사가 나오시는 걸 확인 하셨을 것 입니다 병사를 소환 하면, 다음 병사를 소환 하기 까지 시간이 필요하니 전하가 소유하신 골드와 시간을 잘 분배 하셔서 소환 하시길 바라겠습니다.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();

        Time.timeScale = 1f;
        StageManager.Instance.enemyTower.SetStopSpawn(false);
        while (StageManager.Instance.enemyTower.units.Count == 0)
            yield return null;

        Time.timeScale = 0f;
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.enemyTower.units[0].transform.position);
        ViewMessage("지금과 같이 아군 병사를 소환하고 일정 시간이 지나면 적 기지에서도 적이 소환 됩니다. 병사들이 싸우는 것을 보시면 서로 통과하지 않고 1:1 전투가 일어나게 됩니다.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        StageManager.Instance.enemyTower.units[0].OnDead += ConditionSatisfied;
        yield return StartCoroutine(CoWaitCondition());
        targetEvent -= ConditionSatisfied;
        Time.timeScale = 0f;

        ViewMessage("아군이 적을 쓰러뜨렸습니다. 적을 쓰러뜨리면 전리품으로 \"골드\"와 \"EXP\"를 얻을 수 있습니다. EXP를 어디에 사용하는지 궁금하시겠지요? 설명 드리겠습니다.");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 0f;

        ViewMessage("하단 우측의 “공격력 강화” 버튼이 보이시는지요? 이 버튼으로 우리 병사들을 더 강인하게 업그레이드 할 수 있습니다. 전하의 지혜를 사용해 원하시는 병사를 선택하고, 체력 혹은 경험치를 업그레이드 할 수 있습니다. 마침 적을 죽여서 EXP를 얻었으니, 한번 병사를 강화를 해 보시는 걸 추천 드립니다.");
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

        ViewMessage("잘 하셨습니다! “강화”버튼을 누르면 병사의 능력치 가 강화가 됩니다. 단, 강화를 할 때 EXP가 소모되니 이점을 참고 하시고 신중하게 병사를 강화하시기 바랍니다.");
        yield return StartCoroutine(CoWaitClick());
        HighlightOff();

        ViewMessage("아군 병사들은 적 성을 공격하면 큰 데미지를 입히고 사라지게 됩니다. 병사들의 숭고한 희생 이지요. 이것은 적 병사들도 마찬가지이니, 유의 하시길 바랍니다.");
        yield return StartCoroutine(CoWaitClick());

        //HighlightOn(GameObject.Find("TowerEnemy").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.enemyTower.transform.position);
        ViewMessage("전하, 저희의 목표는 적 성을 쓰러뜨리는 것입니다. 적 성을 부수지 않는다면 적군은 계속해서 소환됩니다. 잘 살펴 보시면 성의 상단에 빨간색 게이지 바가 보이실 것입니다. 그것이 각 성 기지의 체력을 표현하는 것입니다.");
        yield return StartCoroutine(CoWaitClick());


        //HighlightOn(GameObject.Find("TowerPlayer").transform.Find("HP/Square").GetComponent<RectTransform>(), true);
        Camera.main.GetComponent<CameraManager>().SetCameraPosition(StageManager.Instance.playerTower.transform.position);
        ViewMessage("적의 성 체력을 다 깎으면 이번 전투는 저희의 승리이고, 반대로 저희의 체력이 모두 깎이면 왕국은 멸망하게 됩니다.");
        yield return StartCoroutine(CoWaitClick());

        HighlightOff();
        ViewMessage("제 설명은 여기까지 입니다. 설명한 것만으로는 부족할 수 있으니 직접 전투를 해보시기 바랍니다.");
        yield return StartCoroutine(CoWaitClick());
        CloseMessage();
        Time.timeScale = 1f;

        while (StageManager.Instance.playerTower.HP > StageManager.Instance.playerTower.MaxHP / 2f
            && StageManager.Instance.enemyTower.HP > StageManager.Instance.enemyTower.MaxHP / 2f)
            yield return null;
        ViewMessage(" 지금 아군 병사가 전부 밀려 난 게 보이십니까? 적 성의 체력이 50% 미만이 되면 적 성의 마법사들이 한 번 마법을 사용하여 아군 병사가 전부 멀리 밀려납니다. 참고하시기 바랍니다.");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 0f;

        ViewMessage("전하. 모의전투가 끝이 났습니다. 이제부터는 마왕군이 침략하고 있는 왕국을 구하기 위해서 출정을 떠날 차례입니다.\r\n희망이 없는 상황이지만..저는 총명했던 전하의 능력을 믿습니다. 꼭 왕국을 구원하고 이 나라의 백성들을 구하는 영웅이 되시옵소서.\r\n");
        yield return StartCoroutine(CoWaitClick());
        Time.timeScale = 1f;
        GameManager.Instance.DoneTutorial = true;
        StageManager.Instance.Victory();
        GameManager.Instance.SelectedStageID++;
        Destroy(gameObject);

    }
}