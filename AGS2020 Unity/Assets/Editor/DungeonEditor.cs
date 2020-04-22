using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class DungeonEdito : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Level level = target as Level;

        if (GUILayout.Button("ダンジョン生成"))
        {
            level.SendMessage("Bloock");
            Debug.Log("生成");
        }
    }
}
