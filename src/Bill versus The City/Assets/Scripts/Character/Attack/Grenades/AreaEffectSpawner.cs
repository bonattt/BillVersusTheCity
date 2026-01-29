using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AreaEffectSpawner : MonoBehaviour, IAreaEffect
{
    [Tooltip("area effect actually spawned by the area spawner")]
    public GameObject effect_prefab;
    public bool spawn_on_start = false;
    public AreaEffectSpawner root { get; protected set; }

    public bool is_root => root == null || root == this;

    [Tooltip("Total radius of the final area propegation.")]
    [SerializeField] private float _area_radius;
    public float area_radius { 
        get => _area_radius;
        set
        {
            _area_radius = value;
        }
    }

    [Tooltip("Radius of an individual area chunk.")]
    public float chunk_radius = 0.5f; 
    public float chunk_distance => chunk_radius * 0.9f; // slightly smaller than area_radius to avoid gaps in the area coverage

    [Tooltip("Duration in seconds applied to area effect(s) spawned.")]
    [SerializeField] private float _area_effect_duration = 8f;
    public float area_effect_duration { // TODO --- the interface property for this was removed, this can be simplified
        get => _area_effect_duration;
        set {
            _area_effect_duration = value;
        }
    }

    public float raycast_height => 0.5f;
    public LayerMask blocks_propegation { get; set; }

    protected List<AreaEffectSpawner> already_spawned = new List<AreaEffectSpawner>();

    protected GameObject spawned_effect = null;

    // Start is called before the first frame update
    void Start()
    {
        if (spawn_on_start)
        {
            if (root == null || root == this) {
                SpawnAsRoot();
            } else {
                Vector3 direction = (transform.position - this.root.transform.position).normalized; // direction from root to this as a unit-vector
                SpawnAsBranch(direction);
            }
        }
    }

    void LateUpdate() {
        // self destruct after the first frame. 
        Destroy(gameObject);
    }

    // NOTE: 
    // root = Central AreaEffectSpawner
    // branch = AreaEffectSpawner that is not the root
    // leaf = IAreaEffect spawned by a branch
    public void SpawnAsRoot()
    {
        spawned_effect = SpawnLeafEffect();
        // spawns as the center of a spawn propigation.
        this.root = this;
        foreach (Vector3 spawn_direction in GetSpawnDirections()) {
            SpawnBranchInDirection(spawn_direction);
        }
    }

    private bool SpawnAsBranch(Vector3 direction)
    {
        /* spawns as a node in the spawning */
        if (Vector3.Distance(transform.position, root.transform.position) > area_radius) {
            // outside the radius of effect, so do nothing (Recursive base-case)
            return false;
        }
        AreaEffectSpawner other = EffectExistsWithinDistance(chunk_distance / 2f);
        if (other != null) {
            return false; // an area was already spawned too close to this one.
        }
        if (ExpansionBlocked()) {
            // wall blocks expansion
            return false;
        }

        spawned_effect = SpawnLeafEffect();
        spawned_effect.transform.parent = root.spawned_effect.transform;
        foreach(Vector3 spawn_direction in GetSpawnDirections(direction)) {
            SpawnBranchInDirection(spawn_direction);
        }
        return true;
    }

    private bool ExpansionBlocked() {
        Vector3 start = new Vector3(transform.position.x, raycast_height, transform.position.z);
        Vector3 end = new Vector3(root.transform.position.x, raycast_height, root.transform.position.z);
        Vector3 direction = end - start;
        RaycastHit hit;
        if (Physics.Raycast(start, direction.normalized, out hit, direction.magnitude, blocks_propegation)) {
            return true;
        }
        return false;
    }

    private AreaEffectSpawner EffectExistsWithinDistance(float threshold) {
        foreach(AreaEffectSpawner other in this.root.already_spawned) {
            float distance_to = Vector3.Distance(transform.position, other.transform.position);
            if (distance_to < threshold) { return other; }
        }
        return null;
    }

    private IEnumerable<Vector3> GetSpawnDirections() {
        return new Vector3[] {
            new Vector3(1f, 0, 0).normalized,    // north
            new Vector3(1f, 0, 1f).normalized,   // north east
            new Vector3(0, 0, 1f).normalized,    // east 
            new Vector3(-1f, 0, 1f).normalized,  // south east
            new Vector3(-1f, 0, 0).normalized,   // south
            new Vector3(-1f, 0, -1f).normalized, // south west
            new Vector3(0, 0, -1f).normalized,   // west
            new Vector3(1f, 0, -1f).normalized,  // north west
        };
    }

    private IEnumerable<Vector3> GetSpawnDirections(Vector3 start_direction) {
        start_direction = start_direction.normalized;
        float base_angle_degrees = 45f;
        return new Vector3[] {
            // start_direction,
            // Quaternion.Euler(0f, 45f, 0f) * start_direction,  // rotated 45 degrees from start_direction
            // Quaternion.Euler(0f, -45f, 0f) * start_direction, // rotated -45 degrees from start_direction
            Quaternion.Euler(0f, base_angle_degrees/2f, 0f) * start_direction, 
            Quaternion.Euler(0f, -base_angle_degrees/2f, 0f) * start_direction,
        };
    }


    private AreaEffectSpawner SpawnBranchInDirection(Vector3 direction)
    {
        // spawns a new AreaEffectSpawner at the given position.
        GameObject clone = Instantiate(gameObject);
        AreaEffectSpawner area_spawner = clone.GetComponent<AreaEffectSpawner>();
        area_spawner.spawn_on_start = false;
        // clone_area.root = this.root;
        clone.transform.position = transform.position + direction;
        area_spawner.root = this.root;
        area_spawner.blocks_propegation = blocks_propegation;
        area_spawner.SpawnAsBranch(direction);
        this.root.already_spawned.Add(area_spawner);
        Debug.DrawLine(transform.position, area_spawner.transform.position, Color.black, 1f);
        Debug.DrawLine(transform.position, root.transform.position, Color.green, 1f);
        return area_spawner;
    }

    private GameObject SpawnLeafEffect() {
        GameObject leaf = Instantiate(effect_prefab);
        leaf.transform.position = transform.position;
        leaf.transform.rotation = transform.rotation;
        IAreaEffect area_effect = leaf.GetComponent<IAreaEffect>();
        area_effect.area_radius = this.chunk_radius;
        area_effect.area_effect_duration = area_effect_duration;
        return leaf;
    }

}
