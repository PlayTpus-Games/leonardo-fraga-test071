using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class CardGrid : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [Space(15)]
    [Tooltip("At least of axis have to be a pair number.")]
    [SerializeField] private Vector2Int _gridSize;
    private int rows => _gridSize.y;
    private int columns => _gridSize.x;
    public Vector2Int Size => _gridSize;
    public int TotalSize => _gridSize.x * _gridSize.y;
    
    [SerializeField] private bool _preserveAspectRatio;
    [Tooltip("This was intended to be used with OdinInspector's InLineEditor attribute for a fluid workflow")]
    [SerializeField/*, InLineEditor*/] private GridConfigData _gridData;
    [Tooltip("This was intended to be used with OdinInspector's InLineEditor attribute for a fluid workflow")]
    [SerializeField/*, InLineEditor*/] private CardSizeConfigData _cardData;
    [SerializeField] private bool _showGizmos = true;
    
    private Vector3[,] _gridPositions;
    public Vector3[,] Positions => _gridPositions;
    private Bounds _bounds => _col.bounds;
    private Vector3 _center => _bounds.center + new Vector3(0f, _bounds.extents.y, 0f);
    private Vector3 _gridWorldSize => _bounds.size - new Vector3(_gridData.Padding.x, 0, _gridData.Padding.y) * 0.5f;
    public Vector3 CardSize => _cardSize;
    private Vector3 _cardSize;
    private Vector3 _initialCardPos;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // At least one axis must be a pair. Otherwise the pair of cards won't match
        _gridSize = new Vector2Int(Mathf.Clamp(_gridSize.x, 2, 10), Mathf.Clamp(_gridSize.y, 2, 10));
        if (_gridSize.x % 2 != 0 && _gridSize.y % 2 != 0)
        {
            if (_gridSize.x >= 3)
                _gridSize.x--;
            else
                _gridSize.x++;
        }
    }
#endif
    private void Update()
    {
        if (Application.isPlaying || _gridData is null || _cardData is null)
            return;
        
        CalculateCardSize();
        CalculateInitialCardPosition();
        CreatePositionGrid();
    }

    private void CalculateCardSize()
    {
        Vector2 totalSpacing = new Vector2(_gridData.Spacing.x * (columns - 1), _gridData.Spacing.y * (rows - 1));
        _cardSize = new Vector3((_gridWorldSize.x - totalSpacing.x) / columns, _cardData.Thickness, (_gridWorldSize.z - totalSpacing.y) / rows);
        _cardSize.x = Mathf.Clamp(_cardSize.x, _cardData.MinSize.x, _cardData.MaxSize.x);
        _cardSize.z = Mathf.Clamp(_cardSize.z, _cardData.MinSize.y, _cardData.MaxSize.y);

        if (_preserveAspectRatio)
        {
            if (_cardSize.z * _cardData.AspectRatio > _cardSize.x)
                _cardSize.z = _cardSize.x / _cardData.AspectRatio;
            else if (_cardSize.z * _cardData.AspectRatio < _cardSize.x)
                _cardSize.x = _cardSize.z * _cardData.AspectRatio;
        }
    }

    private void CalculateInitialCardPosition()
    {
        _initialCardPos = _center;
        _initialCardPos.x -= (columns - 1) * (_cardSize.x * 0.5f) + (columns - 1) * (_gridData.Spacing.x * 0.5f);
        _initialCardPos.z += (rows - 1) * (_cardSize.z * 0.5f) + (rows - 1) * (_gridData.Spacing.y * 0.5f);
        _initialCardPos.y += _cardData.Thickness * 0.5f;
    }
    
    private void CreatePositionGrid()
    {
        _gridPositions = new Vector3[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 pos = _initialCardPos;
                pos.x += _cardSize.x * column + _gridData.Spacing.x * column;
                pos.z -= _cardSize.z * row + _gridData.Spacing.y * row;
                _gridPositions[row, column] = pos;
            }    
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_center, new Vector3(_gridWorldSize.x, 0f, _gridWorldSize.z));
        Gizmos.DrawSphere(_center, 0.2f);
        
        GizmosPaddingLines();
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_initialCardPos, 0.2f);

        if (_gridPositions is not { Length: > 0 })
            return;
        
        Gizmos.color = Color.green;
        foreach (Vector3 pos in _gridPositions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
            Gizmos.DrawWireCube(pos, _cardSize);
        }
    }

    private void GizmosPaddingLines()
    {
        if (_gridData.Padding == Vector2.zero)
            return;
        
        Vector3 botLeft = _bounds.min;
        botLeft.y = _center.y;
        Vector3 botLeftPadded = botLeft + new Vector3(_gridData.Padding.x, 0f, _gridData.Padding.y) * 0.25f;
        Gizmos.DrawLine(botLeft, botLeftPadded);
        
        Vector3 topLeft = new Vector3(_bounds.min.x, _center.y, _bounds.max.z);
        Vector3 topLeftPadded = topLeft + new Vector3(_gridData.Padding.x, 0f, -_gridData.Padding.y) * 0.25f;
        Gizmos.DrawLine(topLeft, topLeftPadded);
        
        Vector3 topRight = _bounds.max;
        Vector3 topRightPadded = topRight + new Vector3(-_gridData.Padding.x, 0f, -_gridData.Padding.y) * 0.25f;
        Gizmos.DrawLine(topRight, topRightPadded);
        
        Vector3 botRight = new Vector3(_bounds.max.x, _center.y, _bounds.min.z);
        Vector3 botRightPadded = botRight + new Vector3(-_gridData.Padding.x, 0f, _gridData.Padding.y) * 0.25f;
        Gizmos.DrawLine(botRight, botRightPadded);
    }
#endif
}
