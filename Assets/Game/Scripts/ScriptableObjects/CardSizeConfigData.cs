using UnityEngine;

[CreateAssetMenu(fileName = "default-card-size", menuName = "Game/Card Size Configuration")]
public class CardSizeConfigData : ScriptableObject
{
    [SerializeField] private Vector2 _maxSize = Vector2.one * 2f;
    [SerializeField] private Vector2 _minSize = Vector2.one * 0.1f;
    [SerializeField] private float _thickness = 0.1f;
    
    public Vector2 MaxSize => _maxSize;
    public Vector2 MinSize => _minSize;
    public float Thickness => _thickness;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _maxSize = new Vector2(Mathf.Clamp(_maxSize.x, 0.1f, 20), Mathf.Clamp(_maxSize.y, 0.1f, 20));
        _minSize = new Vector2(Mathf.Clamp(_minSize.x, 0.1f, 9), Mathf.Clamp(_minSize.y, 0.1f, 9));
        _thickness = Mathf.Clamp(_thickness, 0.1f, 2f);
    }
#endif
}
