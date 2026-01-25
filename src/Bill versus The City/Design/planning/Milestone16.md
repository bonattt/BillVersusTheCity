
=====================
=== MILESTONE 016 ===
=====================

Planned on 2026-01-17
Started on 2026-01-17
Finished on 2026-??????

PLANNED FEATURES
[ ] Explore Unity Audio Mixer
[ ] Explore more Grenade options
    [+] REFACTOR: extract explosion interface to support grenade variety
    [ ] Smoke Grenade???
    [~] Flashbang
        [+] AoE damage effect
        [+] AoE spawned by grenade
        [ ] FIX: Flashbang UI flashes white on editor mode play
        [+] Add: flame duration
        [ ] FIX: Flame spreads through walls
    [+] Incindiary Grenade
        [ ] FIX: particles don't scale correctly when spawned by grenade
    [ ] Grenade Enemies
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

UNPLANNED BUGFIXES
[ ] BUG: FleeFromThreatsBehavior line 117 null pointer
    - I think this is caused by spawned enemies not having a reference to the player, but I haven't investigated
[ ] BUG: `ArgumentException: This VisualElement is not my child` when removing a Dialogue Portrait
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from a previous sprint; I couldn't figure out how to reproduce this (still can't)
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
