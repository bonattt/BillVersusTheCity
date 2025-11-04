
=====================
=== MILESTONE 014 ===
=====================

Planned on 2025-10-18
Started on 2025-10-28
Finished on 2025-??????

PLANNED FEATURES

[~] REFACTOR: to have CharCtrl movement actions (eg. sprint, crouch, roll, )
    [+] Initial refactor
    [ ] add aiming as a move action
    [ ] Factor crouch_percent and aim_percent into movement speed for actions
    [ ] extract code from CharCtrl that will never be used by enemies
[ ] Refactor: make EnemyManager utilize AbstractEnemyGroup
[ ] Implement next_shot_ready feedback sound for pump shotguns
[ ] Balance Grenade damage
    [ ] base damage on grenades should be deadlier
    [ ] maybe add more damage falloff

PLANNED BUGFIXES
[ ] FIX: enemies manager is counting enemies through walls that aren't even aware as "closest" for max engaged enemies
[ ] FIX: existing SoundEffects should subscribe to AudioSettings, and update their volume
    STEPS TO REPRODUCE:
        - equip grenades and click to hold a grenade: fuse sound shouls start playing
        - without releasing LMB, hit escape, and navigate to the audio settings menu
        - set master and/or SFX volume to 0
        - EXPECTED: sound should be muted
        - ACTUAL: sound continues to play with volume uneffected by change
[ ] FIX: when a grenade is present at the start of a level, the fuse sound plays during the dialogue. Once the dialogue finishes, the sound goes away!
    --> I noticed that, the fuse sound doesn't go away on level start, but it gets much quieter. I think this may have to do with 3D audio stuff
[ ] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways

UNPLANNED FEATURES


UNPLANNED BUGFIXES
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from previous sprint; I couldn't figure out how to reproduce this (still can't)
[ ] BUG: crouch-dive doesn't change cancel aiming
[ ] BUG: crouch-dive and Jump don't cancel reload