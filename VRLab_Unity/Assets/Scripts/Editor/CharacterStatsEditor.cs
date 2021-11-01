using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(CharacterStats))]
public class CharacterStatsEditor : Editor
{
    private CharacterStats statsTarget;
    private void OnEnable()
    {
        statsTarget = target as CharacterStats;
    }

    public override void OnInspectorGUI()
    {


        EditorGUILayout.HelpBox("Put me in the VRRig prefab on SCR_Locomation script, at player Stats. Right under movement",MessageType.Info);
        statsTarget.xPower = EditorGUILayout.FloatField("XPower", statsTarget.xPower);
        statsTarget.yPower = EditorGUILayout.FloatField("YPower",statsTarget.yPower);

        statsTarget.zPower = EditorGUILayout.FloatField("ZPower",statsTarget.zPower);
        EditorGUILayout.HelpBox("WIP ZPower not usefull for now", MessageType.Warning);

        statsTarget.gripBtton = (InputHelpers.Button) EditorGUILayout.EnumPopup("Grip Button", statsTarget.gripBtton);
        statsTarget.fingerButton = (InputHelpers.Button)EditorGUILayout.EnumPopup("Activate Finger button", statsTarget.fingerButton);
        EditorUtility.SetDirty(statsTarget);
        Repaint();
        serializedObject.ApplyModifiedProperties();
    }
}
