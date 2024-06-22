using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator gen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (gen.autoUpdate)
            {
                gen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            gen.GenerateMap();
        }
    }
}
