using ScrollBGTest;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public ScrollBackgroundCtrl background;
    public Transform playerTowerPos;
    public Transform enemyTowerPos;

    public float xLeftBound;
    public float xRightBound;

    private float constant;
    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    public bool IsCameraMoved { get; private set; }

    private void Awake()
    {
        constant = 1 + (((float)Screen.width / Screen.height - 1f) - (2532f / 1170f - 1f)) / (2532f / 1170f - 1f) / 2f;
    }

    private void Start()
    {
        MinX = playerTowerPos.position.x + xLeftBound * constant;
        MaxX = enemyTowerPos.position.x - xRightBound * constant;
    }

    public void Update()
    {
        IsCameraMoved = false;
        if (GameManager.Instance.touchManager.Moved && GameManager.Instance.touchManager.receiver.Received)
        {
            MoveCamera(GameManager.Instance.touchManager.WorldDeltaPos * 1.3f, Vector3.right);
            IsCameraMoved = true;
        }
    }

    public void MoveCamera(Vector3 deltaPos) => MoveCamera(deltaPos, Vector3.zero);
    public void MoveCamera(Vector3 deltaPos, Vector3 reverseAxis)
    {
        SetCameraPosition(transform.position + new Vector3(
            deltaPos.x * (reverseAxis.x != 0f ? -1f : 1f),
            deltaPos.y * (reverseAxis.y != 0f ? -1f : 1f),
            deltaPos.z * (reverseAxis.z != 0f ? -1f : 1f)));
    }
    public void SetCameraPosition(Vector3 position)
    {
        background.MoveValue = Mathf.Clamp(position.x, MinX, MaxX);
        transform.position = new(
            background.MoveValue,
            transform.position.y,
            transform.position.z);
    }
}
