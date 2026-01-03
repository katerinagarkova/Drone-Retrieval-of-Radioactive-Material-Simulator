using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float ascendSpeed = 3f;
    public float rotationSpeed = 120f;
    public float maxVelocity = 6f;
    
    [Header("Waypoints")]
    public float waypointTolerance = 1.5f;
    
    private List<Vector3> waypoints = new List<Vector3>();
    private Rigidbody rb;
    private int currentWaypointIndex = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; 
        rb.drag = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (waypoints.Count > 0)
        {
            NavigateWaypoints();
        }
        
        MaintainHover();
        LimitVelocity();
    }

    public void MoveTo(Vector3 pos)
    {
        waypoints.Clear();
        waypoints.Add(pos);
        currentWaypointIndex = 0;
    }

    public void Teleport(Vector3 pos, Quaternion rot)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.position = pos;
        transform.rotation = rot;
        
        waypoints.Clear();
    }

    void NavigateWaypoints()
    {
        if (currentWaypointIndex >= waypoints.Count) return;

        Vector3 target = waypoints[currentWaypointIndex];
        Vector3 dir = (target - transform.position);
        Vector3 moveDir = new Vector3(dir.x, 0, dir.z).normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }

        rb.AddForce(moveDir * moveSpeed, ForceMode.Acceleration);

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                             new Vector2(target.x, target.z)) < waypointTolerance)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Count) waypoints.Clear();
        }
    }

    void MaintainHover()
    {
        float targetHeight = 10.0f;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 50f))
        {
            float currentHeight = transform.position.y - hit.point.y;
            float error = targetHeight - currentHeight;
            rb.AddForce(Vector3.up * error * ascendSpeed, ForceMode.Acceleration);
        }
    }

    void LimitVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity)
            rb.velocity = rb.velocity.normalized * maxVelocity;
    }
}