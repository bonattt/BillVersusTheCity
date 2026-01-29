

// using System.Collections.Generic;
// using UnityEngine;

// public interface ITeleportEventListener {
//     public void OnTeleport(ITeleportEvent tp_event);
// }

// public interface ITeleportEvent {
//     public CharCtrl character { get; }
//     public Vector3 start_point { get; }
//     public Vector3 end_point { get; }
//     public string message { get; }
// }

// public class TeleportationEventManager
// {
//     private static TeleportationEventManager _inst; 
//     public static TeleportationEventManager inst {
//         get {
//             if (_inst == null) {
//                 _inst = new TeleportationEventManager();
//             }
//             return _inst;
//         }
//     }

//     private List<ITeleportEventListener> subscribe_all = new List<ITeleportEventListener>();

//     public void SubscribeAll(ITeleportEventListener new_sub) => subscribe_all.Add(new_sub);
//     public void UnsubscribeAll(ITeleportEventListener sub) => subscribe_all.Remove(sub);

//     public void OnTeleport(ITeleportEvent tp_event) {
//         foreach (ITeleportEventListener sub in subscribe_all) {
//             sub.OnTeleport(tp_event);
//         }
//     }

// }
