using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteractionText : AbstractInteractionText
{
    public string text;
    public override string GetText() {
        return text;
    }

}
