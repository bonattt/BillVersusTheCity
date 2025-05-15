using UnityEngine;
using UnityEngine.UIElements;

public class UIDocFollowMouse : MonoBehaviour    {
    
    private UIDocument ui_doc;
    private VisualElement visual_element;
    private Camera _camera;
    public Vector2 offset = new Vector2(0f, -75f); 
    public Vector2 debug_mouse_position;

    void Start() {
        _camera = Camera.main;
        ui_doc = GetComponent<UIDocument>();
        visual_element = ui_doc.rootVisualElement;
    }
    void LateUpdate() {
        UpdatePosition();
    }

    private void UpdatePosition() {
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            visual_element.panel, InputSystem.current.MouseWorldPosition(), _camera
        ) + offset;
        debug_mouse_position = newPosition - offset;
        visual_element.transform.position = new Vector2(
            newPosition.x - (visual_element.layout.width / 2),
            newPosition.y // - (visual_element.layout.height / 2)
        );
    }
}