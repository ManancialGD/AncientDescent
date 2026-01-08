using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followLerp = 0.5f;

    private Transform followTarget;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
    
    private void FixedUpdate()
    {
        if (followTarget == null) return;

        Vector3 target = followTarget.position;
        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, followLerp);
    }
}
