using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 followOffset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minHeight = 2f;
    public float maxHeight = 40f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 currentOffset;

    void Awake()
    { 
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        currentOffset = followOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentOffset.y -= scroll * zoomSpeed;
            currentOffset.z += scroll * zoomSpeed;
            currentOffset.y = Mathf.Clamp(currentOffset.y, minHeight, maxHeight);
            currentOffset.z = Mathf.Clamp(currentOffset.z, -maxHeight, -minHeight);
        }

        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        currentOffset = followOffset; 
    }

    public void ResetCamera()
    {
        target = null;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}