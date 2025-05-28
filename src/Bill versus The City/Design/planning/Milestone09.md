
=====================
=== MILESTONE 008 ===
=====================

Started on 2025-05-18
Finished on 2025-0????

PLANNED FEATURES

[+] Add some alternate controls
    [+] e to advance dialogues
    [+] scroll to switch weapon slots
        
[+] Hearing System
    [+] Refactor bullet and gunshot effect to use hearing system
    [~] Polish Hearing System
        [+] Increase sound range, and the impact of walls.
        [?] Alerted Enemies should make a sound, alerting other guys at their location
        [X] Sounds should effect enemies off the camera less
        [+] Play a sound when an enemy notices you

[+] Melee enemy
    [+] Melee refactors
    [+] Melee attack works
        [+] Attacks hit and deal damage
        [+] FIX: attacks hit multiple times, but should only hit once
    [+] Melee attack effects
        [+] Basic
        [+] Refactor to not hardcode this

PLANNED BUGFIXES
[+] FIX: Enemies not alerted by bullets ---> fix via hearing system
    ~~[ ] alerts go out, but they don't seem to work on enemies near the edge of the screen~~
    ~~[ ] Add alerting effect to threat-detection (bullets passing by)~~

[+] FIX: Revisit default settings code. Difficulty should default to "normal" not to "custom"
    [+] Fixed defaults
    [+] I fixed defaults, but switching profiles needs to reload settings from the new save file
[ ] FIX: "Move with WASD" tutorial doesn't clear

UNPLANNED FEATURES
[X] Police Countdown level conditions --> next milestone
[X] Level Config should support events that introduce new enemies --> next milestone

UNPLANNED BUGFIXES
[ ] Somehow I got stuck in both an aim and crouch state together, and couldn't get out. I cleared crouch somehow, not sure how, but not aim.
    - unknown steps to reproduce. I was trying to get scroll-wheel weapon switching to work at the time.
[?] Somehow melee enemy triggered EnemyBehavior.ReloadCancelled, got a null pointer on weapon, (because it takes a firearm), and then froze
        indefinitely. I'm not sure how that was a result, or how it happened in the first place
    - unknown steps to reproduce
    - I think I resolved this but did not document it. I think this call was made when the enemy was killed.
[ ] Damage numbers displaying a large gray number and small red number when a small amount of armor is removed and the overflow health damage   
    kills the player. (Should be large red number and small gray number)
[+] on Tutorial 1, the enemy is saying "huh" at weird times
[+] FIX: enemies say "huh" when killed
[ ] Last minute: make melee enemies sprint at you

SOOSH FEEDBAC
[+] Soosh was having errors trying to name save files
[+] FIX: can aim while in non-combat mode
[+] change "exit game" in pause menu to "return to main menu"
[+] FIX: level 1 still says "defeat all enemies to leave" no matter what the 
[+] only one gun range works, and that was confusing
[+] soosh still doesn't like "~~evil~~[chaotic neutral] ~~velocity~~[momentum]"
- level 2 still too hard
