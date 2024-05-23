using ScrollBGTest;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public ScrollBackgroundCtrl background;
    public Transform pos1;
    public Transform pos2;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    public float xLeftBound;
    public float xRightBound;

    private float constant;


    private void Awake()
    {
        constant = 1 + (((float)Screen.width / Screen.height - 1f) - (2532f / 1170f - 1f)) / (2532f / 1170f - 1f) / 2f;
    }

    public void Update()
    {
        if (GameManager.Instance.touchManager.Moved && GameManager.Instance.touchManager.receiver.Received)
        {
            MoveCamera(GameManager.Instance.touchManager.WorldDeltaPos*1.3f, Vector3.right);
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
        background.MoveValue = lockX ? transform.position.x : Mathf.Clamp(position.x,
            (pos1.position.x < pos2.position.x ? pos1.position.x : pos2.position.x) + xLeftBound * constant,
            (pos1.position.x > pos2.position.x ? pos1.position.x : pos2.position.x) - xRightBound * constant);
        transform.position = new(
            background.MoveValue,
            lockY ? transform.position.y : Mathf.Clamp(position.y,
            pos1.position.y < pos2.position.y ? pos1.position.y : pos2.position.y,
            pos1.position.y > pos2.position.y ? pos1.position.y : pos2.position.y),
            lockZ ? transform.position.z : Mathf.Clamp(position.z,
            pos1.position.z < pos2.position.z ? pos1.position.z : pos2.position.z,
            pos1.position.z > pos2.position.z ? pos1.position.z : pos2.position.z));
    }
}
