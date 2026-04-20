 
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
[+] implement enemies taking cover
    [ ] FIX: enemy behvior to take cover should be changed to actually use low cover that can be crouched under
    [ ] FIX: enemy perception should see through low cover, and try to shoot at cover. OR implement a bool for this.
[+] NPC civilian bystanders

PLANNED BUGFIXES (pre-existing bugs I planned to fix this sprint)
[ ] NullReferenceException: Object reference not set to an instance of an object --> whenev
        EnemiesManager.GetEnemyDistance () (at Assets/Scripts/Character/EnemyGroups/EnemiesManager.cs:90)

SPRINT BUGS (bugs found this sprint and fixed)
[?] Error closing game: menus are trying to close from the choreography, and getting an error because the menu isn't open...
    --> working towards fixing this, I added a cancel to Choreography, which shouldn't have fixed this by-itself because it still calls `MenuManager.inst.CloseMenu`
    however, with this change, I can no longer reproduce this error.


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
