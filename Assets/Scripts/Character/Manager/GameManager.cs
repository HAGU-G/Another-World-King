using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform Left;
    public Transform Right;

    public CharacterAI character;

    public CharacterData player;
    public CharacterData enemy;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var c = Instantiate(character, Left.position, Quaternion.identity);
            c.charData = player;
            c.spriteRenderer.flipX = false;
            c.IsPlayer = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            var c = Instantiate(character, Right.position, Quaternion.identity);
            c.charData = enemy;
            c.spriteRenderer.flipX = true;
            c.IsPlayer = false;
        }
    }
}
