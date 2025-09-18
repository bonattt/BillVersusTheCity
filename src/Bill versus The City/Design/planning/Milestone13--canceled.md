
=====================
=== MILESTONE 013 ===
=====================

Planned on 2025-08-15
Started on 2025-08-????
Finished on 2025-??????

PLANNED FEATURES

[ ] Implement more robust system for showing the keybindings for controls
[ ] implement hold E to skip dialogue
[ ] implement hold E to skip Choreography 
[ ] Implement AoE damage effects
[ ] Implement fire-grenades

PLAYTEST FEEDBACK
[~] Increase grenade explosion size shift, even if not actually sprinting 


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


UNPLANNED BUGFIXES
[ ] I was able to die, then X out of the death dialogue somehow to move around the level as a dead guy (on the grenades tutorial)
    Steps to reproduce:
     - I couldn't figure out how I did this
