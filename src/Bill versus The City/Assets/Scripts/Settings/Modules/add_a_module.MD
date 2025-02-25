to add a new settings module in the settings menu, the following steps are required:

1) Create a new settings module script in `Assets/Settings/Modules/` which extends AbstractSettingsModule
1) Add the new settings module to the singleton GameSettings
    1) add a field/property to store the new settings module class.
    1) add read and write calls for the new module in `GameSettings.LoadFromJson` and `GameSettings.AsJson`
1) Create a new settings module menu controller in `Assets/UI/Menus/Settings/` which extends AbstractSettingsModuleMenu
1) Add the new settings module menu in SettingsMenuCtrl script:
    1) add a public VisualTreeAsset field for the sub-menu layout of the new settings module. Most likely, you'll use the same default UXML already used by other settings modules. Make sure to set this in the prefab for the settings menu!
    1) add a string const for the name of the new tab, and add the const to the `tabs_order` string array, to set where the new tab appears in the list.
    1) 
