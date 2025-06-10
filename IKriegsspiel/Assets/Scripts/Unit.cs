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
        var size = collider.bounds.size;
        top.localPosition = new Vector3(0f, size.y / 2f + decalHeightOffset, 0f);
        top.localRotation = Quaternion.Euler(90f, 0f, 0f);
        top.localScale = new Vector3(size.x, size.z, 1f);

        var rend = top.GetComponent<MeshRenderer>();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");
        var mat = new Material(shader);
        mat.mainTexture = tex;
        rend.material = mat;
    }
}
