========================
===== ITERATION 1 ======
========================


[ ] UI
    [+] Ammo Display
    [ ] Weapon Selection Toolbar
    [+] Healthbars

[ ] Combat
    [+] Attack effects
    [+] Weapon based attack stats
    [+] Ammo capacity and reloads
    [+] Health Depletion
    [+] Armor-based damage calculations
    [ ] Accuracy system

[ ] Effects
    [+] Gunshot and hit vfx
    [+] Gunshot and hit sfx
    [+] Reload sfx
    [+] Reload progress UI

    [ ] SFX system
        [ ] supports multiple volume sliders (sfx, music, voice, etc)
        [ ] supports deconflicting noices (20 gunshots )

[ ] Enemy Behavior
    [+] Don't shoot walls
    [+] detection modes
    [+] Hunt down the player
    [ ] 


[ ] BUGS
    [+] Fix bullet collsions


[+] Maps
    [+] Build a prototype map.

========================
==== DEMO 000 NOTES ====
========================

- Better enemy detection and coordination
- detection range
- don't move and shoot at the same time
- doors


================
==== Demo 1 ====
================

[+] Metric System

[~] Accuracy System
    [+] Accuracy based on angle, instead of postion
    [+] accuracy from weapon
    [X] accuracy from enemy
    [+] recoil decreases accuracy while shooting
    [+] Aiming increases accuracy

[+] Aiming
    [+] Aiming accuracy
    [+] Aiming Camera
    [+] Aiming is actually good!
    [X] Aiming UI 

[X] Difficulty System --> do in another milestone
    [ ]

[X] Enemy Detection  --> do in another milestone
    [ ] Vision Cone detection
    [ ] Vision Range
    [ ] Hearing
    [ ] Detection not instant

[X] Player Detection --> not sure I will do at all

[+] Weapons Mechanics
    [+] Ammo Pool
    [+] Shotgun
    [+] AR-15

[~] Weapons UI
    [+] Show equipable weapons
    [+] Show equipped weapon
    [+] Weapon Icons
    [ ] Weapon-based shooting and reload sounds

[ ] Truck
    [+] replenish ammo
    [ ] repair armor
    [ ] heal partial damage
    [+] limitted uses
    [+] prototype model

[+] Damage number UI

[+] Bugs
    [+] Fix reload UI meter

[+] Enemy Behavior
    [+] Don't crowd the player
    [-] Actually aim

=============================
    DEMO NOTES
SOOSH:
    - rooms too big
        - enemies very hard too see
    - can shoot through walls
    - alt is the alternative for 
    - zoom out for weapon zoom


    - trying to shoot with no ammo feels bad
        - make a click or auto-reload
    - cancel reload cancels on empty gun???



================
==== Demo 2 ====
================

[X] Enemies 
    [X] Enemies reload

[+] Settings system (GameSettings)
    [+] Observer pattern
    [+] Volume -> float 
    [X] graphics (do later, but must be supported) -> bool, floats, ints, enums
    [X] difficulty (do later) -> numbers, enums

[+] Menus
    [+] Exit Game
    [+] Restart Level
    [+] Settings Menu
        [+] Audio Settings UI
        [+] Gameplay Settings UI
        [X] Graphics Settings UI

[+] Menu Backends
    [+] MenuManager
        [+] Create and destroy menus from prefabs
        [+] create submenus
    [+] Settings Module menu helpers
    [+] Audio Settings
    [+] Difficulty Settings
    [+] Mouse Sensitivity
    [X] Graphics Settings (font size) --> do later
    

[+] Sounds
    [+] weapon specific sounds
        [+] Shoot
        [+] Reload
    [+] Shoot Empty Weapon Sound
    [+] Reload Cancel sound

[+] Fixes
    [+] Shooting through walls
    [+] Cancel Reload with Aim feels bad
    [+] Rapid clicks firing empty machineguns

[+] Interaction UI
    [+] Interactable objects show interaction on hover
    [~] REFACTOR: extract ammo-containier display to it's own UI document
    [+] show ammo-container contents on the interaction
    
[+] Weapons
    [~] Zoom out camera while aiming --> doesn't work great
    [+] Mouse sensitivity while aiming
    [X] Aiming Reticle --> do later

[+] Enemy perception
    [+] Enemies take a moment to notice to the player
    [+] Enemies notice the player if they take damage
    [~] Enemies take a moment to react before they can shoot
    [+] Enemies notice the player if shots are fired nearby


=============================
    DEMO NOTES
SOOSH:
    - fences look breakable
    - FPS display doesn't work in pause
    - Truck Asset has weird collisions
    - Aim-zoom janky
    - click R again canceling reload is annoying
        - only have "shoot" cancel it, BUT aiming doesn't work while reloading
    - shooting through wall still works occasionally, but it's harder


===================
==== DEMO 003 =====
===================

[ ] Aiming reticle
    [ ] Reticle appears as you aim
    [ ] hide mouse while unpaused in main gameplay

[~] Save and Load data
    [+] Save/Load Game settings
    [ ] Save/Load metrics

[ ] Scene Management
    [+] Can load into new scenes
        [+] Load new scenes when all enemies defeated
        [ ] Add a Delay before level ends, or require a return to truck
    [~] Static resources transfer to new scene seemlessly 
        [+] Static resources transfer to new scene
        [+] REGRESSION BUG: restart-level breaks now
        [+] REGRESSION BUG: reload UI breaks on new-scene load
        [+] weapon toolbar doesn't update after new scene load
        [X] previously equipped weapon should be retained on a new scene load (won't do this sprint, maybe never)
    [+] Main Menu
    [ ] Pass Dyamic player configuration to next level

[ ] Equipment Select at the start of a level

[ ] Equipment Pickups

[ ] Dialogue system

[ ] Fixes
    [ ] fences look breakable
    [ ] FPS display doesn't work in pause
    [+] Truck Asset has weird collisions
    [ ] Aim-zoom janky
    [ ] click R again canceling reload is annoying
        - only have "shoot" cancel it, BUT aiming doesn't work while reloading
    [ ] shooting through wall still works occasionally, but it's harder

[ ] Bugs
    [ ] hovering on Interactions is not detected sometimes

    

=============================
==== Unplanned features =====
=============================

[ ] Menus

[ ] Enemy Detection
    [ ] Hearing
    [ ] Vision Range
    [ ] Vision Direction


[ ] Difficulty System
    [ ] override availible equipment
    [ ] override reload
    [ ] truck medkit limit
    [ ] truck armor plates limit
    [ ] medkit healing rate
    [ ] player damage rate
    [ ] enemy damage rate
    [ ] Enemy detection effectiveness