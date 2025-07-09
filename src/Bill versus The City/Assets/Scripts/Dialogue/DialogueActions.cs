
using System;
using System.Collections;
using System.Collections.Generic;
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


public class DialogueSpeachAction : IDialogueAction {
    // class for representing one line of text spoken by a character

    public bool wait_for_player_input { get; set; }

    public string cmd { get; private set; }
    public string speaker { get; private set; }
    public string text { get; private set; }

    public DialogueSpeachAction(string[] args) {
        wait_for_player_input = true;
        cmd = args[0];
        speaker = args[1];

        // set the dialogue's text to the remaining contents of the line of dialogue, after the command name and speaker name are used
        text = string.Join(" ", args.Skip(2));
    }

    public void ResolveDialogue(DialogueController dialogue) {
        dialogue.SetSpeakerName(speaker);
        dialogue.SetText(text);
    }
}


public class DialogueMoveAction : IDialogueAction {
    /** class representing a new character entering the scene, or moving around in the scene */
    public bool wait_for_player_input { get; set; }
    
    public string cmd { get; private set; }
    public string actor_name { get; private set; }
    // public StageDirection side { get; private set; }
    public StagePosition position { get; private set; }
    public StageDirection facing { get; private set; }
    public string pose { get; private set; }

    private const string USAGE_EXAMPLE = "`enter character right` or `move character left facing right` or `enter character left facing right <pose>`";

    public DialogueMoveAction(string[] args) {
        if (!(args.Length == 3 || args.Length == 5 || args.Length == 6)) {
            throw new DialogueActionsException($"MOVE/ENTER command usage: {USAGE_EXAMPLE}");
        }
        wait_for_player_input = false;
        cmd = args[0];
        actor_name = args[1];
        // side = DialogueActionUtil.StageDirectionFromString(args[2]);
        position = DialogueActionUtil.StagePositionFromString(args[2]);

        if (position == StagePosition.unspecified) {
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
            facing = DialogueActionUtil.StagePositionOposite(position);
        }
    }
    public void ResolveDialogue(DialogueController ctrl) {
        // `enter` command sets the portrait direction, but doesn't support posing
        if (pose == null) {
            ctrl.SetPortrait(actor_name, position, facing);
        } else {
            ctrl.SetPortrait(actor_name, pose, position, facing);
        }
    }
}

public class DialoguePoseAction : IDialogueAction {
    // class representing a new character entering the scene
    public bool wait_for_player_input { get; set; }
    
    public string cmd { get; private set; }
    public string actor_name { get; private set; }
    public string pose { get; private set; }


    public DialoguePoseAction(string[] args) {
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

public class DialogueExitAction : IDialogueAction {
    // dialogue action for a character exiting the scene
    public bool wait_for_player_input { get { return false; }}
    public string cmd { get; private set; }
    public string actor_name { get; private set; }

    public DialogueExitAction(string[] args) {
        cmd = args[0];
        actor_name = args[1];
    }
    public void ResolveDialogue(DialogueController ctrl) {
        ctrl.RemovePortrait(actor_name);
    }
}

public class DialougeAliasAction : IDialogueAction {
    
    public bool wait_for_player_input { get { return false; }}
    public string cmd { get; private set; }
    public string character_name { get; private set; }
    public string portrait_name { get; private set; }
    public string display_name { get; private set; }

    private const string USAGE_EXAMPLE = "Alias command usage `alias gangsta1 as gangsta` or `alias gangsta1 as gangsta show Daniel`";
    public DialougeAliasAction(string[] args) {
        if (args.Length < 4) {
            throw new DialogueActionsException($"expected at least 4 args, got {args.Length} in '{string.Join(", ", args)}': {USAGE_EXAMPLE}");
        }
        cmd = args[0];
        character_name = args[1];
        if (! args[2].ToLower().Equals("as")) {
            Debug.LogWarning($"expected 'as' got '{args[2]}': {USAGE_EXAMPLE}");
        }
        portrait_name = args[3];
        if (args.Length >= 5) {
            if (! args[4].ToLower().Equals("show")) {
                Debug.LogWarning($"expected 'show' got '{args[4]}': {USAGE_EXAMPLE}");
            }
            display_name = args[5];
        } else {
            display_name = character_name;
        }
    }
    public void ResolveDialogue(DialogueController ctrl) {
        ctrl.AddAlias(character_name, portrait_name, display_name);
    }
}

public class DialogueBlockingAction : IDialogueAction {
    // class representing changes to blocking during dialogue (character portrait changes
    public bool wait_for_player_input { get; set; }

    public string cmd { get; private set; }
    public List<string> actors { get; private set; }


    private const string USAGE_EXAMPLE = "blocking bill . gangsta1 gangsta2";
    // private const string USAGE_EXAMPLE = "blocking left bill gangsta gangsta2"; // old format

    public DialogueBlockingAction(string[] args) {
        // TODO --- implement this
        if (args.Length < 3) {
            throw new DialogueActionsException($"`blocking` requires at least 3 arguments. Usage: `{USAGE_EXAMPLE}`");
        }
        cmd = args[0];
        actors = new List<string>(args.Skip(2));
    }

    public void ResolveDialogue(DialogueController ctrl) {
        // ctrl.SetBlocking(actors);
        throw new NotImplementedException("blocking actions were removed.");
    }

}


public class DialogueEmoteAction : IDialogueAction {

    public bool wait_for_player_input { get => false; }
    
    
    public string cmd { get; private set; }
    public string actor_name { get; private set; }
    public string pose_name { get; private set; }

    public DialogueEmoteType emote { get; private set; }

    public const string USAGE_EXAMPLE = "emote bill angry; emote bill none;";

    public DialogueEmoteAction(string[] args) {
        if (args.Length != 3) {
            throw new DialogueActionsException($"invalid Emote action. USAGE: {USAGE_EXAMPLE}");
        }
        cmd = args[0];
        actor_name = args[1];
        pose_name = args[2];
        emote = DialogueEmote.StringToEmote(pose_name);
    }

    public void ResolveDialogue(DialogueController ctrl) {
        ctrl.UpdateEmote(actor_name, emote);
    }
}



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