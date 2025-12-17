
=====================================
=== Known Bugs / Missing Features ===
=====================================
[+] FIX: some HUD elements break if you exit to main menu and start the game again (requires large refactor)
[ ] FIX: sometimes, if a level with countdown failure condition is failed, the level will fail again immediately on restart. 
    --> The issue is now intermittent.
[ ] FIX: slow hipfire animation for rifles
[ ] FIX: giant muzzleflash on hipfire animation...
[X] FIX: Animation siezes up if you START aiming, then start shooting before aiming finishes
[ ] implement aiming for enemies when in optimal combat range
[ ] FIX: weapon select UI should display error if weapons not selected
    - error message in red
    - error noise 
    - if weapons are null, set them to the first weapons availible. Otherwise, set to previously equipped.

[ ] FIX: Crouch while aiming locks into aim animation
[ ] FIX: Errors when game is closed during dialogues
[ ] FIX: Errors on level start from healthBar UI
[ ] FIX: on level restart, pickup weapons remain visible in the weapon equip toolbar
[ ] FIX: searching enemies get stuck on terrain sometimes
[+] FIX: restarting a countdown level causes the "complete objective" dialogue to show up before the level restarts, and linger under the 
    weapon select menu
[ ] FIX: reload UI coloring doesn't work
[ ] FIX: if diagloue contains a `break` statement before any dialogue, lorem ipsum text will show
[ ] if sequential level condition to clear all enemies triggers an enemy spawn before another clear enemies condition, "Escape to your truck will show" until the first LevelCondition's triggers finish evaluating




[X] FIX: Player keeps sliding when move inupt is released near my new test-tutorial interaction
    ---> setting physics layer to interaction fixed this.
    ---> I'm leaving note of this, because soosh reported experienceing something like this, so I want to remember the steps to reproduce, incase is see it again.