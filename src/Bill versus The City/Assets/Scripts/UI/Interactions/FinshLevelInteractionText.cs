
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UIElements;

public class FinshLevelInteractionText : AbstractInteractionText
{
    public Interaction interaction;
    public override string GetText()
    {
        if (!interaction.interaction_enabled)
        {
            return "Complete objectives before leaving the level";
        }
        return "End level";
    }
}
