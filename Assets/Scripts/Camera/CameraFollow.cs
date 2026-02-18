using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followLerp = 0.5f;

    private Transform followTarget;
    private Transform roomTarget;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    private void FixedUpdate()
    {
        if (followTarget == null) return;

        Vector3 targetPosition;

        if (roomTarget != null)
            targetPosition = (followTarget.position + roomTarget.position) * 0.5f;
        else
            targetPosition = followTarget.position;

        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followLerp
        );
    }

    public void SetRoomTarget(Transform target)
    {
        roomTarget = target;
    }
}
