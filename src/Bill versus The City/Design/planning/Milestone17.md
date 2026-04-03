
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
     - carryover from a previous sprint; I couldn't figure out how to reproduce this (still can't (still still can't))
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
[+] a bunch of empty game objects are spawned named: "Global Managers > GameSoundGizmos > New Game Object" 
[+] BUG: if a grenade blows up in your hands, it does not use ammo.
[+] BUG: if you pause while holding a grenade, you cannot drop/throw that grenade 
[+] BUG: ending a level with a pickup item seems to cause it to show up selected already when you open the weapon select menu
    (needs more testing. When I implemented the PMR-30 and used it as a pickup, it caused the conventional handgun and the PMR-30 to both 
    appear selected when I opened the gunstore scene.)
    - I was wrong about the cause of this, the Glock and PMR30 guns had a duplicate item_id, so they both got selected.
[+] BUG: ~~Switching weapons at the gun store for the first time enables combat~~, and allows you to shoot while not on the gun range. Going
    on the range then leaving will remove this, and switching weapons again will not reenable combat.
     - actually, combat just doesn't start disabled like it should. Once you interact with the disable/enable zone, it works as intended.
     This was all WAY simpler than I thought; the level config had "combat enabled" set to true 🤦‍♂️
[+] Aiming magnum revolver gives you super-speed
[+] BUG: cover doesn't work
    - I think this was caused by the changes I made to make it possible for StandAndShootBehavior to shoot at something OTHER than the player. 
    - Enemies are shooting on a flat trajectory, even when the player crouches.
    - I am going to solve this by making cover detect when the player is nearby and crouching, and invisibly grow larger for the player. I 
      kinda hate this, but I think it will work.
[+] BUG: picking up a weapon while reloading breaks the reload UI (a frozen reload UI displays until a new reload is started)
    - NOTE: I saw several other methods of freezing the reload UI, but I didn't note the steps to reproduce.
    - I fixed the null-pointer below, which fixed this, but now instead, reload will carry over onto whatever weapon you picked up
[+] BUG: null pointer in ReloadSounds line 60 in cancel reload sound
    --> i suspect this is responsible for the busted reload UI when you cancel a reload
[+] BUG: level music volume (at least on the "test" level) scales only with "master" volume, but not with "music" volume
    - there was some additional weirdness where, the volume was not applied initially until I went into settings and hit "apply settings"
    - the above also effects sound effects for some reason. my best guess rn is that the volume from settings isn't applied to the AudioMixer on start up, and is only applied when I update in the settings menu. (also, music is playing on master instead of music, which is a seperate bug)
    --> after switching to audioMixers, I needed to explicitly apply volume settings to the AudioMixer on start up (previously, I polled volume level when a sound was played). The "PlayMusic" method circumvented the code that was setting the audioMixerGroup on sounds, so I needed to implement that. This was actually two unrelated bugs caused by the same change.
[+] BUG: New cover-system blocks throwing grenades while crouched.
    - this should be a fairly easy fix using physics layers.
[+] FIX: Actually, I still hate the invisibleCover system. idk why i didn't just change the collider height while crouched.
    [+] Implement this feature
    [+] Add distinction in attack code for whether to shoot flat in a direction, or actually shoot directly at the target.

[ ] BUG: Aiming while crouched increases movement speed
[ ] FIX: given the changes to cover, I have some ideas to fix diving;
    [ ] DON'T lockt the player into a crouch after a dive, and make the player stand up much faster to peak cover.
    [ ] Lock the player out of diving repeatedly
- BUG: last grenade cannot be thrown (Out of ammo error)
    - note, add an out of ammo click to throwing grenade without ammo
- BUG: if you grab a different weapon in the same slot while holding a grenade, the grenade cannot be thrown anymore.
    - same thing occurs if you switch to a different weapon slot
    - in both cases, if you switch back to the weapon, you can then throw the grenade.
[ ] REFACTOR: make a script for "HeldGrenade" which will persist, even if the current weapon is switched, and move
     controls for actually throwing the grenade into this class, instead of the current ThrownAttack class