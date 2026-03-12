
=====================
=== MILESTONE 016 ===
=====================

Planned on 2026-02-10
Started on 2026-02-11
Finished on 2026-??????

PLANNED FEATURES
[+] Explore Unity Audio Mixer
    - added a unity audio mixer, and changed AudioSettings to apply volumes through the AudioMixer
[+] Implement Grenade Enemies
[ ] Try to make Grocery Store level more fun
    [ ] FIX: enemies hanging back
    [ ] Add weapon veriety
[ ] OUTLINE: Explore more level type concepts
    - room clear (standard)
        - interupt drug deal
        - clear gang house
    - wave survival 
        - survive gang attack
        - survive swat attack
    - flight/escape
[+] play with particle effects

PLANNED BUGFIXES


LOW PRIORITY TASKS
[ ] REFACTOR: extract code from CharCtrl that will never be used by enemies
[ ] FIX: 2+ instances of AOE that overlap stack damage. This should not stack
    - to do this, I need to move the tracking of Damage-over-time onto the target, not the AOE. However, I already spent too much time on this feature that may not stick around.

UNPLANNED BUGFIXES
[X] BUG: Countdown UI doesn't work on wave survival level (countdown still triggers the mission victory conditions)
    --> cannot reproduce
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
[ ] BUG: if you pause while holding a grenade, you cannot drop/throw that grenade 
[ ] BUG: picking up a weapon while reloading breaks the reload UI (a frozen reload UI displays until a new reload is started)
[ ] BUG: ending a level with a pickup item seems to cause it to show up selected already when you open the weapon select menu
    (needs more testing. When I implemented the PMR-30 and used it as a pickup, it caused the conventional handgun and the PMR-30 to both 
    appear selected when I opened the gunstore scene.)
[ ] BUG: ~~Switching weapons at the gun store for the first time enables combat~~, and allows you to shoot while not on the gun range. Going
    on the range then leaving will remove this, and switching weapons again will not reenable combat.
     - actually, combat just doesn't start disabled like it should. Once you interact with the disable/enable zone, it works as intended.
[ ] Aiming magnum revolver gives you super-speed