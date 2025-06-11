using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Unit : MonoBehaviour
{
    public Texture2D texture;
    public float decalHeightOffset = 0.01f;


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


    }

    /// <summary>
    /// Replace the current unit texture at runtime.
    /// </summary>
    /// <param name="tex">Texture to apply on the top side.</param>
    public void SetTexture(Texture2D tex)
    {
        texture = tex;
        ApplyTexture(tex);
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

        Vector3 worldTopCenter;
        Vector3 worldSize;
        if (collider is BoxCollider box)
        {
            // BoxCollider provides reliable local size information that isn't
            // affected by the object's rotation. Use it to compute the world
            // dimensions and top position so copied/rotated units size correctly.
            worldSize = Vector3.Scale(box.size, transform.lossyScale);
            Vector3 worldCenter = transform.TransformPoint(box.center);
            worldTopCenter = worldCenter + transform.up * (worldSize.y / 2f + decalHeightOffset);
        }
        else
        {
            // Fallback for other collider types.
            var bounds = collider.bounds;
            worldTopCenter = new Vector3(bounds.center.x,
                                         bounds.max.y + decalHeightOffset,
                                         bounds.center.z);
            worldSize = bounds.size;
        }

        top.position = worldTopCenter;

        // keep the orientation relative to the parent object
        top.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);

        // adjust scale so the decal matches the collider dimensions in world space
        Vector3 parentScale = transform.lossyScale;
        top.localScale = new Vector3(worldSize.x / parentScale.x,
                                     worldSize.z / parentScale.z,
                                     1f);

        var rend = top.GetComponent<MeshRenderer>();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");
        var mat = new Material(shader);
        mat.mainTexture = tex;
        rend.material = mat;
    }


}
