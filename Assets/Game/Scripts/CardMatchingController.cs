using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardSpawner), typeof(CardFlipper))]
public class CardMatchingController : MonoBehaviour
{
    [SerializeField] private LayerMask _cardLayerMask;
    [SerializeField] private float _unflipDelay;

    private CardSpawner _cardSpawner;
    private CardFlipper _flipper;
    private Camera _camera;
    private Card _selectedCard;
    private HashSet<int> _cardsLeft;

    private void Awake()
    {
        _cardSpawner = GetComponent<CardSpawner>();
        _flipper = GetComponent<CardFlipper>();
        _camera = Camera.main;
    }

    private void Start()
    {
        _cardsLeft = new HashSet<int>(_cardSpawner.Cards.Count / 2);
        foreach (Card card in _cardSpawner.Cards)
            _cardsLeft.Add(card.Index);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 30f, _cardLayerMask))
            {
                Card card = hitInfo.transform.GetComponent<Card>();
                if (card.IsRevealed || card.IsFlipping)
                    return;

                _flipper.FlipCard(card, CardFlipper.FlipType.CardSelected);

                if (_selectedCard is null)
                    _selectedCard = card;
                else if (_selectedCard.Index == card.Index)
                {
                    _cardsLeft.Remove(card.Index);
                    StartCoroutine(RemoveCardsFromBoard(_selectedCard, card));
                    _selectedCard = null;
                    SoundManager.instance.Play_Match();
                    
                    if (_cardsLeft.Count == 0)
                        StartCoroutine(Victory(card));
                }
                else
                {
                    StartCoroutine(UnflipCards(_selectedCard, card));
                    _selectedCard = null;
                    SoundManager.instance.Play_Mismatch();
                }
            }
        }
    }

    private IEnumerator UnflipCards(Card prevCard, Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        yield return new WaitForSeconds(_unflipDelay);
        _flipper.FlipCard(prevCard, CardFlipper.FlipType.CardSelected);
        _flipper.FlipCard(currentCard, CardFlipper.FlipType.CardSelected);
    }
    
    private IEnumerator RemoveCardsFromBoard(Card prevCard, Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        
        prevCard.PlayMatchEffects();
        currentCard.PlayMatchEffects();
        
        Destroy(prevCard.gameObject, 0.5f);
        Destroy(currentCard.gameObject, 0.5f);
    }
    
    private IEnumerator Victory(Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        yield return new WaitForSeconds(_unflipDelay);
        SoundManager.instance.Play_Win(1f, false);
    }
}
