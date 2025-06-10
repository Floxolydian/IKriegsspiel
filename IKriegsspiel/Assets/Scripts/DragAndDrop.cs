using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    public float liftHeight = 0.5f;
    public float rotateSpeed = 10f;

    Camera cam;
    Rigidbody grabbed;
    Collider grabbedCol;
    float grabDistance;

    PhysicsMaterial noBounce;

    void Start()
    {
        cam = Camera.main;
        noBounce = new PhysicsMaterial();
        noBounce.bounciness = 0f;
    }

    public bool IsDragging => grabbed != null;

    void Update()
    {
        if (grabbed == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                BeginDrag();
        }
        else
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();
            else
                UpdateDrag();
        }
    }

    void BeginDrag()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            var rb = hit.rigidbody;
            if (rb != null && rb.GetComponent<Map>() == null)
            {
                grabbed = rb;
                grabbedCol = hit.collider;
                grabDistance = hit.distance;

                grabbed.isKinematic = true;
                grabbed.useGravity = false;
                grabbed.linearVelocity = Vector3.zero;
                grabbed.angularVelocity = Vector3.zero;
                break;
            }
        }
    }

    void UpdateDrag()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 pos = ray.GetPoint(grabDistance);

        pos.y = Mathf.Max(pos.y, liftHeight);
        grabbed.transform.position = pos;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
            grabbed.transform.Rotate(Vector3.up, scroll * rotateSpeed, Space.World);
    }

    void EndDrag()
    {
        grabbed.isKinematic = false;
        grabbed.useGravity = true;
        if (grabbedCol != null)
        {
            if (grabbedCol.material == null)
                grabbedCol.material = noBounce;
            else
                grabbedCol.material.bounciness = 0f;
        }
        grabbed = null;
        grabbedCol = null;
    }
}
