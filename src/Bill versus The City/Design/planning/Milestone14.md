
=====================
=== MILESTONE 014 ===
=====================

Planned on 2025-09-17
Started on 2025-09-17
Finished on 2025-??????

PLANNED FEATURES

[ ] fix combat
    [+] Fix Sound
        [+] disable sound (for now)
        [+] try to fix sound (maybe it not go through walls)
        [ ] FIX: enemies manager is counting enemies through walls that aren't even aware as "closest" for max engaged enemies
    [+] Add manual system for alerting groups of enemies when N-number of enemies are remaining/killed from a region-based group.
    [ ] Refactor: make EnemyManager utilize AbstractEnemyGroup
    [+] Add Enemy Zone gizmo color variable

PLANNED BUGFIXES
[ ] FIX: existing SoundEffects should subscribe to AudioSettings, and update their volume
    STEPS TO REPRODUCE:
        - equip grenades and click to hold a grenade: fuse sound shouls start playing
        - without releasing LMB, hit escape, and navigate to the audio settings menu
        - set master and/or SFX volume to 0
        - EXPECTED: sound should be muted
        - ACTUAL: sound continues to play with volume uneffected by change
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
[ ] FIX: when a grenade is present at the start of a level, the fuse sound plays during the dialogue. Once the dialogue finishes, the sound goes away!
    --> I noticed that, the fuse sound doesn't go away on level start, but it gets much quieter. I think this may have to do with 3D audio stuff


UNPLANNED FEATURES
[~] Add Vaulting over low-cover
    [+] Implement vaulting with sprint
    [+] Improve controls (don't use sprint)
    [+] Add Sound
    [+] Add Animation
    [ ] Allow vault while standing still
[ ] REFACTOR: to have CharCtrl movement actions (eg. sprint, crouch, roll, )

[ ] DEBUG OPTIONS
    [ ] Add debug infinite ammo flags
    [ ] Add open switch weapon menu
[ ] Implement next_shot_ready sound for pump shotguns

UNPLANNED BUGFIXES
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - I couldn't figure out how I did this
[+] Fix EXTREMELY loud footstep sound when player teleports with stairs.
