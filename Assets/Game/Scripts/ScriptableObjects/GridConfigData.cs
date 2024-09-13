using System;
using UnityEngine;

[CreateAssetMenu(fileName = "default-grid-data", menuName = "Game/Grid Configuration")]
public class GridConfigData : ScriptableObject
{
    [SerializeField] private Vector2 _padding;
    [SerializeField] private Vector2 _spacing;
    [SerializeField] private Vector2 _cardMaxSize = Vector2.one * 2f;
    [SerializeField] private Vector2 _cardMinSize = Vector2.one * 0.1f;
    [SerializeField] private float _cardThickness = 0.1f;

    public Vector2 Padding => _padding;
    public Vector2 Spacing => _spacing;
    public Vector2 CardMaxSize => _cardMaxSize;
    public Vector2 CardMinSize => _cardMinSize;
    public float CardThickness => _cardThickness;

    private void OnValidate()
    {
        _cardMaxSize = new Vector2(Mathf.Clamp(_cardMaxSize.x, 0.1f, 20), Mathf.Clamp(_cardMaxSize.y, 0.1f, 20));
        _cardMinSize = new Vector2(Mathf.Clamp(_cardMinSize.x, 0.1f, 9), Mathf.Clamp(_cardMinSize.y, 0.1f, 9));
        _spacing = new Vector2(Mathf.Clamp(_spacing.x, 0, 10), Mathf.Clamp(_spacing.y, 0, 10)); 
        _cardThickness = Mathf.Clamp(_cardThickness, 0.1f, 2f);
    }
}
