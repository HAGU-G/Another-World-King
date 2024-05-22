using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public bool Touch { get; private set; }
    public bool Moved { get; private set; }
    public Vector2 Pos { get; private set; }
    public Vector3 WorldPos => Camera.main.WorldToScreenPoint(Pos);
    public Vector2 DeltaPos { get; private set; }
    public Vector3 WorldDeltaPos { get; private set; }

    private float dpi;

    private void Awake()
    {
        dpi = Screen.dpi;
    }

    private void Update()
    {
        bool isTouched = Touch;
        Touch = false;

        int touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            Touch = true;

            float x = 0;
            float y = 0;
            foreach (var touch in Input.touches)
            {
                x += touch.position.x;
                y += touch.position.y;
            }
            x /= touchCount;
            y /= touchCount;
            UpdatePos(new Vector2(x, y));
        }
        else if (Input.GetMouseButton(0))
        {
            Touch = true;

            UpdatePos(Input.mousePosition);
        }

        Moved = Touch && isTouched && DeltaPos != Vector2.zero;
    }

    private void UpdatePos(Vector2 newPos)
    {
        DeltaPos = newPos - Pos;
        WorldDeltaPos = Camera.main.ScreenToWorldPoint(newPos) - Camera.main.ScreenToWorldPoint(Pos);
        Pos = newPos;
    }
}
