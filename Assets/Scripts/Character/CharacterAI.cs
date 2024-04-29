using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public class CharacterAI : MonoBehaviour
{
    public CharacterData charData;
    public BoxCollider2D attackBox;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private TextMeshPro text;

    public bool IsPlayer { get; set; }
    private List<CharacterAI> targets = new();
    private int hp;
    private int Hp
    {
        get => hp;
        set
        {
            hp = value;
            text.text = hp.ToString();
        }
    }
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        text = GetComponentInChildren<TextMeshPro>();

        
        Hp = charData.maxHp;
        attackBox.size = Vector2.right * charData.attackRange + Vector2.up;
        attackBox.offset = Vector2.right * (1f + (charData.attackRange - 1f) * 0.5f) * (spriteRenderer.flipX ? -1f : 1f);
        lastAttackTime = Time.time - 1f / charData.attackSpeed;
        spriteRenderer.color = charData.color;
    }

    private void Update()
    {

        if (Time.time - lastAttackTime >= 1f / charData.attackSpeed)
        {
            bool isAttacked = false;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null || targets[i].hp <= 0)
                    continue;

                targets[i].Hp -= charData.attackdamage;
                isAttacked = true;
                if (targets[i].Hp <= 0)
                {
                    Destroy(targets[i].gameObject);
                    targets[i] = null;
                }
            }

            if (isAttacked)
                lastAttackTime = Time.time;

        }

        targets.RemoveAll(x => x == null);

        if (targets.Count > 0)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        else
        {
            rb.velocity = Vector2.right * 1f * (spriteRenderer.flipX ? -1f : 1f);
            rb.isKinematic = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        var charAI = collision.GetComponent<CharacterAI>();
        if (charAI == null)
            return;

        if (charAI.IsPlayer != IsPlayer)
        {
            if (!targets.Contains(charAI))
            {
                targets.Add(charAI);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        var charAI = collision.GetComponent<CharacterAI>();
        if (charAI == null)
            return;

        if (charAI.IsPlayer != IsPlayer)
            targets[targets.FindIndex(x => x == charAI)] = null;
    }
}
