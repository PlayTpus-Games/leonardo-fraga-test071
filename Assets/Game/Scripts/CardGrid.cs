using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CardGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize;
    private int rows => _gridSize.y;
    private int columns => _gridSize.x;
    
    [SerializeField] private Vector2 _padding;
    [SerializeField] private Vector2 _spacing;
    [SerializeField] private Vector2 _cardMaxSize = Vector2.one * 2f;
    [SerializeField] private Vector2 _cardMinSize = Vector2.one * 0.1f;
    [SerializeField] private float _cardThickness = 0.1f;
    [SerializeField] private bool _showGizmos = true;

    private Vector3[,] _gridPositions;
    private BoxCollider _col;
    private Bounds _bounds;
    private Vector3 _center;
    private Vector3 _gridWorldSize;
    private Vector3 _cardSize;
    private Vector3 _initialCardPos;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _gridSize = new Vector2Int(Mathf.Clamp(_gridSize.x, 2, 10), Mathf.Clamp(_gridSize.y, 2, 10));
        _cardMaxSize = new Vector2(Mathf.Clamp(_cardMaxSize.x, 0.1f, 20), Mathf.Clamp(_cardMaxSize.y, 0.1f, 20));
        _cardMinSize = new Vector2(Mathf.Clamp(_cardMinSize.x, 0.1f, 9), Mathf.Clamp(_cardMinSize.y, 0.1f, 9));
        _spacing = new Vector2(Mathf.Clamp(_spacing.x, 0, 10), Mathf.Clamp(_spacing.y, 0, 10)); 
        _cardThickness = Mathf.Clamp(_cardThickness, 0.1f, 2f);
    }
#endif
    private void Awake() => Init();
    private void Init()
    {
        CalculateGridWorldSize();
        CalculateCardSize();
        CalculateInitialCardPosition();
        CreatePositionGrid();
    }

    private void CalculateGridWorldSize()
    {
        _col ??= GetComponent<BoxCollider>();
        _bounds = _col.bounds;
        _center = _bounds.center;
        _center.y += _bounds.extents.y;
        _gridWorldSize = _bounds.size - new Vector3(_padding.x, 0, _padding.y) * 0.5f;
    }

    private void CalculateCardSize()
    {
        Vector2 totalSpacing = new Vector2(_spacing.x * (columns - 1), _spacing.y * (rows - 1));
        _cardSize = new Vector3((_gridWorldSize.x - totalSpacing.x) / columns, _cardThickness, (_gridWorldSize.z - totalSpacing.y) / rows);
        _cardSize.x = Mathf.Clamp(_cardSize.x, _cardMinSize.x, _cardMaxSize.x);
        _cardSize.z = Mathf.Clamp(_cardSize.z, _cardMinSize.y, _cardMaxSize.y);
    }

    private void CalculateInitialCardPosition()
    {
        _initialCardPos = _center;
        _initialCardPos.x -= (columns - 1) * (_cardSize.x * 0.5f) + (columns - 1) * (_spacing.x * 0.5f);
        _initialCardPos.z += (rows - 1) * (_cardSize.z * 0.5f) + (rows - 1) * (_spacing.y * 0.5f);
        _initialCardPos.y += _cardThickness * 0.5f;
    }
    
    private void CreatePositionGrid()
    {
        _gridPositions = new Vector3[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 pos = _initialCardPos;
                pos.x += _cardSize.x * column + _spacing.x * column;
                pos.z -= _cardSize.z * row + _spacing.y * row;
                _gridPositions[row, column] = pos;
            }    
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;
        
        Init();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_center, new Vector3(_gridWorldSize.x, 0f, _gridWorldSize.z));
        Gizmos.DrawSphere(_center, 0.2f);
        
        GizmosPaddingLines();
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_initialCardPos, 0.2f);
        
        Gizmos.color = Color.green;
        foreach (Vector3 pos in _gridPositions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
            Gizmos.DrawWireCube(pos, _cardSize);
        }
    }

    private void GizmosPaddingLines()
    {
        if (_padding == Vector2.zero)
            return;
        
        Vector3 botLeft = _bounds.min;
        botLeft.y = _center.y;
        Vector3 botLeftPadded = botLeft + new Vector3(_padding.x, 0f, _padding.y) * 0.25f;
        Gizmos.DrawLine(botLeft, botLeftPadded);
        
        Vector3 topLeft = new Vector3(_bounds.min.x, _center.y, _bounds.max.z);
        Vector3 topLeftPadded = topLeft + new Vector3(_padding.x, 0f, -_padding.y) * 0.25f;
        Gizmos.DrawLine(topLeft, topLeftPadded);
        
        Vector3 topRight = _bounds.max;
        Vector3 topRightPadded = topRight + new Vector3(-_padding.x, 0f, -_padding.y) * 0.25f;
        Gizmos.DrawLine(topRight, topRightPadded);
        
        Vector3 botRight = new Vector3(_bounds.max.x, _center.y, _bounds.min.z);
        Vector3 botRightPadded = botRight + new Vector3(-_padding.x, 0f, _padding.y) * 0.25f;
        Gizmos.DrawLine(botRight, botRightPadded);

    }
#endif
}
