using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class ContextMenuManager : MonoBehaviour
{
    public Camera cam;

    bool menuOpen;
    Object targetObject; // Map or Unit
    Vector2 menuPosition;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
            TryOpenMenu();

        if (menuOpen && Mouse.current.leftButton.wasPressedThisFrame)
            menuOpen = false;
    }

    void TryOpenMenu()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var unit = hit.collider.GetComponentInParent<Unit>();
            var map = hit.collider.GetComponentInParent<Map>();
            if (unit != null)
                targetObject = unit;
            else if (map != null)
                targetObject = map;
            else
                return;

            menuOpen = true;
            var mousePos = Mouse.current.position.ReadValue();
            menuPosition = new Vector2(mousePos.x, Screen.height - mousePos.y);
        }
    }

    void OnGUI()
    {
        if (!menuOpen || targetObject == null)
            return;

        const float width = 120f;
        const float height = 25f;
        Rect rect = new Rect(menuPosition.x, menuPosition.y, width, height);
        if (GUI.Button(rect, "Select texture"))
        {
            SelectTexture();
            menuOpen = false;
        }
    }

    void SelectTexture()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Select texture", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            if (targetObject is Unit u)
                u.SetTexture(tex);
            else if (targetObject is Map m)
                m.SetTexture(tex);
        }
#endif
    }
}
