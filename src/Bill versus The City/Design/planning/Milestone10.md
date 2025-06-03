
=====================
=== MILESTONE 010 ===
=====================

Started on 2025-05-31
Finished on 2025-06-??

PLANNED FEATURES
        
[~] Cutscene/character choreography
    [+] Choreography can move characters around
    [ ] Choreography can control the camera
    [+] Choreography steps can execute simultaenously
    [+] Choreography steps can compose multiple sequential steps
        - this combined with previous subtask will allow a nav-mesh character to walk to a destination while a manual 
            character takes several steps to walk a path simultaneously.
    [ ] FIX: manual character is running, instead of walking
    [+] REFACTOR: remove copypasta between Choreography and SequentialChoreographyStep. Choreography should just be implemented to dynamically use 
        it's own SequentialChoreographyStep, instead of re-implementing functionality.

    [ ] Choreography can flow into a dialogue
    [ ] Dialogue can flow into a Choreography
    [ ] Choreography works with live enemies, not just dummy NPCs

[ ] additional stages in a level (eg. defeat some enemies, triggering more to spawn)
    [ ] an arbitrary number of objectives and event triggers on completion should be easy to configure in a LevelConfig

[ ] improve dialoug UI
    [ ] Speaking character should be clear
    [ ] names of non-speaking characters should be clear (name should show directly under the character actually speaking)
    [ ] FIX: when running directly in a scene, opening dialogues fails due to a null pointer. The issue doesn't occur if the scene
        was loaded from a main-menu launch of the game.
         - the null pointer is caused because the dialogue's Start, which sets up references to VisualElements populated dynamically, is called  
            AFTER trying to populate those objects. I don't know why this happens out of order for directly loaded scenes only.
    ~~[ ] Add "Emotes" to character portraits~~ --> I think this will just be implemented

PLANNED BUGFIXES
[ ] 

UNPLANNED FEATURES
[ ] Add check if player already owns to purchasable guns
[ ] Add damage numbers on the shooting range
[ ] Add additional distances on the shooting range

UNPLANNED BUGFIXES
 - 
