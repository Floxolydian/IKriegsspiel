using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Map : MonoBehaviour
{
    public Texture2D texture;
    public float dynamicFriction = 1f;
    public float staticFriction = 1f;
    [Tooltip("Maximum size of a single tile texture in pixels")]
    public int tileSize = 4096;

    void Awake()
    {
        // assign friction
        var coll = GetComponent<Collider>();
        if (coll != null)
        {
            var pm = new PhysicsMaterial();
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
            ApplyTexture(texture);
        }
    }

    /// <summary>
    /// Replace the current map texture at runtime.
    /// </summary>
    /// <param name="tex">Texture to display on the map.</param>
    public void SetTexture(Texture2D tex)
    {
        texture = tex;
        ApplyTexture(tex);
    }

    void ApplyTexture(Texture2D tex)
    {
        if (tex.width <= tileSize && tex.height <= tileSize)
        {
            ApplySingle(tex);
        }
        else
        {
            ApplyTiled(tex);
        }
    }

    void ApplySingle(Texture2D tex)
    {
        var rend = GetComponent<MeshRenderer>();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");
        var mat = new Material(shader);
        mat.mainTexture = tex;
        rend.material = mat;
    }

    void ApplyTiled(Texture2D tex)
    {
        var baseRenderer = GetComponent<MeshRenderer>();
        var size = baseRenderer.bounds.size;
        float unitsPerPixelX = size.x / tex.width;
        float unitsPerPixelZ = size.z / tex.height;
        baseRenderer.enabled = false;

        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");

        int tilesX = Mathf.CeilToInt(tex.width / (float)tileSize);
        int tilesY = Mathf.CeilToInt(tex.height / (float)tileSize);

        for (int y = 0; y < tilesY; y++)
        {
            for (int x = 0; x < tilesX; x++)
            {
                int w = Mathf.Min(tileSize, tex.width - x * tileSize);
                int h = Mathf.Min(tileSize, tex.height - y * tileSize);

                var tileTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                tileTex.SetPixels(tex.GetPixels(x * tileSize, y * tileSize, w, h));
                tileTex.Apply();

                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = $"Tile_{x}_{y}";
                quad.transform.SetParent(transform, false);
                quad.transform.localScale = new Vector3(w * unitsPerPixelX, h * unitsPerPixelZ, 1f);
                float posX = -size.x / 2f + (x * tileSize + w / 2f) * unitsPerPixelX;
                float posZ = -size.z / 2f + (y * tileSize + h / 2f) * unitsPerPixelZ;
                quad.transform.localPosition = new Vector3(posX, 0f, posZ);

                var qrend = quad.GetComponent<MeshRenderer>();
                var mat = new Material(shader);
                mat.mainTexture = tileTex;
                qrend.material = mat;
            }
        }
    }
}
