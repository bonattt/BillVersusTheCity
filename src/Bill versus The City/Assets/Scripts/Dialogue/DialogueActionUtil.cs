using System;
using System.Linq;

using UnityEngine;


public static class DialogueActionUtil {
    public static string StageDirectionToString(StageDirection dir) {
        return $"{dir}";
    }

    public static StageDirection StageDirectionFromString(string dir) {
        switch(dir.ToLower()) {
            case "left":
                return StageDirection.left;
            
            case "right":
                return StageDirection.right;

            case "unspecified":
                return StageDirection.unspecified;

            default:
                Debug.LogError($"string cannot be converted to stage direction '{dir}'");
                return StageDirection.unspecified;
        }
    }

    public static StageDirection StageDirectionOposite(StageDirection dir) {
        // gets the opposite direction of the passed dirction. Eg. given right, return left
        switch (dir) {
            case StageDirection.left:
                return StageDirection.right;
            
            case StageDirection.right:
                return StageDirection.left;

            default:
                return StageDirection.unspecified;
        }
    }
}

