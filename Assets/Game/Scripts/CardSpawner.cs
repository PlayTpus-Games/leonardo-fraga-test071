using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardSpawner : MonoBehaviour
{
    [SerializeField, Required("This field is required")] private GameObject _cardPrefab;
    [SerializeField, ReadOnly] private GameObject _cardHolder;
    [SerializeField, ReadOnly] private List<Card> _cards;
    
    private (Sprite sprite, int index)[] _spriteIndex;
    
    public void Spawn(int totalSize, Vector3 cardSize, Vector3[,] gridPositions, CardImageData imageData)
    {
#if UNITY_EDITOR
        if (!_cardPrefab)
        {
            Debug.LogWarning($"The ''Card Prefab'' field of CardSpawner.cs is required to spawn!");
            return;
        }
        
        DeleteAll();
        CalculateSpriteIndexes(totalSize, imageData);
        
        _cards = new List<Card>(totalSize);
        int i = 0;
        foreach (Vector3 pos in gridPositions)
        {
            Transform cardClone = ((GameObject)PrefabUtility.InstantiatePrefab(_cardPrefab)).transform;
            cardClone.localScale = cardSize;
            cardClone.position = pos;
            
            Card card = cardClone.GetComponent<Card>();
            card.SetCard(_spriteIndex[i++]);
            _cards.Add(card);
        }
        
        AddCardsToCardHolder();
#endif        
    }
    
    public void DeleteAll()
    {
#if UNITY_EDITOR
        if (_cardHolder != null)
            DestroyImmediate(_cardHolder);
        
        CardHolder[] cardHolders = FindObjectsByType<CardHolder>(FindObjectsSortMode.None);
        foreach (CardHolder cardHolder in cardHolders)
            DestroyImmediate(cardHolder.gameObject);

        _cards?.Clear();
#endif
    }
    private void CalculateSpriteIndexes(int totalSize, CardImageData imageData)
    {
        int[] spriteIndexes = new int[imageData.Sprites.Length];
        for (int i = 0; i < imageData.Sprites.Length; i++)
            spriteIndexes[i] = i;

        spriteIndexes.Shuffle();
        
        _spriteIndex = new (Sprite, int)[totalSize];
        for (int i = 0; i < totalSize; i+=2)
        {
            _spriteIndex[i] = (imageData.Sprites[spriteIndexes[i]], spriteIndexes[i]);
            if (i+1 < _spriteIndex.Length)
                _spriteIndex[i+1] = (imageData.Sprites[spriteIndexes[i]], spriteIndexes[i]);
        }

        _spriteIndex.Shuffle();
    }
    private void AddCardsToCardHolder()
    {
        _cardHolder = new GameObject("Card Holder", typeof(CardHolder));
        foreach (Card card in _cards)
            card.transform.SetParent(_cardHolder.transform);
        
        _cardHolder.GetComponent<CardHolder>().LoadCards(_cards);
    }
}
