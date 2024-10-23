
using System;
using System.Linq;

using UnityEngine;

public interface IDialogueAction {
    // interface for the effects of a line in a dialogue file.
    // eg. Changes in blocking, or text to display

    public bool wait_for_player_input { get; }
    public void ResolveDialogue(DialogueController ctrl);
}


public class DialogueNoOp : IDialogueAction {
    // no-op dialogue action
    private bool _wait_for_player_input;
    public bool wait_for_player_input { get { return _wait_for_player_input; }}

    public DialogueNoOp() : this (false) { /* do nothing */ }
    public DialogueNoOp(bool player_input) {
        this._wait_for_player_input = player_input;
    }
    public void ResolveDialogue(DialogueController ctrl) {
        // do nothing: no-op
    }
}


public class DialogueSpeach : IDialogueAction {
    // class for representing one line of text spoken by a character

    public bool wait_for_player_input { get; set; }

    public string cmd { get; private set; }
    public string speaker { get; private set; }
    public string text { get; private set; }

    public DialogueSpeach(string[] args) {
        wait_for_player_input = true;
        cmd = args[0];
        speaker = args[1];

        // set the dialogue's text to the remaining contents of the line of dialogue, after the command name and speaker name are used
        text = string.Join(" ", args.Skip(2));
    }

    public void ResolveDialogue(DialogueController dialogue) {
        dialogue.SetSpeakerName(speaker); // TODO --- implement naration text
        dialogue.SetText(text);
    }
}


public class DialogueBlocking : IDialogueAction {
    // class representing a new character entering the scene
    public bool wait_for_player_input { get; set; }
    
    public string cmd { get; private set; }
    public string actor_name { get; private set; }
    public StageDirection side { get; private set; }
    public StageDirection facing { get; private set; }
    public string pose { get; private set; }


    public DialogueBlocking(string[] args) {
        if (!(args.Length == 3 || args.Length == 5 || args.Length == 6)) {
            throw new DialogueActionsException("BLOCKING command usage: `enter character right` or `blocking character left facing right` or `enter character left facing right <pose>`");
        }
        wait_for_player_input = false;
        cmd = args[0];
        actor_name = args[1];
        side = DialogueActionUtil.StageDirectionFromString(args[2]);

        if (side == StageDirection.unspecified) {
            throw new DialogueActionsException($"new characters in a dialogue must be on the left or right");
        }

        if (args.Length == 5) {
            facing = DialogueActionUtil.StageDirectionFromString(args[4]);
            pose = null;
        } 
        else if (args.Length == 6) {
            facing = DialogueActionUtil.StageDirectionFromString(args[4]);
            pose = args[5];
        } else {
            facing = StageDirection.unspecified;
            pose = null;
        }
        // if facing is not given, place the character facing 
        if (facing == StageDirection.unspecified) {
            facing = DialogueActionUtil.StageDirectionOposite(side);
        }
    }
    public void ResolveDialogue(DialogueController ctrl) {
        // `enter` command sets the portrait direction, but doesn't support posing
        if (pose == null) {
            ctrl.SetPortrait(actor_name, side, facing);
        } else {
            ctrl.SetPortrait(actor_name, pose, side, facing);
        }
    }
}

public class DialoguePose : IDialogueAction {
    // class representing a new character entering the scene
    public bool wait_for_player_input { get; set; }
    
    public string cmd { get; private set; }
    public string actor_name { get; private set; }
    public string pose { get; private set; }


    public DialoguePose(string[] args) {
        if (args.Length < 3) {
            throw new DialogueActionsException("POSE command usage: `pose character angry`");
        }
        wait_for_player_input = false;
        cmd = args[0];
        actor_name = args[1];
        pose = args[2];
    }

    public void ResolveDialogue(DialogueController ctrl) {
        // `enter` command sets the portrait direction, but doesn't support posing
        ctrl.UpdatePose(actor_name, pose);
    }
}

public class DialogueExit : IDialogueAction {
    
    public bool wait_for_player_input { get; }
    public string cmd { get; private set; }
    public string actor_name { get; private set; }

    public DialogueExit(string[] args) {
        cmd = args[0];
        actor_name = args[1];
    }
    public void ResolveDialogue(DialogueController ctrl) {
        ctrl.RemovePortrait(actor_name);
    }
}

// public class DialogueBlocking : IDialogueAction {
//     // class representing changes to blocking during dialogue (character portrait changes
//     public bool wait_for_player_input { get; set; }
    
//     public string cmd { get; private set; }
//     public string actor { get; private set; }
//     public string text { get; private set; }


//     public DialogueBlocking(string[] args) {
//         // TODO --- implement this
//         throw new NotImplementedException("DialogueBlocking IDialogueAction is not yet implemented");
//     }
//     public void ResolveDialogue(DialogueController ctrl) {
//         // TODO --- implement this
//         throw new NotImplementedException("DialogueBlocking IDialogueAction is not yet implemented");
//     }

// }



[System.Serializable]
public class DialogueActionsException : System.Exception
{
    // public DialogueActionsException() { }
    public DialogueActionsException(string message) : base(message) { }
    // public DialogueActionsException(string message, System.Exception inner) : base(message, inner) { }
    // protected DialogueActionsException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}