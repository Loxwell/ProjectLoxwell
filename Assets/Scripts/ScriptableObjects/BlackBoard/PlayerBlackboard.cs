using LSG;
using LSG.LWBehaviorTree;
using System;
using UnityEngine;
using static LSG.Utilities.BitField;

public class PlayerBlackboard : ScriptableObject, IBlackboard
{
    [SerializeField]
    public Transform player;
}