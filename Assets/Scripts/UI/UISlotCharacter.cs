﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public RawImage rawImage;
    public CharacterInfos characterInfos;

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        textName.text = DataTableManager.GetString(characterInfos.unitData.prefab);

        if (characterInfos.dress != null)
        {
            rawImage.uvRect = GameObject.FindWithTag(Tags.uiManager).GetComponent<UIManager>().AddSlotRenderers(characterInfos.dress);
        }
    }
}
