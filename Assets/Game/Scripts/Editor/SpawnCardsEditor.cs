using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnCards))]
public class SpawnCardsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpawnCards script = (SpawnCards)target;
        
        GUILayout.Space(15);
        if (GUILayout.Button("Update Cards"))
            script.Spawn();
        
        if (GUILayout.Button("Delete Cards"))
            script.DeleteAll();
    }
}
