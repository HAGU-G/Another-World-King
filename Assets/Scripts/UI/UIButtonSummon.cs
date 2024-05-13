using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSummon : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public Button button;
    public CharacterInfos CharacterInfos { get; private set; } = new();

    public void SetData(CharacterInfos characterInfos)
    {
        CharacterInfos = characterInfos;
        cost.text = CharacterInfos.unitData.cost.ToString();
    }
}
