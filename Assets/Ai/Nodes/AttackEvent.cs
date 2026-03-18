using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AttackEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AttackEvent", message: "Attack Started/Finished", category: "Events", id: "4d36b2ccc195d536673624dc3b9047a3")]
public sealed partial class AttackEvent : EventChannel { }

