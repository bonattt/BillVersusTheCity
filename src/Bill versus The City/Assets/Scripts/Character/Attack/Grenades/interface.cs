

using UnityEngine;

public interface IAreaEffect
{
    public float area_radius { get; set; }
    public float area_effect_duration { get; set; }
    public LayerMask blocks_propegation { get; set; }
}
