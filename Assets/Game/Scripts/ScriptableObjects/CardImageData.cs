using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "card-image-database", menuName = "Game/Card's Image Database")]
public class CardImageData : ScriptableObject
{
    [FormerlySerializedAs("_fruits")] [SerializeField] private Sprite[] _sprites;
    public Sprite[] Sprites => _sprites;

    private bool _updated;
    public bool Updated => _updated;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _updated = true;
        EditorApplication.delayCall += () => _updated = false;
    }
#endif
}
