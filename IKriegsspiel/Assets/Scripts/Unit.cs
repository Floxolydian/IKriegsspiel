using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Unit : MonoBehaviour
{
    public Texture2D texture;
    public float decalHeightOffset = 0.01f;
    GameObject selectionIndicator;

    void Awake()
    {
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>("unit");
            if (texture == null)
            {
                TextAsset txt = Resources.Load<TextAsset>("unit");
                if (txt != null)
                {
                    byte[] data = System.Convert.FromBase64String(txt.text);
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(data);
                }
            }
        }
        if (texture != null)
            ApplyTexture(texture);

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        CreateSelectionIndicator();
        SetSelected(false);
    }

    void ApplyTexture(Texture2D tex)
    {
        Transform top = transform.Find("Top");
        if (top == null)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.name = "Top";
            quad.transform.SetParent(transform, false);
            top = quad.transform;
        }
        var collider = GetComponent<Collider>();
        var bounds = collider.bounds;

        // place the decal at the very top of the collider regardless of pivot
        Vector3 worldTopCenter = new Vector3(bounds.center.x,
                                             bounds.max.y + decalHeightOffset,
                                             bounds.center.z);
        top.position = worldTopCenter;

        // keep the orientation relative to the parent object
        top.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);

        // adjust scale so the decal matches the collider dimensions in world space
        Vector3 parentScale = transform.lossyScale;
        top.localScale = new Vector3(bounds.size.x / parentScale.x,
                                     bounds.size.z / parentScale.z,
                                     1f);

        var rend = top.GetComponent<MeshRenderer>();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");
        var mat = new Material(shader);
        mat.mainTexture = tex;
        rend.material = mat;
    }

    void CreateSelectionIndicator()
    {
        var collider = GetComponent<Collider>();
        if (collider == null) return;

        var bounds = collider.bounds;
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(ring.GetComponent<Collider>());
        ring.name = "SelectionIndicator";
        ring.transform.SetParent(transform, false);

        float radiusX = bounds.size.x * 1.1f;
        float radiusZ = bounds.size.z * 1.1f;
        ring.transform.localScale = new Vector3(radiusX / transform.lossyScale.x, 0.05f, radiusZ / transform.lossyScale.z);
        ring.transform.localPosition = Vector3.zero;
        var rend = ring.GetComponent<MeshRenderer>();
        var outlineShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (outlineShader == null)
            outlineShader = Shader.Find("Unlit/Color");
        var outlineMat = new Material(outlineShader);
        outlineMat.color = Color.yellow;
        rend.material = outlineMat;
        ring.SetActive(false);
        selectionIndicator = ring;
    }

    public void SetSelected(bool selected)
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(selected);
    }
}
