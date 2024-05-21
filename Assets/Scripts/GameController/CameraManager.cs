using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;
    public TouchManager touchManager;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    public float xLeftBound;
    public float xRightBound;

    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
    }

    public void Update()
    {
        if (touchManager.Moved)
        {
            MoveCamera(touchManager.WorldDeltaPos, Vector3.right);
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
        transform.position = new(
            lockX ? transform.position.x : Mathf.Clamp(position.x,
            (pos1.position.x < pos2.position.x ? pos1.position.x : pos2.position.x) + xLeftBound,
            (pos1.position.x > pos2.position.x ? pos1.position.x : pos2.position.x) - xRightBound),
            lockY ? transform.position.y : Mathf.Clamp(position.y,
            pos1.position.y < pos2.position.y ? pos1.position.y : pos2.position.y,
            pos1.position.y > pos2.position.y ? pos1.position.y : pos2.position.y),
            lockZ ? transform.position.z : Mathf.Clamp(position.z,
            pos1.position.z < pos2.position.z ? pos1.position.z : pos2.position.z,
            pos1.position.z > pos2.position.z ? pos1.position.z : pos2.position.z));
    }
}
