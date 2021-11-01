using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "NewCharacterStats",order =0)]
public class CharacterStats : ScriptableObject
{
    
    public float xPower = 5;
    public float yPower = -2;
    public float zPower = 2;
    public InputHelpers.Button fingerButton = InputHelpers.Button.Trigger;
    public InputHelpers.Button gripBtton= InputHelpers.Button.Grip;
}
