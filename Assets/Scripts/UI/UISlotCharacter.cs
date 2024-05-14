using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public GameObject pos;
    public CharacterInfos characterInfos = new();

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        textName.text = characterInfos.unitData.ignore;

        GameObject character = null;
        if (characterInfos.dress != null)
            character = Instantiate(characterInfos.dress, pos.transform);

        if (character != null)
        {
            character.transform.localScale = Vector3.one * 80f;
            var renderers = character.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sprite != null)
                {
                    renderer.AddComponent<CanvasRenderer>();
                    var r = renderer.AddComponent<Image>();
                    r.sprite = renderer.sprite;
                    r.color = renderer.color;
                }
            }

            //var animator = character.GetComponentInChildren<Animator>();
            //if (animator != null)
            //    animator.enabled = false;
        }

    }
}
