
public enum StageDirection {
    left,
    right,
    // unspecified is generally not a valid option to select a character's direction
    // but can sometimes be used to leave a character wherever they already were
    unspecified
}

public enum StagePosition {
    unspecified=-1,
    far_left=0,
    center_left=1,
    center_right=2,
    far_right=3,
}


public enum IDialogueEvent {

}