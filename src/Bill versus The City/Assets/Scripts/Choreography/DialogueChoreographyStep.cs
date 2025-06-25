using UnityEngine;

public class DialogueChoreographyStep : AbstractChoreographyStep, IGameEventEffect {

    public string dialogue_file_path;

    private DialogueController opened_dialogue = null;

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        opened_dialogue = MenuManager.inst.OpenDialoge(dialogue_file_path);
        opened_dialogue.AddCloseCallback(this);
    }

    public override void Complete() {
        base.Complete();
        // opened_dialogue = null;
    }

    public bool effect_completed { get => this.choreography_complete; }

    public void ActivateEffect() {
        // NOTE: the IGameEventEffect for DialougeChoreographyStep is to mark the ChoreographyStep as completed, not to lauch the choreography step
        Complete();
    }
}