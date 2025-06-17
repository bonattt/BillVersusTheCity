using System;
using System.Linq;

using UnityEngine;


public static class DialogueActionUtil {
    public static string StageDirectionToString(StageDirection dir) {
        return $"{dir}";
    }

    public static StageDirection StageDirectionFromString(string dir) {
        switch (dir.ToLower()) {
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

    public static string StagePositionToString(StagePosition pos) {
        return $"{pos}";
    }

    public static StagePosition StagePositionFromString(string pos) {
        switch (pos) {
            case "left": return StagePosition.far_left;
            case "far-left": return StagePosition.far_left;
            case "far_left": return StagePosition.far_left;

            case "center-left": return StagePosition.center_left;
            case "center_left": return StagePosition.center_left;

            case "center-right": return StagePosition.center_right;
            case "center_right": return StagePosition.center_right;

            case "right": return StagePosition.far_right;
            case "far_right": return StagePosition.far_right;
            case "far-right": return StagePosition.far_right;

            default:
                Debug.LogError($"string cannot be converted to stage position '{pos}'");
                return StagePosition.unspecified;
        }

    }
    public static StageDirection StagePositionOposite(StagePosition pos) {
        return StageDirectionOposite(StagePositionToSide(pos));
    }

    public static StageDirection StagePositionToSide(StagePosition pos) {
        switch (pos) {
            case StagePosition.far_left: return StageDirection.left;
            case StagePosition.center_left: return StageDirection.left; 
            case StagePosition.center_right: return StageDirection.right;
            case StagePosition.far_right: return StageDirection.right;
            case StagePosition.unspecified: return StageDirection.unspecified;

            default:
                throw new DialogueActionsException($"unhandled StagePosition {pos}");
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

    public static int GetStagePositionIndex(StagePosition position) {
        // switch (position) {
        //     case StagePosition.far_left: return 0;
        //     case StagePosition.center_left: return 1;
        //     case StagePosition.center_right: return 2;
        //     case StagePosition.far_right: return 3;
        //     default:
        //         Debug.LogError($"unhandled stage position '{position}'");
        //         return -1;
        // }
        return (int)position;
    }
    public static StagePosition IndexToStagePosition(int index) {
        switch (index) {
            case 0: return StagePosition.far_left;
            case 1: return StagePosition.center_left;
            case 2: return StagePosition.center_right;
            case 3: return StagePosition.far_right;
            default:
                Debug.LogError($"unhandled stage index '{index}'");
                return StagePosition.unspecified;
        }
    }
}

