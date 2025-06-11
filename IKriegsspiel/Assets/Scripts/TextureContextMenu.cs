using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureContextMenu : MonoBehaviour
{
#if UNITY_EDITOR
    void OnMouseOver()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            ShowMenu();
        }
    }

    void ShowMenu()
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Select texture"), false, OnSelectTexture);
        Vector2 pos = Mouse.current.position.ReadValue();
        pos.y = Screen.height - pos.y;
        menu.DropDown(new Rect(pos, Vector2.zero));
    }

    void OnSelectTexture()
    {
        string path = EditorUtility.OpenFilePanel("Select texture", "", "png,jpg,jpeg");
        if (string.IsNullOrEmpty(path))
            return;

        byte[] data = System.IO.File.ReadAllBytes(path);
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
#endif
}
