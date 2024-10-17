using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class DialogueFile {
    // class for reading a single dialogue file
    // dialogue files parse a text file into dialogue actions, which can be new dialogue lines of new blocking (characters entering, leaving, or changing poses)

    public const string SAVE_FILE_DIR = ".game_data\\dialogue";
    public string file_name { get; private set; }
    public LanguageSetting language { get; private set; }

    private List<string> lines = null;
    private List<IDialogueAction> actions;

    public string filepath {
        get {
            // TODO --- if save_name is ever user-generated, this will need to be sanitized
            return Path.Join(Application.dataPath, $"{SAVE_FILE_DIR}\\{file_name}");
        }
    }

    public DialogueFile(string file_name) : this (file_name, LanguageSetting.english) { /* do nothing */ }

    public DialogueFile(string file_name, LanguageSetting language) {
        this.file_name = file_name;
        this.language = language;
    }

    public void ParseFile() {
        // parses a dialogue file into dialogue actions

        string file_text = File.ReadAllText(filepath);
        ParseLinesFromData(file_text);  // get lines from raw file data
        ParseActions(); // get dialogue actions from file
    }

    public void ParseLinesFromData(string data) {
        // takes the contents of a dialogue file, and parses it into lines

        lines = new List<string>();
        char[] delimiters = new char[] { '\n', ';' };
        
        // Split the string and remove empty entries
        string[] split_result = data.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        lines = new List<string>();

        // only add lines containing something other than whitespace
        for (int i = 0; i < split_result.Length; i++) {
            string s = split_result[i].Trim();
            if (! s.Equals("")) {
                lines.Add(s);
            }

        }
    }

    public void ParseActions() {
        // parses the dialogue actions from a line of text
        actions = new List<IDialogueAction>();
        for (int i = 0; i < lines.Count; i++) {
            // regular for-loop to gaurantee execution order
            IDialogueAction new_action = GetActionFromLine(lines[i]);
            if (new_action == null) {
                throw new DialogueFileException($"error on line {i+1} of dialogue file '{filepath}' with command '{lines[i]}'");
            }
            actions.Add(new_action);
        }
    }

    public static IDialogueAction GetActionFromLine(string action_line) {
        string[] split_result = action_line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (split_result.Length == 0) {
            throw new DialogueFileException($"invalid dialogue line '{action_line}'");
        }
        string cmd = split_result[0];
        switch(cmd) {
            case "noop":
                return new DialogueNoOp();

            case "break":
                // stop for player input
                return new DialogueNoOp(true);

            case "say":
                return new DialogueSpeach(split_result);

            case "enter":
                return new DialogueEnter(split_result);

            case "blocking":
                return new DialogueBlocking(split_result);

            default:
                return null;
        }
    }

    public List<string> GetLines() {
        // returns a copy of the lines of text
        return new List<string>(lines);
    }


    private int _index = 0;

    public void ResetIterator() {
        _index = 0;
    }

    public IDialogueAction GetNextAction() {
        if (_index >= actions.Count) {
            return null;
        }
        return actions[_index++];
    }
}


[System.Serializable]
public class DialogueFileException : System.Exception
{
    public DialogueFileException() { }
    public DialogueFileException(string message) : base(message) { }
    // public DialogueFileException(string message, System.Exception inner) : base(message, inner) { }
    // protected DialogueFileException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}


public enum LanguageSetting {
    english
}