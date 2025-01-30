using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DamageNumberController : AbstractFloaterEffect
{
    
    public string damage_value = "10";

    private Label damage_label;

    // void Start() {
    //     Destroy(this.gameObject, duration);
    //     damage_label = uiDocument.rootVisualElement.Q<Label>("Damage");
    //     UpdateText();
    //     ScatterStartOffset();
    //     transform.position += start_offset;
    // }

    // private void ScatterStartOffset() {
    //     if (scatter != 0f) {
    //         float scatter_x = Random.Range(-scatter, scatter);
    //         float scatter_y = 0f;
    //         float scatter_z = Random.Range(-scatter, scatter);
    //         Vector3 scatter_vector = new Vector3(scatter_x, scatter_y, scatter_z);
    //         start_offset += scatter_vector;
    //     }
    // }

    public override void SetupEffect() {
        damage_label = uiDocument.rootVisualElement.Q<Label>("Damage");
    }

    public override void UpdateEffect() {
        UpdateText();
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

    // void Update() {
    //     UpdatePosition();
    //     UpdateText();
    // }

    // private void UpdatePosition() {
    //     // makes the damage numbers float upwards over time
    //     float upwards_movement = upware_float * Time.deltaTime;
    //     transform.position += new Vector3(0f, upwards_movement, 0f); 
    // }
}
