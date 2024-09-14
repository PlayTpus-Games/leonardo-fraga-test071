using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardGrid))]
public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardGrid _grid;
    [SerializeField] private CardImageData _imagesData;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private List<Card> _cards;
    public List<Card> Cards => _cards;
    private (Sprite sprite, int index)[] _spriteIndex;
    private FlipCardsAtBeginning _flipCards;
    private CardMatchingController _matchingController;
    
    private const int MAX_INSTANTIATION_PER_FRAME = 10;

    private void Awake()
    {
        _flipCards = GetComponent<FlipCardsAtBeginning>();
        _matchingController = GetComponent<CardMatchingController>();
    }

    public void Spawn() => StartCoroutine(SpawnCoroutine());
    private IEnumerator SpawnCoroutine()
    {
        DeleteAll();
        yield return null;
        CalculateSpriteIndexes();
        
        _cards = new List<Card>(_grid.TotalSize);
        int i = 0;
        int instantiated = 0;
        foreach (Vector3 pos in _grid.Positions)
        {
            Transform cardClone = Instantiate(_cardPrefab).transform;
            cardClone.localScale = _grid.CardSize;
            cardClone.position = pos;
            
            Card card = cardClone.GetComponent<Card>();
            card.SetCard(_spriteIndex[i++]);
            _cards.Add(card);

            instantiated++;
            if (instantiated >= MAX_INSTANTIATION_PER_FRAME)
            {
                yield return null;
                instantiated = 0;
            }
        }
        
        _matchingController.CountCards();
        _flipCards.FlipCards();
    }
    
    private void CalculateSpriteIndexes()
    {
        int[] spriteIndexes = new int[_imagesData.Sprites.Length];
        for (int i = 0; i < _imagesData.Sprites.Length; i++)
            spriteIndexes[i] = i;

        spriteIndexes.Shuffle();
        
        _spriteIndex = new (Sprite, int)[_grid.TotalSize];
        for (int i = 0; i < _grid.TotalSize; i+=2)
        {
            _spriteIndex[i] = (_imagesData.Sprites[spriteIndexes[i]], spriteIndexes[i]);
            if (i+1 < _spriteIndex.Length)
                _spriteIndex[i+1] = (_imagesData.Sprites[spriteIndexes[i]], spriteIndexes[i]);
        }

        _spriteIndex.Shuffle();
    }
    
    public void DeleteAll()
    {
        while (_cards.Count > 0)
        {
            if (_cards[0] != null)
                Destroy(_cards[0].gameObject);
            
            _cards.RemoveAt(0);
        }

        _cards = new List<Card>(_grid.TotalSize);
    }
}
