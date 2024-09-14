using UnityEngine;
[CreateAssetMenu(fileName = "card-image-database", menuName = "Game/Card's Image Database")]
public class CardImageData : ScriptableObject
{
    [SerializeField] private Sprite[] _fruits;
    public Sprite[] Fruits => _fruits;
}
