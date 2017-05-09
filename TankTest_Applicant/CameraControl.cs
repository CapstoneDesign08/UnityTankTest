using UnityEngine;
public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float m_MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    public Transform m_Target; // All the targets the camera needs to encompass.

    private Camera m_Camera;                        // Used for referencing the camera.
    private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
            Move();
    }

    public void Start()
    {
        m_Target = GameObject.FindWithTag("Tank").transform;

    }
    private void Move()
    {
        // Smoothly transition to that position. add tank position
        transform.position = Vector3.SmoothDamp(transform.position, m_Target.position, ref m_MoveVelocity, m_DampTime);
    }

    public void SetStartPositionAndSize()
    {
        // Set the camera's position to the desired position without damping.
        transform.position = m_Target.position;

    }
}
