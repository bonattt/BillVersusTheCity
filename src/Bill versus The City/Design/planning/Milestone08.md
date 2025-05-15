
=====================
=== MILESTONE 008 ===
=====================

Started on 2025-04-21
Finished on 2025-??

[+] Collect money, and it's preserved between levels
    [+] Refactor item-pickup drops
    [+] Save player's money when a level is finished
    [+] Money resets on level restart

[+] Contiune
    [+] save current level and continue from main menu

[+] REFACTOR: make the HUD a scene-scoped game object
    [+] Add old HUD elements
    [+] Add money-display to HUD
    [+] Move dynamic victory condition HUDs to main HUD UXML
    [+] Add countdown HUD to main hud
    [+] add non-combat HUD
[+] FIX: HUD breaks on retry-level

[+] Implement addative scene-loading in level_config
    [+] figure out how to place level-specific assets in a level using additive scene content. (Implement additive scene guideline prefabs)

[+] HUB world
    [+] Bill's house, with NPC you can talk to.
        [+] map is reusable between "levels" 
        [+] Dialogue changes between levels
        [+] "Next level" interaction changes between levels
    [+] Gunstore
        [+] Has usable shooting range
        [+] Can buy new guns, which are added to your usable weapons

[+] FIX: settings not loaded from save properly
[X] FIX: loading into non-combat scene doesn't disable combat section of HUD 
    - Cannot reporduce/already fixed
[+] FIX: player cannot move while aiming without a weapon
[+] FIX: HUD loads inconsistently
[+] FIX: save progress doesn't save purchased guns
[ ] FIX: Enemies not alerted by bullets
[+] FIX: reload mouse/character follow only works in 1920x1080 screen resolution (it's hacked to work with a hard-coded offset there)

