
=====================
=== MILESTONE 016 ===
=====================

Planned on 2026-01-???
Started on 2026-01-???
Finished on 2026-??????

PLANNED FEATURES
 - writing?
 - bugs?
 - visuals?
 - combat?

PLANNED BUGFIXES


LOW PRIORITY TASKS
[ ] REFACTOR: extract code from CharCtrl that will never be used by enemies

UNPLANNED BUGFIXES
[ ] BUG: Police Timer appears in a broken state on levels without a police timer
[ ] BUG: crouch-dive doesn't change cancel aiming
[ ] BUG: crouch-dive and Jump don't cancel reload
[ ] BUG: FleeFromThreatsBehavior line 117 null pointer
    - I think this is caused by spawned enemies not having a reference to the player, but I haven't investigated
[ ] BUG: `ArgumentException: This VisualElement is not my child` when removing a Dialogue Portrait
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from a previous sprint; I couldn't figure out how to reproduce this (still can't)
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
