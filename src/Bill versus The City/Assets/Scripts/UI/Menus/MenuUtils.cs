using System;
using System.Collections.Generic;

using UnityEngine; 
using UnityEngine.UIElements;

public static class MenuUtils {

    public static DropdownField SetupEnumDropdown(Type EnumType) {
        return SetupEnumDropdown(new DropdownField(), EnumType);
    }
    public static DropdownField SetupEnumDropdown(DropdownField dropdown, Type EnumType) {
        dropdown.choices = new List<string>(Enum.GetNames(EnumType));
        return dropdown;
    }
}