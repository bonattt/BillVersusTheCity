 
=====================
=== MILESTONE 016 ===
=====================

Planned on 2026-04-14
Started on 2026-04-14
Finished on 2026-??????

PLANNED FEATURES
[ ] Finish story outline
[ ] Start writing
[~] implement trial levels with new level types
    [+] Drug-bust level
    [ ] Flight level
    - <stretch> NPC ally combatants
    - <stretch> gun-store level where NPCs fight on your side
[ ] Update Unity version
[~] implement enemies taking cover
    [+] feature (behavior and movement script support crouching)
    [ ] My cover-detection logic is way more busted than I realized, I need to fix this
        [+] Prevent enemies selecting un-traversable paths
        [ ] Implement distinction between hard and soft cover (based on whether a crouch is required to use cover)
        [ ] Refactor pathfinding utils to return Vector3's instead of transforms for cover-position calculations
    [ ] FIX: crouching doesn't change the enemy's colliders. Crouching enemies should not "activate" invisible cover, but their collider should shrink so bullets go over them. Enemies should never crouch except behind cover.

[+] NPC civilian bystanders

PLANNED BUGFIXES (pre-existing bugs I planned to fix this sprint)
[ ] NullReferenceException: Object reference not set to an instance of an object --> whenev
        EnemiesManager.GetEnemyDistance () (at Assets/Scripts/Character/EnemyGroups/EnemiesManager.cs:90)

SPRINT BUGS (bugs found this sprint and fixed)
[?] Error closing game: menus are trying to close from the choreography, and getting an error because the menu isn't open...
    --> working towards fixing this, I added a cancel to Choreography, which shouldn't have fixed this by-itself because it still calls `MenuManager.inst.CloseMenu`
    however, with this change, I can no longer reproduce this error.
[+] FIX: audio sources not cleaned up.
[ ] FIX: Player crouching sinks into the floor partially; this causes enemies to shoot down, and hit the floor.


LOW PRIORITY TASKS
[ ] REFACTOR: extract code from CharCtrl that will never be used by enemies
[ ] FIX: 2+ instances of AOE that overlap stack damage. This should not stack
    - to do this, I need to move the tracking of Damage-over-time onto the target, not the AOE. However, I already spent too much time on this feature that may not stick around.

UNPLANNED BUGFIXES (Known bugs I don't plan to fix yet)
[ ] BUG: Countdown UI doesn't work on wave survival level (countdown still triggers the mission victory conditions)
    --> cannot reproduce
[ ] BUG: FleeFromThreatsBehavior line 117 null pointer
    - I think this is caused by spawned enemies not having a reference to the player, but I haven't investigated
[ ] BUG: `ArgumentException: This VisualElement is not my child` when removing a Dialogue Portrait
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from a previous sprint; I couldn't figure out how to reproduce this (still can't (still still can't))
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
