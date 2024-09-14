using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "card-image-database", menuName = "Game/Card's Image Database")]
public class CardImageData : ScriptableObject
{
    [FormerlySerializedAs("_fruits")] [SerializeField] private Sprite[] _sprites;
    public Sprite[] Sprites => _sprites;
}
