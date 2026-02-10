
=====================
=== MILESTONE 016 ===
=====================

Planned on 2026-02-10
Started on 2026-02-11
Finished on 2026-??????

PLANNED FEATURES
[ ] Explore Unity Audio Mixer
[ ] Implement Grenade Enemies
[ ] Try to make Grocery Store level more fun
    [ ] FIX: enemies hanging back
    [ ] Add weapon veriety
[ ] OUTLINE: Explore more levely type concepts
    - room clear (standard)
        - interupt drug deal
        - clear gang house
    - wave survival 
        - survive gang attack
        - survive swat attack
    - flight/escape
[ ] play with particle effects

PLANNED BUGFIXES


LOW PRIORITY TASKS
[ ] REFACTOR: extract code from CharCtrl that will never be used by enemies
[ ] FIX: 2+ instances of AOE that overlap stack damage. This should not stack
    - to do this, I need to move the tracking of Damage-over-time onto the target, not the AOE. However, I already spent too much time on this feature that may not stick around.

UNPLANNED BUGFIXES
[ ] BUG: FleeFromThreatsBehavior line 117 null pointer
    - I think this is caused by spawned enemies not having a reference to the player, but I haven't investigated
[ ] BUG: `ArgumentException: This VisualElement is not my child` when removing a Dialogue Portrait
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from a previous sprint; I couldn't figure out how to reproduce this (still can't)
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
[ ] a bunch of empty game objects are spawned named: "Global Managers > GameSoundGizmos > New Game Object" 
[ ] BUG: if a grenade blows up in your hands, it does not use ammo.