using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public RayReceiver receiver;

    public bool Tap { get; private set; }
    public bool Touch { get; private set; }
    public bool Moved { get; private set; }
    public Vector2 Pos { get; private set; }
    public Vector3 WorldPos => Camera.main.WorldToScreenPoint(Pos);
    public Vector2 DeltaPos { get; private set; }
    public Vector3 WorldDeltaPos { get; private set; }
    public Vector2 PrevPos { get; private set; }

    private bool firstIDMoved;

    private float dpi;
    private int firstID;

    private void Awake()
    {
        dpi = Screen.dpi;
    }

    private void Update()
    {
        Tap = false;
        Touch = false;

        int touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            Touch = true;

            foreach (var touch in Input.touches)
            {
                if (firstID == touch.fingerId)
                    Pos = touch.position;
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (firstID == 0)
                        { 
                            firstID = touch.fingerId;
                            Pos = touch.position;
                            PrevPos = touch.position;
                        }
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (firstID != touch.fingerId)
                            break;
                        DeltaPos = touch.deltaPosition;
                        Moved = DeltaPos != Vector2.zero;
                        if (!firstIDMoved)
                            firstIDMoved = Moved;
                        WorldDeltaPos = Camera.main.ScreenToWorldPoint(Pos) - Camera.main.ScreenToWorldPoint(PrevPos);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (firstID != touch.fingerId)
                            break;
                        Tap = !firstIDMoved;
                        firstID = 0;
                        Moved = false;
                        firstIDMoved = false;
                        break;
                }
                if (firstID == touch.fingerId)
                    PrevPos = touch.position;
            }
        }
    }
}
