using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "ReusableAIBehavior", menuName = "ReusableAIBehavior")]
[System.Serializable]
public class ReusableAIBehavior : ScriptableObject {
    [SerializeReference] public AIBehavior savedBehavior;
}