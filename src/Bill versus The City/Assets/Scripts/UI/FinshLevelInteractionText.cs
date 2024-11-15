
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

public class FinshLevelInteractionText : AbstractInteractionText
{
    public override string GetText() {
        if (EnemiesManager.inst.remaining_enemies > 0) {
            return "Defeat all enemies to complete the level";
        }
        return "End level";
    }
}
