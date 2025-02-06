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

[~] Scene Management
    [+] Can load into new scenes
        [+] Load new scenes when all enemies defeated
        [+] Add a Delay before level ends, or require a return to truck
    [~] Static resources transfer to new scene seemlessly 
        [+] Static resources transfer to new scene
        [+] REGRESSION BUG: restart-level breaks now
        [+] REGRESSION BUG: reload UI breaks on new-scene load
        [+] weapon toolbar doesn't update after new scene load
        [X] previously equipped weapon should be retained on a new scene load (won't do this sprint, maybe never)
    [+] Main Menu
    [~] Pass Dyamic player configuration to next level

[+] Equipment Select at the start of a level

[+] Equipment Pickups

[+] Dialogue system
    [+] Dialogue System
        [+] Loads portraits from dialogue file
        [+] Rearange portraits 
        [+] Step through dialogue text
        [+] Automatically set the speaker label
        [+] set pose on characters
        [X] Portraits gray out while not speaking ---> do later
        [X] Portraits on a side together can stack ---> good enough for now
        [+] Portraits can be ordered on a side
        [+] Use multiple portraits with the same art
        [+] Can pause during dialogue
        [+] Dialogue's can trigger events 
    [+] Portrait Manager
        [+] PortraitSystem class
        [+] Get test portraits

[ ] Fixes
    [+] Absent Save file is breaking the built game
    [+] Truck Asset has weird collisions
    [+] Audio Sources delete script, not game object
    [+] Exception finishing levels with a pickup weapon
    [ ] FPS display doesn't work in pause
    [ ] Aim-zoom janky
    [ ] click R again canceling reload is annoying
        - only have "shoot" cancel it, BUT aiming doesn't work while reloading
    [ ] shooting through wall still works occasionally, but it's harder
    [ ] Shooting wall alerts enemies on the other side
    [ ] Test un-pause attack blocking
    [ ] fences look breakable --> won't fix (game will look nothing like it looks now)

[ ] Bugs
    [ ] hovering on Interactions is not detected sometimes



===================
==== DEMO 004 =====
===================

[~] More interesting Enemy Behavior
    [+] varied passive behaviors
        [+] Patrol
        [+] Wonder
    [+] Less Omnipotent agression
        [+] Enemies do not always know exactly where the player is when aggressive
        [+] Enemies eventually lose track of the player
        [ ] Enemies will search out the player if they know the player is present on the map
    [X] Enemies can seek out an use tactical positions
    [+] Enemies can retreat to reload weapons
        [+] Enemies track ammo as they shoot
        [+] Enemies can reload when they run out
        [+] Enemies will reposition somewhere out of the way to reload when they do
        [+] UI indicates if an enemy is reloading
        [ ] Random chance for different reload behaviors
        [ ] Reload works with single-shot reload weapons
            [ ] Enemies can fully reload if not interupted
            [ ] Enemies will cancel reloading if they are interupted, and start shooting again after only partially reloading.
    [+] Retreat SubBehavior
        [+] Enemies find a semi optimal position to take cover from the player
        [+] Bugfix
            [+] Cover detection broken
            [+] Finding cover proceedurally is buggy --> replaced with waypoint system.
            [+] Enemies on a point count themselves as cover <--- you are here 
            [+] Enemies are too willing to run past the player to find cover
                -> replace distance from player in distance score
                -> use player direction and travel direction instead
        [X] Optimize

    [+] FIX: enemy state updates UIs should be destroyed if they still exist when a new update is usued
    [~] FIX: Enemies should avoid shooting each other (raycast to look for fellow enemies)
    [~] FIX: Enemies should avoid getting stuck in doorways
        - this and previous task aren't totally fixed, but they are improved by widening the NavMeshAgent.radius so enemies don't come too close to each other
    [+] FIX: interactions feel deceptive about which interaction actually triggers
    [+] FIX: weapon pickups can block each other
    [+] FIX: enemies need to get too close before they start shooting
    [X] FIX: the middle of the fence can be crawled over
    [+] FIX: can shoot through the wall while crouched
    [+] FIX: aiming line isn't blocked by the truck
    [+] FIX: aim line is short when mouse is near the player, maybe
    
    [+] FIX: can `escape` out of weapon select and death menus
    [+] FIX: Crouch dive prevents shooting without aim until you uncrouch and recrouch
    [~] FIX: fix pause on level-start weapon select
    [X] FIX: enemy awareness notification moves around the enemy when rotating
    [X] FIX: camera jitter when mousing over walls
    [?] FIX: Enemy reloads instantly sometimes --> cannot reproduce

[+] Tutorial Text
[+] Levels
    [+] Cover Level
	
===================
FEEDBACK: Soosh
	- Movement feels like it has momentum
	- Cover functionality is not clear
	- What is and is not cover is not clear
	- Level too hard
	- Aim line still follows mouse if you pause while aiming
	- HP < 0.5 shows in UI as HP 0, but does not kill you
	- sometimes enemies can see you before you see them
	- "the shotgun got nerfed"
	- red line goes through fence

===================
==== DEMO 005 =====
===================

[ ] Use placeholder character for player and enemy animations
    [+] animate walking
    [~] animate running
        [+] rotate the character to the move direction while sprinting
        [~] add a sprint animation so the character doesn't skate like shadow the hedgehog
    [X] animate weapon switching --> no animation for this
    [+] animate crouching
        [+] Crouching animates
        [+] figure out some animation for crouch dive
        [+] Fix crouch dive not working --> crouch dive speed is far too slow
    [X] animate crawling --> no animation for this
    [+] Animate shooting
        [+] Animate aiming handgun
        [+] Animate aiming rifle
    [~] Polish up Enemy
        [ ] implement aiming for enemies when in optimal combat range
        [+] Disable collider immediately on death
        [+] split clean up and delayed effects so enemy can drop pickup after animation plays
        [+] FIX: reload UI sticks around after enemy is killed
    [X] FIX: enemy detection not working with animations!! 
        ---> I had this happen on tutorial 1, and could not reproduce on level 1
        ---> not a bug, notice speed was turned really low on the enemy.
    [+] FIX: enemies sometimes continue to rotate after being killed
    [ ] REFACTOR: make animator read the character controller, instead of having the character controller write to the animator

[+] Fixes
    [+] Aim line appears underneath the street
    [+] Aim line too short when mouse close to player 
    [+] Better FPS display
	[+] 0 < HP < 1 shows in UI as HP 0, but does not kill you
	[+] Aim line still follows mouse if you pause while aiming
    [+] move aim line to the hight of "shoot_from" so it passes through cover when not crouched
    [X] FIX: crouch dive doesn't trigger if move right is held ---> hardware issue, not a bug
        ---> crouch input is false any time move right is held!?!?
        ---> Unity's Input.GetKey(KeyCode.space) fails here 
        ---> issue with my keyboard...
    [+] FIX: crouching without moving gets stuck in crouch
    [X] FIX: Animation siezes up if you START aiming, then start shooting before aiming finishes
        ---> not fixing this time around
    [ ] FIX: slow hipfire animation for rifles
    [ ] FIX: giant muzzleflash on hipfire animation...
    [+] FIX: enemy alertness UI shows up while enemy is dead
===================
FEEDBACK:
===================

===================
===== DEMO 06 =====
===================
    [~] Level Config system: allows easily configuring several options for level
        [+] TOGGLE: weapon select on level start
        [+] TOGGLE: combat enabled
        [+] setup starting weapons
        [+] setup victory conditions, which can be sequential
            [+] Survive for X time
            [+] Escape to truck
            [+] Defeat X number of enemies
        [+] Level Dialogue
            [+] Start Level Dialogue
            [+] Clear Objectives Dialogue
            [+] Finish Level Dialogue
        [+] REFACTOR: fail_level_conditions --> non-sequential conditions
        [ ] Level music
        [ ] FIX: update UI to communicate when the level doesn't require clearing all enemies
        [ ] FIX: countdown condition triggers immediately on level restart

    [ ] Implement a defense mission
        [ ] implement special behaviors
            - special behavior does something unique while the enemy hasn't yet spotted the player
            - once the player is spotted, special behavior is removed
            [ ] "Attack" behavior moves to an attack possition, and switches to patroling randomly or searching unless the player is found
            [ ] Covering Fire: just shoots a gun towards a specified point

    [+] FIX: player doesn't animate without weapon equipped
    [ ] FIX: sometimes, start weapons equip to the wrong slot, and start without properly copying and setting ammo to full...
    [ ] FIX: weapon select UI should display error if weapons not selected
        - error message in red
        - error noise 
        - if weapons are null, set them to the first weapons availible. Otherwise, set to previously equipped.
    [ ] Crouch while aiming locks into aim animation

=============================
========== Roadmap ==========
=============================

[+] Milestone 4: enemy behavior improvements
[ ] Milestone 5 Add player and enemy placeholder-animations from Unity asset store.
[ ] Milestone 6: Level config
    --> automate configs for setting up scripts for verious possible level features. (Win condition, victory dialogue, start dialogue, weapon selections)
    --> Allow levels to start with pre-set weapons instead of the weapon selection screen
    --> implement level music
    --> enemy ammo drops
    GOAL: make it easier to configure commonly variable elements of levels

[ ] Milestone 7: improvements to gameplay
	- detection based on whether they're on-screen
	X fix cover ---> already did this, kinda
	- ???


[ ] Milestone 8: Progression and Hub World(s) (Bill's House and Gunstore), Saving
    --> Save the player's current level
    --> save settings
    --> restore to saved level
    --> create player owned weapons, which can be purchased for money at the gun store.
    --> add pickups in levels for cash
    --> Semi-auto shotgun
    
[ ] Milestone ???: More Levels
[ ] Milestone ???: More Dialogue
[ ] Milestone ???: Art assets
[ ] Milestone ???: Better tutorials


=====================================
=== Known Bugs / Missing Features ===
=====================================
    [ ] FIX: slow hipfire animation for rifles
    [ ] FIX: giant muzzleflash on hipfire animation...
    [X] FIX: Animation siezes up if you START aiming, then start shooting before aiming finishes
    [ ] implement aiming for enemies when in optimal combat range



=============================
==== Unplanned features =====
=============================

[ ] Skip Level System
[ ] Speed-run dialogue mode (hold to click through fast)
[ ] Additional controls to advance dialogue (press-e)
[ ] Dialogue anti-buffering (block inputs for a short time after dialogue opens, to prevent playes skipping dialogue if it opens while they're shooting)

[ ] "swarm intelligence" ememy AI
    - randomly offset destinations when enemy is traveling to waypoints
    - enemies should account for other enemy positons when setting destinations
    - maybe add additional raycasts to player, to make the AI seek more clean-shots to the player, rather than shots it can make with a 0 width raycast, but not a bullet with a small non-zero width.
    [ ] FIX: check if enemy is actually on screen before INITIATING hostility, but allow enemies to still shoot from further off screen if already hostile

[ ] Move camera closer to ground while crouching (reduce vision radius)

[ ] Menus

[ ] Difficulty System
    [ ] override availible equipment
    [ ] override reload
    [ ] truck medkit limit
    [ ] truck armor plates limit
    [ ] medkit healing rate
    [ ] player damage rate
    [ ] enemy damage rate
    [ ] Enemy detection effectiveness