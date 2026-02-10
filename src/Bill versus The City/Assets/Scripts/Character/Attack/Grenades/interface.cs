

using UnityEngine;

public interface IAreaEffectPartial
{
    /* represents a partial chunk of a full area effect */
    public float sub_area_radius { get; set; }
    public IAreaEffectRegion parent { get; set; }
}

public interface IAreaEffectRegion {
    /* represents an effect over a large area, which is implemented in a spread of many instances of IAreaEffectPartial */
    public float area_radius { get; set; }
    public float area_effect_duration { get; set; }
    public LayerMask blocks_propegation { get; set; }
    public void TargetInArea(IAttackTarget t, float at_time);
    public void AddChild(IAreaEffectPartial child);
    public void RemoveChild(IAreaEffectPartial child);
}

public enum AreaDamageScalingMode {
    transform,
}