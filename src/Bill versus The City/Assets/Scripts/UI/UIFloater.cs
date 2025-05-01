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

    private VisualElement m_Bar;
    private Camera m_MainCamera;

    private void Start() {
        m_MainCamera = Camera.main;
        m_Bar = GetComponent<UIDocument>().rootVisualElement;
        
        SetPosition();
    }

    private void LateUpdate() {
        if (TransformToFollow != null) {
            SetPosition();
        }
    }

    public void SetPosition() {
        // Debug.Log(m_Bar);
        // Transforms a world absolute position to a local coordinate on a panel
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            m_Bar.panel, TransformToFollow.position, m_MainCamera
        );

        m_Bar.transform.position = new Vector2(
            newPosition.x - m_Bar.layout.width / 2,
            newPosition.y - m_Bar.layout.height / 2
        ) + offset;
    }
}
