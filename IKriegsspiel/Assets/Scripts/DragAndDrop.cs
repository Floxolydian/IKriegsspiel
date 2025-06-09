using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float liftHeight = 1f;
    public float rotateSpeed = 10f;

    Camera cam;
    Rigidbody grabbed;
    Collider grabbedCol;
    PhysicMaterial noBounce;

    void Start()
    {
        cam = Camera.main;
        noBounce = new PhysicMaterial();
        noBounce.bounciness = 0f;
    }

    void Update()
    {
        if (grabbed == null)
        {
            if (Input.GetMouseButtonDown(0))
                BeginDrag();
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
                EndDrag();
            else
                UpdateDrag();
        }
    }

    void BeginDrag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var rb = hit.rigidbody;
            if (rb != null && rb.GetComponent<Map>() == null)
            {
                grabbed = rb;
                grabbedCol = hit.collider;
                grabbed.isKinematic = true;
                grabbed.useGravity = false;
                grabbed.velocity = Vector3.zero;
                grabbed.angularVelocity = Vector3.zero;
            }
        }
    }

    void UpdateDrag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float dist = Vector3.Distance(cam.transform.position, grabbed.transform.position);
        Vector3 pos = ray.GetPoint(dist);
        pos.y = Mathf.Max(pos.y, liftHeight);
        grabbed.transform.position = pos;

        float scroll = Input.mouseScrollDelta.y;
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
