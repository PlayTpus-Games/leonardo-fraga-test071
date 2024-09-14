using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CardGrid))]
public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardGrid _grid;
    [SerializeField] private CardImageData _imagesData;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private List<Card> _cards;
    public List<Card> Cards => _cards;
    
    public void Spawn()
    {
        DeleteAll();
        CalculateSpriteIndexes();
        
        _cards = new List<Card>(_grid.TotalSize);
        int i = 0;
        foreach (Vector3 pos in _grid.Positions)
        {
            Transform cardClone = ((GameObject)PrefabUtility.InstantiatePrefab(_cardPrefab)).transform;
            cardClone.localScale = _grid.CardSize;
            cardClone.position = pos;
            
            Card card = cardClone.GetComponent<Card>();
            card.SetCard(_spriteIndex[i++]);
            _cards.Add(card);
        }
    }

    private (Sprite sprite, int index)[] _spriteIndex;
    private void CalculateSpriteIndexes()
    {
        int[] spriteIndexes = new int[_imagesData.Fruits.Length];
        for (int i = 0; i < _imagesData.Fruits.Length; i++)
            spriteIndexes[i] = i;

        spriteIndexes.Shuffle();
        
        _spriteIndex = new (Sprite, int)[_grid.TotalSize];
        for (int i = 0; i < _grid.TotalSize; i+=2)
        {
            _spriteIndex[i] = (_imagesData.Fruits[spriteIndexes[i]], spriteIndexes[i]);
            if (i+1 < _spriteIndex.Length)
                _spriteIndex[i+1] = (_imagesData.Fruits[spriteIndexes[i]], spriteIndexes[i]);
        }

        _spriteIndex.Shuffle();
    }
    
    public void DeleteAll()
    {
        while (_cards.Count > 0)
        {
            DestroyImmediate(_cards[0].gameObject);
            _cards.RemoveAt(0);
        }

        _cards = new List<Card>(_grid.TotalSize);
    }
}
