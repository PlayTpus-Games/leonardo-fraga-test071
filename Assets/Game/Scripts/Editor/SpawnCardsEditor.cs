using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardSpawner))]
public class SpawnCardsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardSpawner script = (CardSpawner)target;
        
        GUILayout.Space(15);
        if (GUILayout.Button("Update Cards"))
            script.Spawn();
        
        if (GUILayout.Button("Delete Cards"))
            script.DeleteAll();
    }
}
