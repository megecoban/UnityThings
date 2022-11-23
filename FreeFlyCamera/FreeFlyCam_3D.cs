using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCam_3D : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool isActive = true;
    private Vector3 startPos;
    private Quaternion startRot;
    [Space]

    [Header("Cursor Settings")]
    [SerializeField] private CursorLockMode lockModeForCursor = CursorLockMode.Locked;
    [Space]

    [Header("Keys")]
    [SerializeField] private KeyCode resetToStartPosKey = KeyCode.P;
    [SerializeField] private KeyCode resetToStartRotationKey = KeyCode.R;
    [SerializeField] private KeyCode movementKeyForward = KeyCode.W;
    [SerializeField] private KeyCode movementKeyBack = KeyCode.S;
    [SerializeField] private KeyCode movementKeyRight = KeyCode.D;
    [SerializeField] private KeyCode movementKeyLeft = KeyCode.A;
    [SerializeField] private KeyCode speedKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode slowdownKey = KeyCode.LeftControl;
    [Space]

    [Header("Movement Speed Settings")]
    [SerializeField] [Min(0.000001f)] private float speed = 1f;
    [SerializeField] [Min(0)] private float speedMultipler = 2f;
    [SerializeField] [Min(0.000001f)] private float speedDivider = 2f;
    private Vector3 movementDirection = Vector3.zero;
    [Space]

    [Header("Rotation Settings")]
    [SerializeField] [Min(0)] private float mouseSensitivity = 1f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    [Space]

    [Header("Zoom Settings")]
    [SerializeField] [Min(0)] private float zoomSensitivity = 1f;
    [SerializeField][Min(1)] private int maxZoomIn = 2;
    [SerializeField][Min(1)] private int maxZoomOut = 179;


    void Start()
    {
        mouseX = this.transform.rotation.y;
        mouseY = this.transform.rotation.x;
        
        Cursor.lockState = lockModeForCursor;

        startPos = this.transform.position;
        startRot = this.transform.rotation;
    }

    void Update()
    {
        if (isActive == false)
            return;

        if (maxZoomOut < maxZoomIn)
            maxZoomOut = maxZoomIn;

        if (Input.GetKey(resetToStartPosKey))
            this.transform.position = startPos;

        if (Input.GetKey(resetToStartRotationKey))
        {
            mouseX = startRot.y;
            mouseY = startRot.x;
            this.transform.rotation = startRot;
        }

        mouseX += Input.GetAxis("Mouse X");
        mouseY -= Input.GetAxis("Mouse Y");

        if (Cursor.lockState != lockModeForCursor)
            Cursor.lockState = lockModeForCursor;

        MovementSystem();
        RotationSystem();
        ZoomSystem();
    }

    private void MovementSystem()
    {
        int isForward = Input.GetKey(movementKeyForward) && Input.GetKey(movementKeyBack) ? 0 : Input.GetKey(movementKeyForward) ? 1 : Input.GetKey(movementKeyBack) ? -1 : 0;
        int isRight = Input.GetKey(movementKeyRight) && Input.GetKey(movementKeyLeft) ? 0 : Input.GetKey(movementKeyRight) ? 1 : Input.GetKey(movementKeyLeft) ? -1 : 0;

        movementDirection = isForward * this.transform.forward + isRight * this.transform.right;
        movementDirection = movementDirection.normalized;

        int movementType = Input.GetKey(speedKey) && Input.GetKey(slowdownKey) ? 0 : Input.GetKey(speedKey) ? 1 : Input.GetKey(slowdownKey) ? -1 : 0;

        Vector3 movementSpeed = movementType == 0 ? movementDirection * speed : movementType == 1 ? movementDirection * speed * speedMultipler : movementType == -1 ? movementDirection * speed / speedDivider : movementDirection * speed;
        movementSpeed *= Time.deltaTime;

        this.transform.position += movementSpeed;
    }

    private void RotationSystem()
    {
        this.transform.rotation = Quaternion.Euler(mouseY * mouseSensitivity, mouseX * mouseSensitivity, this.transform.rotation.z);
    }

    private void ZoomSystem()
    {
        this.GetComponent<Camera>().fieldOfView += (int)(Input.mouseScrollDelta.y * -1f * zoomSensitivity);

        if (maxZoomIn == maxZoomOut)
        {
            this.GetComponent<Camera>().fieldOfView = maxZoomOut;
            return;
        }

        if (this.GetComponent<Camera>().fieldOfView < maxZoomIn)
            this.GetComponent<Camera>().fieldOfView = maxZoomIn;
        if (this.GetComponent<Camera>().fieldOfView > maxZoomOut)
            this.GetComponent<Camera>().fieldOfView = maxZoomOut;
    }
}
