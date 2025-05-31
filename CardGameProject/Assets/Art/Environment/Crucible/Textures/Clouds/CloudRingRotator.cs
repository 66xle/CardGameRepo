using UnityEngine;

public class CloudRingRotator : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 10f;

    [Tooltip("Rotation axis (e.g., Vector3.up for Y-axis)")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }
}