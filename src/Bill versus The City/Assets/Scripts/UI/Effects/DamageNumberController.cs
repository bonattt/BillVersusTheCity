using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DamageNumberController : MonoBehaviour
{
    
    public string damage_value = "10";
    public Color text_color = new Color(0.9f, 0.05f, 0.05f);
    public Color outline_color = new Color(0.55f, 0f, 0f);
    public float duration = 3f;
    public float scatter = 0f;
    public Vector3 start_offset = new Vector3(0f, 0f, 0f);
    public float upware_float = 0f;

    public UIDocument uiDocument;
    private Label damage_label;

    void Start() {
        Destroy(this.gameObject, duration);
        damage_label = uiDocument.rootVisualElement.Q<Label>("Damage");
        UpdateText();
        ScatterStartOffset();
        transform.position += start_offset;
    }

    private void ScatterStartOffset() {
        if (scatter != 0f) {
            float scatter_x = Random.Range(-scatter, scatter);
            float scatter_y = 0f;
            float scatter_z = Random.Range(-scatter, scatter);
            Vector3 scatter_vector = new Vector3(scatter_x, scatter_y, scatter_z);
            start_offset += scatter_vector;
        }
    }

    private void UpdateText() {
        damage_label.text = damage_value;
        damage_label.style.color = text_color;

        // TextShadow outline = new TextShadow();
        // outline.offset = new Vector2(1, 1);  // Offset the shadow
        // outline.color = outline_color;        // Shadow color
        // outline.blurRadius = 0;               // No blur for a sharp outline
        
        // TextShadow outline = new TextShadow{
        //     offset = new Vector2(-1, -1),
        //     color = outline_color,
        //     blurRadius = 0
        // };
        damage_label.style.unityTextOutlineColor = outline_color;
    }

    void Update() {
        UpdatePosition();
        UpdateText();
    }

    private void UpdatePosition() {
        // makes the damage numbers float upwards over time
        float upwards_movement = upware_float * Time.deltaTime;
        transform.position += new Vector3(0f, upwards_movement, 0f); 
    }
}
