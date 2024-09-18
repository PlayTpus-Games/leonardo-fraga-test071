using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "default-card-size", menuName = "Game/Card Size Configuration")]
public class CardSizeConfigData : ScriptableObject
{
    [SerializeField] private Vector2 _maxSize = Vector2.one * 2f;
    [SerializeField] private Vector2 _minSize = Vector2.one * 0.1f;
    [SerializeField] private float _thickness = 0.1f;
    [Tooltip("The proportion between width and height (MaxSize.x / MaxSize.y)")]
    [SerializeField, ReadOnly] private float _aspectRatio;
    
    public Vector2 MaxSize => _maxSize;
    public Vector2 MinSize => _minSize;
    public float Thickness => _thickness;
    public float AspectRatio => _aspectRatio;

    private bool _updated;
    public bool Updated => _updated;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _updated = true;
        EditorApplication.delayCall += () => _updated = false;
        
        _maxSize = new Vector2(Mathf.Clamp(_maxSize.x, 0.1f, 20), Mathf.Clamp(_maxSize.y, 0.1f, 20));
        _minSize = new Vector2(Mathf.Clamp(_minSize.x, 0.1f, 9), Mathf.Clamp(_minSize.y, 0.1f, 9));
        _thickness = Mathf.Clamp(_thickness, 0.1f, 2f);
        _aspectRatio = _maxSize.x / _maxSize.y;
    }
#endif
}