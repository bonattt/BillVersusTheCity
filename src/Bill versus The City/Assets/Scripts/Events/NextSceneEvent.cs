using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneEvent : AbstractInteractionGameEvent
{

    public string deprication_warning = "`next_scene` variable is no longer used!?";
    public string next_scene = "Demo001--level01"; // TODO --- variable is depricated

    protected override void Effect() {
        LevelConfig.inst.CompleteLevel();
    }
}
