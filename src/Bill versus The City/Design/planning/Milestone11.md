
=====================
=== MILESTONE 011 ===
=====================

Planned on 2025-06-20
Started on 2025-06-20
Finished on 2025-???

PLANNED FEATURES
[+] make UI more resolution agnostic
    [+] ~~replace all "px" font sizes with "%" font sizes~~
    [X] ~~replace as much "px" size styles and margins as possible with "%"~~
    [+] add scaling to dialogue portraits 

[ ] Explosive Weapons
    [ ] Basic Explosion Damage
    [ ] Mine (explode on trigger)
    [ ] Rocket Launcher (exploding projectile)
    [ ] Grenade (exploding throwable)
        [ ] Thrown item lands near mouse
        [ ] Thrown item goes over cover, even when crouched
        [ ] Grenade explodes after 5 seconds
        [ ] Cook Grenades

[ ] Dialogue "emote" system
    [ ] add an emote command, separate from the "pose" system; adds an emote bubble to any portrait
[ ] debug/difficulty options to unlock weapons

PLANNED BUGFIXES
[ ] FIX: when running directly in a scene, opening dialogues fails due to a null pointer. The issue doesn't occur if the scene
    was loaded from a main-menu launch of the game.
        - the null pointer is caused because the dialogue's Start, which sets up references to VisualElements populated dynamically, is called  
        AFTER trying to populate those objects. I don't know why this happens out of order for directly loaded scenes only.
        - this may be a loading order issue, I stopped being able to reproduce it
        - UPDATE: this seems to occur sometimes when loading the level as a next level too... or maybe this is a different bug, I don't see a null pointer error when it happens
        - UPDATE: when the choreography is loaded in a next level, it IS triggering on level start, then the first step, (a dialogue step) is not being set as "active", while the choreography itself shows as active. No dialogue is opened, and the whole thing stalls.
        - UPDATE: my best guess at present is, something is going wrong with the sequential choreography step Choreography dynamically creates to handle it's steps, eg. Activate() is called before this variable is dyanamically created, causing a null reference, which blocks the choreoghy from getting actually started and it just freezes the game pre-choreography. This can't be quite what's happening, or I would have seen a null pointer in the logs, so I'm still kinda confused.
        - I'm unable to reproduce this anymore to make further progress on fixing it

UNPLANNED FEATURES

UNPLANNED BUGFIXES
[+] Level 1 allows the player to leave immediately, without clearing enemies first
[+] Fix Enemies can hear while combat disabled
[~] FIX: Speaker label at center of screen looks offcenter for many resolutions; mainly because character portraits center on the 
    right in their slot, instead of the center, and don't adjust for resolution
[+] FIX: Player plays the running animation while being moved by choreography, instead of walking.
    - the animation debugger shows the player is walking, not running, but the character is clearly running