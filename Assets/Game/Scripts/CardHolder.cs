using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Card> _cards;
    public List<Card> Cards => _cards;
    public void LoadCards(List<Card> cards) => _cards = new List<Card>(cards);

    private IEnumerator Start()
    {
        yield return null;
        GameplayInitializer.Instance.StartLevel(this);
    }
}
