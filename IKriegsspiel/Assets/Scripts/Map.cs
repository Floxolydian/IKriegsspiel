using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Map : MonoBehaviour
{
    public Texture2D texture;
    public float dynamicFriction = 1f;
    public float staticFriction = 1f;

    void Awake()
    {
        // assign friction
        var coll = GetComponent<Collider>();
        if (coll != null)
        {
            var pm = new PhysicMaterial();
            pm.dynamicFriction = dynamicFriction;
            pm.staticFriction = staticFriction;
            pm.bounciness = 0f;
            coll.material = pm;
        }

        // assign texture
        if (texture == null)
        {
            // try loading as Texture2D first
            texture = Resources.Load<Texture2D>("map");
            if (texture == null)
            {
                // fallback: decode from a base64 text asset placeholder
                TextAsset txt = Resources.Load<TextAsset>("map");
                if (txt != null)
                {
                    byte[] data = System.Convert.FromBase64String(txt.text);
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(data);
                }
            }
        }
        if (texture != null)
        {
            var rend = GetComponent<MeshRenderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = texture;
            rend.material = mat;
        }
    }
}
