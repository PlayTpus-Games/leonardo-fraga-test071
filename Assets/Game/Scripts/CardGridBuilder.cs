using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(CardSpawner)), ExecuteInEditMode]
public class CardGridBuilder : MonoBehaviour
{
    [SerializeField] private BoxCollider _col;
    [SerializeField] private CardSpawner _spawner;
    [Space(15)]
    [Tooltip("At least of axis have to be a pair number.")]
    [SerializeField] private Vector2Int _gridSize;
    private Vector2Int _prevGridSize;
    private int rows => _gridSize.y;
    private int columns => _gridSize.x;
    private int TotalSize => _gridSize.x * _gridSize.y;
    
    [SerializeField] private bool _preserveAspectRatio;
    private bool _prevPreserveAspectRatio;
    [SerializeField, InlineEditor, Required("This field is required.")] private GridConfigData _gridData;
    [SerializeField, InlineEditor, Required("This field is required.")] private CardSizeConfigData _cardData;
    [SerializeField, InlineEditor, Required("This field is required.")] private CardImageData _cardImageData;
    [SerializeField] private bool _showGizmos;
    
    private Vector3[,] _gridPositions;
    private Bounds _bounds => _col.bounds;
    private Vector3 _center => _bounds.center + new Vector3(0f, _bounds.extents.y, 0f);
    private Vector3 _gridWorldSize => _bounds.size - new Vector3(_gridData.Padding.x, 0, _gridData.Padding.y) * 0.5f;
    private Vector3 _cardSize;
    private Vector3 _initialCardPos;
    
    private void Reset()
    {
        _gridSize = new Vector2Int(2, 2);
        _col = GetComponent<BoxCollider>();
        _spawner = GetComponent<CardSpawner>();
        _spawner.DeleteAll();
    }

    private void Update()
    {
        ClampGridSize();
        
        if (NothingChanged())
            return;
        
        if (_showGizmos && (!_gridData || !_cardData || !_cardImageData))
        {
            Debug.LogWarning($"Show Gizmos can only be activate when all of the required fields are filled.");
            _showGizmos = false;
            return;
        }
        
        CalculateCardSize();
        CalculateInitialCardPosition();
        CreateGridPositions();
        _spawner.Spawn(TotalSize, _cardSize, _gridPositions, _cardImageData);

        _prevGridSize = _gridSize;
        _prevPreserveAspectRatio = _preserveAspectRatio;
    }

    private bool NothingChanged()
    {
        return _prevGridSize == _gridSize &&
               _prevPreserveAspectRatio == _preserveAspectRatio &&
               !_gridData.Updated &&
               !_cardData.Updated &&
               !_cardImageData.Updated;
    }
    
    private void ClampGridSize()
    {
        if (_prevGridSize != _gridSize)
        {
            if (_gridSize.x * _gridSize.y > _cardImageData.Sprites.Length)
            {
                if (_prevGridSize.x != _gridSize.x)
                    _gridSize.x = _cardImageData.Sprites.Length / _gridSize.y;
                else
                    _gridSize.y = _cardImageData.Sprites.Length / _gridSize.x;
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
        }
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
    
    private void CreateGridPositions()
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
    
    private void OnDrawGizmos()
    {
        if (!_showGizmos || !_gridData || !_cardData || !_cardImageData)
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
}
