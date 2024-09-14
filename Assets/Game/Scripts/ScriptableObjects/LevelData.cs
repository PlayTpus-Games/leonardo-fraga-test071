using UnityEngine;

[CreateAssetMenu(fileName = "level-data-01", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(2,2);
    [SerializeField] private bool _preserveAspectRatio = true;
    [Tooltip("This was intended to be used with OdinInspector's InLineEditor attribute for a fluid workflow")]
    [SerializeField/*, InLineEditor*/] private GridConfigData _gridData;
    [Tooltip("This was intended to be used with OdinInspector's InLineEditor attribute for a fluid workflow")]
    [SerializeField/*, InLineEditor*/] private CardSizeConfigData _cardData;
    [SerializeField] private CardImageData _imagesData;

    public bool preserveAspectRatio => _preserveAspectRatio;
    public float aspectRatio => _cardData.AspectRatio;
    private Vector2Int _prevGridSize;
    public int rows => _gridSize.y;
    public int columns => _gridSize.x;
    public int totalSize => _gridSize.x * _gridSize.y;
    public Vector2 padding => _gridData.Padding;
    public Vector2 spacing => _gridData.Spacing;
    public float thickness => _cardData.Thickness;
    public Vector2 cardMinSize => _cardData.MinSize; 
    public Vector2 cardMaxSize => _cardData.MaxSize; 
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_prevGridSize == _gridSize)
            return;

        if (_gridSize.x * _gridSize.y > _imagesData.Sprites.Length)
        {
            if (_prevGridSize.x != _gridSize.x)
                _gridSize.x = _imagesData.Sprites.Length / _gridSize.y;
            else
                _gridSize.y = _imagesData.Sprites.Length / _gridSize.x;
        }
        
        _gridSize.x = _gridSize.x < 2 ? 2 : _gridSize.x;
        _gridSize.y = _gridSize.y < 2 ? 2 : _gridSize.y;
        
        // At least one axis must be a pair. Otherwise the pair of cards won't match
        if (_gridSize.x % 2 != 0 && _gridSize.y % 2 != 0)
        {
            if (_gridSize.x >= 3)
                _gridSize.x--;
            else
                _gridSize.x++;
        }

        _prevGridSize = _gridSize;
    }
#endif
}
