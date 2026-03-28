using System.Collections.Generic;

using UnityEngine;

public class CoverActivationRegistry
{
    private static CoverActivationRegistry _inst;
    public static CoverActivationRegistry inst {
        get {
            if (_inst == null) {
                _inst = new CoverActivationRegistry();
            }
            return _inst;
        }
    }

    private List<ICoverActivator> activators = new List<ICoverActivator>();

    public void AddActivator(ICoverActivator a) => activators.Add(a);
    public void RemoveActivator(ICoverActivator a) => activators.Remove(a);

    public IEnumerable<ICoverActivator> IterateAll() {
        foreach (ICoverActivator ca in activators) {
            yield return ca;
        }
    }

    public IEnumerable<ICoverActivator> IterateNearby(Vector3 position) {
        foreach (ICoverActivator ca in activators) {
            float distance = Vector3.Distance(position, ca.activate_cover_position);
            if (distance <= ca.activate_cover_range) {
                yield return ca;
            }
        }
    }
}