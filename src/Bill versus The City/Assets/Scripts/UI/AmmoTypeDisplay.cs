using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class AmmoTypeDisplay {
    private static readonly Dictionary<AmmoType, string> display_values = new Dictionary<AmmoType, string>{
        {AmmoType.handgun, "handgun"},
        {AmmoType.magnum, "magnum"},
        {AmmoType.rifle, "rifle"},
        {AmmoType.shotgun, "shotgun"}
    };

    public static string DisplayValue(AmmoType type) {
        if (display_values.ContainsKey(type)) {
            return display_values[type];
        }
        Debug.LogWarning($"no configured display value for AmmoType.{type}");
        return $"{type}";
    }
}