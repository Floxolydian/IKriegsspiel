using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Windows.Forms;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureContextMenu : MonoBehaviour
{
    bool showMenu;
    Vector2 menuPos;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            showMenu = true;
            menuPos = Mouse.current.position.ReadValue();
        }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            showMenu = false;
        }
    }

    void OnGUI()
    {
        if (!showMenu)
            return;

        Rect rect = new Rect(menuPos.x, Screen.height - menuPos.y, 150, 25);
        if (GUI.Button(rect, "Select texture"))
        {
            showMenu = false;
            string path = OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
                LoadTexture(path);
        }
    }

    string OpenFileDialog()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        var ofd = new OpenFileDialog();
        ofd.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
        ofd.Multiselect = false;
        return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
#elif UNITY_EDITOR
        return EditorUtility.OpenFilePanel("Select texture", "", "png,jpg,jpeg");
#else
        return null;
#endif
    }

    void LoadTexture(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2);
        tex.LoadImage(data);

        var unit = GetComponent<Unit>();
        if (unit != null)
        {
            unit.texture = tex;
            unit.ApplyTexture(tex);
            return;
        }
        var map = GetComponent<Map>();
        if (map != null)
        {
            map.texture = tex;
            map.ApplyTexture(tex);
        }
    }
}
