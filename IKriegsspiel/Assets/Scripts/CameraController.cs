using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 10f;
    public float rotateSpeed = 100f;
    public DragAndDrop dragAndDrop;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (dragAndDrop == null)
            dragAndDrop = FindFirstObjectByType<DragAndDrop>();
    }

    void Update()
    {
        HandleMove();
        HandleZoom();
        HandleRotate();
    }

    void HandleMove()
    {
        if (Keyboard.current == null) return;

        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction = forward * input.y + right * input.x;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        if (Mouse.current == null) return;
        if (dragAndDrop != null && dragAndDrop.IsDragging) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            transform.position += transform.forward * scroll * zoomSpeed * Time.deltaTime;
        }
    }

    void HandleRotate()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.middleButton.isPressed) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        float yaw = delta.x * rotateSpeed * Time.deltaTime;
        float pitch = -delta.y * rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }
}
