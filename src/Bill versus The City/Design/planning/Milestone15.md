
=====================
=== MILESTONE 015 ===
=====================

Planned on 2025-10-18
Started on 2025-10-28 to 2025-11-05
Resumed on 2025-12-13...
Resumed on 2026-01-11...
Finished on 2025-??????

PLANNED FEATURES

[~] REFACTOR: to have CharCtrl movement actions (eg. sprint, crouch, roll, )
    [+] Initial refactor
    [+] add aiming as a move action
    [+] Factor crouch_percent and aim_percent into movement speed for actions
    [+] Make crouch speed blend as you stand up
    [X] extract code from CharCtrl that will never be used by enemies
    [+] REVIEW: Choreography movement seems to maybe be messed up by changes to ManualCharacterMovement
[X] Refactor: make EnemyManager utilize AbstractEnemyGroup
[+] Implement next_shot_ready feedback sound for pump shotguns
[+] Balance Grenade damage
    [~] base damage on grenades should be deadlier
    [~] maybe add more damage falloff

[~] NEW LEVEL
    [+] Event Triggers for level conditions
    [+] Configure 2 waves of enemies
    [+] Add dialogue and choreography for enemy waves
    [+] Add custom objective text for getting groceries
    [+] Stop Police timer once objective is complete
    [X] combat balance and level design

PLANNED BUGFIXES
[ ] FIX: enemies manager is counting enemies through walls that aren't even aware as "closest" for max engaged enemies
[~] FIX: existing SoundEffects should subscribe to AudioSettings, and update their volume
    STEPS TO REPRODUCE:
        - equip grenades and click to hold a grenade: fuse sound shouls start playing
        - without releasing LMB, hit escape, and navigate to the audio settings menu
        - set master and/or SFX volume to 0
        - EXPECTED: sound should be muted
        - ACTUAL: sound continues to play with volume uneffected by change
[~] FIX: when a grenade is present at the start of a level, the fuse sound plays during the dialogue. Once the dialogue finishes, the sound goes away!
    --> I noticed that, the fuse sound doesn't go away on level start, but it gets much quieter. I think this may have to do with 3D audio stuff
    --> the 3D audio only works on certain levels because I overrode the audio-listener to be on the player instead of the camera. I should apply this everywhere.
[X] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways

UNPLANNED FEATURES


UNPLANNED BUGFIXES
[ ] BUG: Police Timer appears in a broken state on levels without a police timer
[ ] BUG: crouch-dive doesn't change cancel aiming
[ ] BUG: crouch-dive and Jump don't cancel reload
[ ] BUG: FleeFromThreatsBehavior line 117 null pointer
    - I think this is caused by spawned enemies not having a reference to the player, but I haven't investigated
[ ] BUG: `ArgumentException: This VisualElement is not my child` when removing a Dialogue Portrait
[+] BUG: police timer counts down during choreography
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - carryover from previous sprint; I couldn't figure out how to reproduce this (still can't)