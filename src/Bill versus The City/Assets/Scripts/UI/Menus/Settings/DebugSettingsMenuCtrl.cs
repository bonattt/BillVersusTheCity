using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class DebugSettingsMenuCtrl : AbstractSettingsModuleMenu {

    private Toggle show_fps_toggle, show_damage_toggle, debug_mode_toggle, player_invincibility_toggle,
            player_invisible_toggle, allow_debug_actions_toggle, unlock_all_weapons_toggle, unrestrict_weapon_slots_toggle;
    private DropdownField show_grenade_fuse_dropdown;

    public DebugSettingsMenuCtrl() {
        // do nothing
    }

    // return the SettingsModule this controller targets
    public override ISettingsModule settings_module { get { return GameSettings.inst.audio_settings; } }

    // takes the root element of the sub-menu, and configures the menu's controller
    public override void Initialize(VisualElement root) {
        this.LoadTemplate(root);
        header_label.text = "Debug Settings";

        VisualElement debug_mode_div = AddToggle("Debug Mode");
        debug_mode_toggle = debug_mode_div.Q<Toggle>();

        VisualElement show_fps_div = AddToggle("Show FPS");
        show_fps_toggle = show_fps_div.Q<Toggle>();

        VisualElement show_damage_div = AddToggle("Show Damage Numbers");
        show_damage_toggle = show_damage_div.Q<Toggle>();

        VisualElement player_invincibility_div = AddToggle("Player Invincibility");
        player_invincibility_toggle = player_invincibility_div.Q<Toggle>();

        VisualElement player_invisible_div = AddToggle("Player Invisible");
        player_invisible_toggle = player_invisible_div.Q<Toggle>();

        VisualElement allow_debug_actions_div = AddToggle("Allow Debug Actions");
        allow_debug_actions_toggle = allow_debug_actions_div.Q<Toggle>();

        VisualElement unlock_all_weapons_div = AddToggle("Unlock All Weapons");
        unlock_all_weapons_toggle = unlock_all_weapons_div.Q<Toggle>();

        VisualElement unrestrict_weapon_slots_toggle_div = AddToggle("Un-restrict weapon slots");
        unrestrict_weapon_slots_toggle = unrestrict_weapon_slots_toggle_div.Q<Toggle>();

        show_grenade_fuse_dropdown = MenuUtils.SetupEnumDropdown(typeof(ShowGrenadeFuse));
        show_grenade_fuse_dropdown.AddToClassList(SETTINGS_ITEM_CLASS);
        settings_pannel.Add(show_grenade_fuse_dropdown);


        LoadSettings();
    }

    private VisualElement AddToggle(string display_text) {
        // adds a div containing a toggle control, and returns the top-level VisualElement

        // TODO --- refactor: move this somewhere more reusable 
        VisualElement div = new VisualElement();
        div.style.flexDirection = FlexDirection.Row;
        div.AddToClassList(SETTINGS_ITEM_CLASS);
        settings_pannel.Add(div);

        Label toggle_label = new Label();
        toggle_label.text = display_text;
        div.Add(toggle_label);

        Toggle toggle = new Toggle();
        div.Add(toggle);

        return div;
    }

    // private (Slider, Label) AddVolumeSlider(string slider_label) {
    //     VisualElement slider_element = SettingsMenuUtil.CreatePercentSlider(slider_label);
    //     settings_pannel.Add(slider_element);
    //     return (slider_element.Q<Slider>(), slider_element.Q<Label>(SettingsMenuUtil.SLIDER_VALUE_LABEL));
    // }

    public override void SaveSettings() {
        // Saves the menu's changes to settings    
        DebugSettings settings = GameSettings.inst.debug_settings;

        settings.show_fps = show_fps_toggle.value;
        settings.debug_mode = debug_mode_toggle.value;
        settings.show_damage_numbers = show_damage_toggle.value;
        settings.player_invincibility = player_invincibility_toggle.value;
        settings.player_invisible = player_invisible_toggle.value;
        settings.allow_debug_actions = allow_debug_actions_toggle.value;
        settings.unlock_all_weapons = unlock_all_weapons_toggle.value;
        settings.unrestrict_weapon_slots = unrestrict_weapon_slots_toggle.value;
        settings.show_grenade_fuse = DebugSettings.ShowGrenadeEnumFromString(show_grenade_fuse_dropdown.value);
    }


    public override void LoadSettings() {
        // sets the UI's elements to match what is stored in settings (reverting any changes)
        DebugSettings settings = GameSettings.inst.debug_settings;

        show_fps_toggle.value = settings.show_fps;
        debug_mode_toggle.value = settings.debug_mode;
        show_damage_toggle.value = settings.show_damage_numbers;
        player_invincibility_toggle.value = settings.player_invincibility;
        player_invisible_toggle.value = settings.player_invisible;
        allow_debug_actions_toggle.value = settings.allow_debug_actions;
        unlock_all_weapons_toggle.value = settings.unlock_all_weapons;
        unrestrict_weapon_slots_toggle.value = settings.unrestrict_weapon_slots;
        show_grenade_fuse_dropdown.value = DebugSettings.ShowGrenadeStringFromEnum(settings.show_grenade_fuse);
        UpdateUI();
    }


    public override bool HasUnsavedChanges() {
        DebugSettings settings = GameSettings.inst.debug_settings;
        return show_fps_toggle.value != settings.show_fps || debug_mode_toggle.value != settings.debug_mode || show_damage_toggle.value != settings.show_damage_numbers;
    }



}