
using UnityEngine;

public interface ICoverActivator {
    public bool is_taking_cover { get; }
    public float activate_cover_range { get; }
    public Vector3 activate_cover_position { get; }
}

public interface ICover {
    public bool is_cover_active { get; }   
}
