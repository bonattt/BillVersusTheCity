
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
            Debug.LogWarning("interaction disabled text"); // TODO --- remove debug
            return "Complete objectives before leaving the level";
        }
        Debug.LogWarning("interaction enabled text"); // TODO --- remove debug
        return "End level";
    }
}
