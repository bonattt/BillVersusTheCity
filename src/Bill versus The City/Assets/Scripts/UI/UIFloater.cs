using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UIFloater : MonoBehaviour
{
    // Script for displaying a UI Document over a GameObject in a scene

    public Vector2 offset = new Vector2(0, 0);
    public Transform TransformToFollow;

    private VisualElement root_visual_element;
    private Camera main_camera;

    private void Start() {
        main_camera = Camera.main;
        root_visual_element = GetComponent<UIDocument>().rootVisualElement;

        ConfigureStyles();
        SetPosition();
    }

    private void ConfigureStyles()
    {
        // TODO --- refactor this with ReloadUI
        root_visual_element.style.alignItems = Align.Center;
        root_visual_element.style.alignSelf = Align.Center;
        root_visual_element.style.justifyContent = Justify.Center;
        root_visual_element.style.position = Position.Absolute;
    }

    private void LateUpdate()
    {
        if (TransformToFollow != null)
        {
            SetPosition();
        }
    }

    public void SetPosition() {
        // Debug.Log(m_Bar);
        // Transforms a world absolute position to a local coordinate on a panel
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            root_visual_element.panel, TransformToFollow.position, main_camera
        );
        root_visual_element.transform.position = new Vector2(
            newPosition.x - root_visual_element.layout.width / 2,
            newPosition.y - root_visual_element.layout.height / 2
        ) + offset;
    }
}
