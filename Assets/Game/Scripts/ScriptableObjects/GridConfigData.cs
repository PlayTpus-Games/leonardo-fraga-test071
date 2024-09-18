using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "default-grid-data", menuName = "Game/Grid Configuration")]
public class GridConfigData : ScriptableObject
{
    [SerializeField] private Vector2 _padding;
    [SerializeField] private Vector2 _spacing;

    public Vector2 Padding => _padding;
    public Vector2 Spacing => _spacing;

    private bool _updated;
    public bool Updated => _updated;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _updated = true;
        EditorApplication.delayCall += () => _updated = false;
        
        _spacing = new Vector2(Mathf.Clamp(_spacing.x, 0, 10), Mathf.Clamp(_spacing.y, 0, 10)); 
    }
#endif
}
