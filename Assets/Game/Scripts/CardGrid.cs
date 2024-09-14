using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CardGrid : MonoBehaviour
{
    public static CardGrid instance;
    
    [SerializeField] private BoxCollider _col;
    [SerializeField] private bool _showGizmos = true;
    [Space(15)] 
    [SerializeField] private LevelData _data;
    
    public Vector3[,] Positions => _gridPositions;
    public int TotalSize => _data.totalSize;
    private Vector3[,] _gridPositions;
    private Bounds _bounds => _col.bounds;
    private Vector3 _center => _bounds.center + new Vector3(0f, _bounds.extents.y, 0f);
    private Vector3 _gridWorldSize => _bounds.size - new Vector3(_data.padding.x, 0, _data.padding.y) * 0.5f;
    public Vector3 CardSize => _cardSize;
    private Vector3 _cardSize;
    private Vector3 _initialCardPos;

    private CardSpawner _spawner;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        _spawner = GetComponent<CardSpawner>();
    }

    public void SetNewLevel(LevelData data)
    {
        _data = data;
        CalculateCardSize();
        CalculateInitialCardPosition();
        CreatePositionGrid();
        _spawner.Spawn();
    }
    
    private void CalculateCardSize()
    {
        Vector2 totalSpacing = new Vector2(_data.spacing.x * (_data.columns - 1), _data.spacing.y * (_data.rows - 1));
        _cardSize = new Vector3((_gridWorldSize.x - totalSpacing.x) / _data.columns, _data.thickness, (_gridWorldSize.z - totalSpacing.y) / _data.rows);
        _cardSize.x = Mathf.Clamp(_cardSize.x, _data.cardMinSize.x, _data.cardMaxSize.x);
        _cardSize.z = Mathf.Clamp(_cardSize.z, _data.cardMinSize.y, _data.cardMaxSize.y);

        if (_data.preserveAspectRatio)
        {
            if (_cardSize.z * _data.aspectRatio > _cardSize.x)
                _cardSize.z = _cardSize.x / _data.aspectRatio;
            else if (_cardSize.z * _data.aspectRatio < _cardSize.x)
                _cardSize.x = _cardSize.z * _data.aspectRatio;
        }
    }

    private void CalculateInitialCardPosition()
    {
        _initialCardPos = _center;
        _initialCardPos.x -= (_data.columns - 1) * (_cardSize.x * 0.5f) + (_data.columns - 1) * (_data.spacing.x * 0.5f);
        _initialCardPos.z += (_data.rows - 1) * (_cardSize.z * 0.5f) + (_data.rows - 1) * (_data.spacing.y * 0.5f);
        _initialCardPos.y += _data.thickness * 0.5f;
    }
    
    private void CreatePositionGrid()
    {
        _gridPositions = new Vector3[_data.rows, _data.columns];
        for (int row = 0; row < _data.rows; row++)
        {
            for (int column = 0; column < _data.columns; column++)
            {
                Vector3 pos = _initialCardPos;
                pos.x += _cardSize.x * column + _data.spacing.x * column;
                pos.z -= _cardSize.z * row + _data.spacing.y * row;
                _gridPositions[row, column] = pos;
            }    
        }
    }
    
#if UNITY_EDITOR
    // Used for visualization only.
    public void UpdateGrid()
    {
        if (_data is null)
            return;
        
        CalculateCardSize();
        CalculateInitialCardPosition();
        CreatePositionGrid();
    }
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
        if (_data.padding == Vector2.zero)
            return;
        
        Vector3 botLeft = _bounds.min;
        botLeft.y = _center.y;
        Vector3 botLeftPadded = botLeft + new Vector3(_data.padding.x, 0f, _data.padding.y) * 0.25f;
        Gizmos.DrawLine(botLeft, botLeftPadded);
        
        Vector3 topLeft = new Vector3(_bounds.min.x, _center.y, _bounds.max.z);
        Vector3 topLeftPadded = topLeft + new Vector3(_data.padding.x, 0f, -_data.padding.y) * 0.25f;
        Gizmos.DrawLine(topLeft, topLeftPadded);
        
        Vector3 topRight = _bounds.max;
        Vector3 topRightPadded = topRight + new Vector3(-_data.padding.x, 0f, -_data.padding.y) * 0.25f;
        Gizmos.DrawLine(topRight, topRightPadded);
        
        Vector3 botRight = new Vector3(_bounds.max.x, _center.y, _bounds.min.z);
        Vector3 botRightPadded = botRight + new Vector3(-_data.padding.x, 0f, _data.padding.y) * 0.25f;
        Gizmos.DrawLine(botRight, botRightPadded);
    }
#endif
}
