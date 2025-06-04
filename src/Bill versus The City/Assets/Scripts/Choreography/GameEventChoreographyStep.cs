


using UnityEngine;

public class GameEventChoreographyStep : AbstractChoreographyStep {

    public MonoBehaviour init_game_event;
    private IGameEventEffect game_event;

    protected override void Start() {
        base.Start();
        game_event = init_game_event.GetComponent<IGameEventEffect>();
        if (game_event == null) {
            Debug.LogError($"{this.GetType()} {gameObject.name} Cannot find IGameEventEffect component on {init_game_event.name}");
        }
    }

    public override void Activate(IChoreography choreography) {
        base.Activate(choreography);
        game_event.ActivateEffect();
        Complete();
    }
}
