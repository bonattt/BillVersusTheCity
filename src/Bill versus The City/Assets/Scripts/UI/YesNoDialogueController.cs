using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class YesNoDialogueController : DialogueController
{
    public string reject_text = "no";

    protected Label reject_label;
    public Button cancel_button { get; protected set; }

    public override void Configure() {
        // setup object references to UI components.
        base.Configure();
        
        reject_label = root_visual.Q<Label>("CancelLabel");
        cancel_button = root_visual.Q<Button>("CancelButton");
        cancel_button.RegisterCallback<ClickEvent>(MenuManager.CloseMenuClick);
    }
    
    public override void UpdateLabels() {
        base.UpdateLabels();
        reject_label.text = reject_text;
    }
}
