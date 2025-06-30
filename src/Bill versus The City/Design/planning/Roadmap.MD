
=============================
========== Roadmap ==========
=============================

[+] Milestone 4: enemy behavior improvements
[+] Milestone 5 Add player and enemy placeholder-animations from Unity asset store.
[~] Milestone 6: Level config
    --> automate configs for setting up scripts for verious possible level features. (Win condition, victory dialogue, start dialogue, weapon selections)
    --> Allow levels to start with pre-set weapons instead of the weapon selection screen
    --> implement level music
    --> enemy ammo drops
    GOAL: make it easier to configure commonly variable elements of levels

[+] Milestone 7: improvements to gameplay
	- detection based on whether they're on-screen
	X fix cover ---> already did this, kinda
	- ???

[+] Milestone 8: Progression and Hub World(s) (Bill's House and Gunstore), Saving
    --> Save the player's current level
    --> save settings
    --> restore to saved level
    --> create player owned weapons, which can be purchased for money at the gun store.
    --> add pickups in levels for cash
    --> Semi-auto shotgun

[+] Milestone 9: (hearing system and melee enemies)
[+] Milestone 10: Advanced Level config
    - Cutscene/character choreography
    - improve dialoug UI
    - additional stages in a level (eg. defeat some enemies, triggering more to spawn)

[ ] Milestone 11: Mishmash (More Dialogue System)
    ~~- add "emote" system to generically apply emotes to any character~~
    - scale portraits to different screen resolutions
    - replace font size in "px" with font size in "%" for ALL UIs
    - explosive weapons

[ ] Milestone ???: More Levels
[ ] Milestone ???: More Dialogue

[ ] Milestone ???: Start making the game look better
    - find some simple ways to make the game look better
[ ] Milestone ???: Art assets
[ ] Milestone ???: Better tutorials




=============================
==== Unplanned features =====
=============================

[ ] Add remove illegal weapons dialogue to level config
[+] Add damage drop off to weapons 
[ ] Add armor effectiveness to weapons
[ ] Reduce boilerplate to adding new fields to settings!
    [ ] FIX: allow save files to assume defaults for missing fields in save file
    [ ] Allow fields to be added from a type-based config of some sort?
[ ] Speed-run dialogue mode (hold to click through fast)
[ ] Additional controls to advance dialogue (press-e)
[ ] Dialogue anti-buffering (block inputs for a short time after dialogue opens, to prevent playes skipping dialogue if it opens while they're shooting)

[?] "swarm intelligence" ememy AI
    - randomly offset destinations when enemy is traveling to waypoints
    - enemies should account for other enemy positons when setting destinations
    - maybe add additional raycasts to player, to make the AI seek more clean-shots to the player, rather than shots it can make with a 0 width raycast, but not a bullet with a small non-zero width.
    [ ] FIX: check if enemy is actually on screen before INITIATING hostility, but allow enemies to still shoot from further off screen if already hostile

[+] Covering Fire: just shoots a gun towards a specified point
[X] Move camera closer to ground while crouching (reduce vision radius)

[ ] Difficulty System
    [ ] override availible equipment
    [ ] override reload
    [ ] truck medkit limit
    [ ] truck armor plates limit
    [ ] medkit healing rate
    [ ] player damage rate
    [ ] enemy damage rate
    [ ] Enemy detection effectiveness
