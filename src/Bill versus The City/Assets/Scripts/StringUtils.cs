
using System.Text;
using UnityEngine;

public static class StringUtils {
    public static readonly string[] FORBIDDEN_SAVE_CHARACTERS = new string[]{
        "<", ">", "/", "\\", "\n", "\t", "\r", ":", "*", "?", "\"", "|", ".", 
        "~", "!", "@", "#", "$", "%", "^", "&", "(", ")", "[", "]", "{", "}"
    };

    public static string StripSaveName(string base_str) => StripCharacters(base_str, FORBIDDEN_SAVE_CHARACTERS);

    public static string StripCharacters(string base_str, string[] forbidden_chars, string replacement = "") {
        string result_str = base_str;
        foreach (string s in forbidden_chars)
        {
            result_str = result_str.Replace(s, replacement);
        }
        if (! result_str.Equals(base_str)) { Debug.LogWarning($"string required sanitization: '{base_str}' --> '{result_str}'"); }
        return result_str;
    }
}