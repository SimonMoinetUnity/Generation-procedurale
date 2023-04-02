using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
   public override void OnInspectorGUI() // Remplace le visuel lorsqu'on appuie sur le bouton
   {
        MapGenerator mapGen = (MapGenerator)target; 
                
        if(DrawDefaultInspector())
        {
            if (mapGen.autoUpdate) // Génère automatiquement la carte
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate")) // En appuyant sur le bouton "Genrate" on génère la carte
        {
            mapGen.DrawMapInEditor();
        }
   }
}
