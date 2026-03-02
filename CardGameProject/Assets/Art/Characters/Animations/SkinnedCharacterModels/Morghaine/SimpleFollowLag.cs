using UnityEngine;

public class SimpleFollowLag : MonoBehaviour
{
    public Transform followTarget;
    public float followSpeed = 5f;
    public float rotationLag = 10f;

    private Quaternion targetRot;

    void LateUpdate()
    {
        if (!followTarget) return;

        // Target rotation = same as head
        targetRot = followTarget.rotation;

        // Smoothly rotate toward it
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationLag);

        // Optional: small positional follow for bounce
        transform.position = Vector3.Lerp(transform.position, followTarget.position,
                                          Time.deltaTime * followSpeed);
    }
}