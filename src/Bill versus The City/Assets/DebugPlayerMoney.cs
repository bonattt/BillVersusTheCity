using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerMoney : MonoBehaviour
{
    public bool show_ui = true;
    public int dollars = -1;
    public int dollars_earned_in_level = -1;
    public int total_dollars = -1;

    void Update()
    {
        dollars = PlayerCharacter.inst.inventory.dollars;
        dollars_earned_in_level = PlayerCharacter.inst.inventory.dollars_earned_in_level;
        total_dollars = PlayerCharacter.inst.inventory.total_dollars;
    }


    private static GUIStyle _label_style = null;
    private static GUIStyle label_style {
        get {
            if (_label_style == null) {
                _label_style = new GUIStyle(GUI.skin.label);
                _label_style.fontSize = 40;
                _label_style.normal.textColor = Color.black;
                _label_style.alignment = TextAnchor.UpperLeft;;
            }
            return _label_style;
        }
    }
    void OnGUI() {
        if (!show_ui || MenuManager.inst.paused) { return; }
        float rect_width = 300f;
        int start_x = 25;
        int start_y = 350;
        GUILayout.BeginArea(new Rect(start_x, start_y, rect_width, rect_width));
        GUILayout.Label($"Money: ${PlayerCharacter.inst.inventory.total_dollars}.00", label_style);
        GUILayout.EndArea();
        Debug.Log($"pootis! new Rect(start_x: {start_x}, start_y: {start_y}, rect_width: {rect_width})");
    }
}
