using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStage : MonoBehaviour
{
    public UIButtonSummon[] buttonSummons;
    public TowerAI playerTower;
    // Start is called before the first frame update
    private void Start()
    {
        for(int i = 0; i < buttonSummons.Length; i++)
        {
            int index = i;
            buttonSummons[index].SetData(GameManager.Instance.GetExpedition(index));
            buttonSummons[index].button.onClick.AddListener(() => { playerTower.TrySpawnUnit(buttonSummons[index].CharacterInfos); });
        }
        
    }
}
