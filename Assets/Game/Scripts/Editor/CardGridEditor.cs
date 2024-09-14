using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardGrid))]
public class CardGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardGrid script = (CardGrid)target;
        
        GUILayout.Space(15);
        if (GUILayout.Button("Update Grid"))
            script.UpdateGrid();
    }
}
