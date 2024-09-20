using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardFlipper), typeof(PreGameCardFlipper))]
public class CardMatchingController : MonoBehaviour, IUnloadable
{
    [SerializeField] private LayerMask _cardLayerMask;
    [SerializeField] private float _unflipDelay;

    private PreGameCardFlipper _preGameCardFlipper;
    private CardFlipper _flipper;
    private Camera _camera;
    private Card _selectedCard;
    private HashSet<int> _cardsLeft;
    private bool _started;
    
    private void Awake()
    {
        _preGameCardFlipper = GetComponent<PreGameCardFlipper>();
        _flipper = GetComponent<CardFlipper>();
    }

    private void OnEnable() => _preGameCardFlipper.OnCardsInitialized += StartGameplay;
    private void OnDisable() => _preGameCardFlipper.OnCardsInitialized -= StartGameplay;
    private void StartGameplay(HashSet<int> cardsLeft)
    {
        _camera = FindObjectOfType<Camera>();
        _cardsLeft = new HashSet<int>(cardsLeft);
        _started = true;
    }

    void Update()
    {
        if (!_started)
            return;
        
        if (Input.GetMouseButtonDown(0))
            TryToFlipCard();
    }

    private void TryToFlipCard()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100f, _cardLayerMask))
        {
            Card card = hitInfo.transform.GetComponent<Card>();
            if (!card || card.IsRevealed || card.IsFlipping)
                return;

            _flipper.FlipCard(card, CardFlipper.FlipType.GameplayFlip);

            if (_selectedCard is null)
                _selectedCard = card;
            else if (_selectedCard.Index == card.Index)
                Match(card);
            else
                Mismatch(card);
        }
    }
    private void Match(Card card)
    {
        _cardsLeft.Remove(card.Index);
        StartCoroutine(RemoveCardsFromBoard(_selectedCard, card));
        _selectedCard = null;
        SoundManager.instance.Play_Match();
                    
        if (_cardsLeft.Count == 0)
            StartCoroutine(Victory(card));
    }
    private void Mismatch(Card card)
    {
        StartCoroutine(UnflipCards(_selectedCard, card));
        _selectedCard = null;
        SoundManager.instance.Play_Mismatch();
    }
    
    private IEnumerator UnflipCards(Card prevCard, Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        yield return new WaitForSeconds(_unflipDelay);
        _flipper.FlipCard(prevCard, CardFlipper.FlipType.GameplayFlip);
        _flipper.FlipCard(currentCard, CardFlipper.FlipType.GameplayFlip);
    }
    
    private IEnumerator RemoveCardsFromBoard(Card prevCard, Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        
        prevCard.PlayVFX();
        currentCard.PlayVFX();
        
        Destroy(prevCard.gameObject, 0.5f);
        Destroy(currentCard.gameObject, 0.5f);
    }
    
    private IEnumerator Victory(Card currentCard)
    {
        yield return new WaitWhile(() => currentCard.IsFlipping);
        yield return new WaitForSeconds(_unflipDelay);
        SoundManager.instance.Play_Win(1f, false);
    }

    public void Unload()
    {
        StopAllCoroutines();
        _started = false;
        _selectedCard = null;
        _cardsLeft = new HashSet<int>();
    }
}
