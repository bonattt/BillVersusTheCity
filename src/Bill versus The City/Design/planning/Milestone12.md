
=====================
=== MILESTONE 011 ===
=====================

Planned on 2025-07-03
Started on 2025-07-04
Finished on 2025-0???


[+] Update tutorial
    [+] Add Tutorials in grenade tutorial
    [+] remove redundant tutorials from Tutorial 3

PLANNED FEATURES
[+] Dialogue "emote" system
    [+] add an emote command, separate from the "pose" system; adds an emote bubble to any portrait
    [+] FIX: remove old character labels
    [+] Add animations to emotes

[+] debug/difficulty options to unlock weapons


PLAYTEST FEEDBACK
[+] FIX: throwing too close to fence bounces back and hits player
[+] FIX: menu has no back button
[~] Increase grenade explosion size
[+] FIX: Pickup item UIs block too much vision
[+] FIX: auto-equip new weapon if current slot is empty
[+] FIX: crouch dive controls improvements
    [+] Add a stuck crouched period after the crouch dive
    [+] Crouch dive from moving, not sprinting
    [+] FIX: crouch dive in place
        steps to reproduce: sprint in a direction on my keyboard with input limitations. Hold D, Shift, and Space then Release D.
        This causes the D input to be removed exactly the frame the space is added, and the character will roll in place, 
    
[+] FIX: cannot shoot while holding shift, even if not actually sprinting 


PLANNED BUGFIXES
[~] FIX: Level 3 (countdown level) has a second countdown UI without numbers, hidden behind the healthbars
    (also, dev room has this too)

UNPLANNED FEATURES
[+] add UI support for custom difficulty
    [+] implement UI
    [+] fix sliders to look like audio-settings sliders
[+] add more custom difficulty settings
    [+] enemy reload speed
    [+] enemy run speed

[+] Add fuse "hiss" sound to cooking grenades
    - use this sound https://freesound.org/people/maximumplay3r/sounds/713344/
[+] Add a "Get money" debug action
[~] Add "default" settings values
    [+] Refactor settings to share more code around Dictionary<string,type> implementations,
        [+] FIX: non-custom difficulty triggeres "unsaved changes" popup
    [+] Implement abstract min/max settings
    [+] Implement abstract default settings
    [X] expand scope of templating to apply to all settings
    [+] Implement graphics settings
[ ] Add doors

UNPLANNED BUGFIXES
[+] FIX: when tutorial not marked as "do not show", it continues to re-play on the same level multiple times
[+] Buyable guns don't show up if you have "unlock all guns" checked
[+] FIX: buying a weapon, then restarting the level refunds the money spent, but keeps the weapon. 
[+] Audio settings don't apply (immediately)
    - steps to reporduce
        - Play in build 011.5, change music or master volume in a level with music
        - changes seemed to be applied later
        - UPDATE: bug only effects music that is already playing
[+] FIX: Player healthbar extends too far when Enemy health buffed in difficulty
    - setps to reproduce: set enemy health to 300%, save, and restart the game
    - this is how I thought I produced the error, but I have been unable to reproduce it now
    - NOTE: error only occurs with DebugSettings.player_invincibility = true, and SOME settings of enemy/player health
    SOLVED: player invulnerability was preventing the player's health from being "damaged" to adjust for difficulty, so the health was ending
        up above maximum value
[+] FIX: when tutorial header and tutorial name are missmatched, tutorial fails to save as "do not show"
[X] FIX: Truck's "Finish Level" interaction header text is not centered!
    --> do this later, the interaction UI needs to be replaced anyways
[+] Purchases aren't saved unless you move to next level, so if you buy, restart, quit, it's not saved.
[+] FIX: null pointer from healthbar
[>] FIX: when a grenade is present at the start of a level, the fuse sound plays during the dialogue. Once the dialogue finishes, the sound goes away!
    --> do later
[+] FIX: cover blocks grenade damage, even if you don't crouch!!
[ ] FIX: Null reference in FleeFromThreatsBehavior
    steps to reproduce: throw a grenade at an enemy
    `Transform dest = WaypointSystem.inst.GetClosestCoverPosition(start_pos, cover_from);` in FleeFromThreatsBehavior at line 116
