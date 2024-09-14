using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CardSpawner), typeof(CardFlipper))]
public class CardMatchingController : MonoBehaviour
{
    public static CardMatchingController instance;
    
    [SerializeField] private LayerMask _cardLayerMask;
    [SerializeField] private float _unflipDelay;

    private CardSpawner _cardSpawner;
    private CardFlipper _flipper;
    private FlipCardsAtBeginning _flipBeginning;
    private Camera _camera;
    private Card _selectedCard;
    private HashSet<int> _cardsLeft;

    private Action OnMatch; 
    private Action OnMismatch;
    private Action OnVictory;
    public void Subscribe_OnMatch(Action action) => OnMatch += action;
    public void Unsubscribe_OnMatch(Action action) => OnMatch -= action;
    public void Subscribe_OnMismatch(Action action) => OnMismatch += action;
    public void Unsubscribe_OnMismatch(Action action) => OnMismatch -= action;
    public void Subscribe_OnVictory(Action action) => OnVictory += action;
    public void Unsubscribe_OnVictory(Action action) => OnVictory -= action;

    private bool _canUpdate;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        _cardSpawner = GetComponent<CardSpawner>();
        _flipper = GetComponent<CardFlipper>();
        _flipBeginning = GetComponent<FlipCardsAtBeginning>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _flipBeginning.Subscribe_OnCardsUnflipped(BlockUpdate);
        _flipBeginning.Subscribe_OnAllCardsFlipper(UnblockUpdate);
        SceneManager.sceneLoaded += GetCamera;
    }

    private void OnDisable()
    {
        _flipBeginning.Unsubscribe_OnCardsUnflipped(BlockUpdate);
        _flipBeginning.Unsubscribe_OnAllCardsFlipper(UnblockUpdate);
        SceneManager.sceneLoaded -= GetCamera;
    }
    
    private void GetCamera(Scene arg0, LoadSceneMode a) => _camera = Camera.main;
    
    private void BlockUpdate() => _canUpdate = false;
    private void UnblockUpdate() => _canUpdate = true;
    
    public void CountCards()
    {
        _cardsLeft = new HashSet<int>(_cardSpawner.Cards.Count / 2);
        foreach (Card card in _cardSpawner.Cards)
            _cardsLeft.Add(card.Index);
    }

    void Update()
    {
        if (!_canUpdate)
            return;
        
        if (Input.GetMouseButtonDown(0))
            TrySelectCard();
    }

    private void TrySelectCard()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100f, _cardLayerMask))
        {
            Card card = hitInfo.transform.GetComponent<Card>();
            if (card.IsRevealed || card.IsFlipping)
                return;

            _flipper.FlipCard(card, CardFlipper.FlipType.CardSelected);

            if (_selectedCard is null)
                SelectFirstCard(card);
            else if (_selectedCard.Index == card.Index)
                Match(card);
            else
                Mismatch(card);
        }
    }
    private void SelectFirstCard(Card card) => _selectedCard = card;
    private void Match(Card card)
    {
        _cardsLeft.Remove(card.Index);
        StartCoroutine(RemoveCardsFromBoard(_selectedCard, card));
        _selectedCard = null;
        SoundManager.instance.Play_Match();
                    
        if (_cardsLeft.Count == 0)
            StartCoroutine(Victory(card));
        
        OnMatch?.Invoke();
    }
    private void Mismatch(Card card)
    {
        StartCoroutine(UnflipCards(_selectedCard, card));
        _selectedCard = null;
        SoundManager.instance.Play_Mismatch();

        OnMismatch?.Invoke();
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
        yield return new WaitForSeconds(1.5f);
        OnVictory?.Invoke();
        yield return null;
        SaveLoadController.instance.Save();
    }
}
