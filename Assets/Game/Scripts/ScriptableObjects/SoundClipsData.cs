using UnityEngine;

[CreateAssetMenu(fileName = "sfx-data-default", menuName = "Game/Sound Clips Data")]
public class SoundClipsData : ScriptableObject
{
    [SerializeField] private AudioClip _sfxFlipping;
    [SerializeField] private AudioClip _sfxMatch;
    [SerializeField] private AudioClip _sfxMismatch;
    [SerializeField] private AudioClip _sfxWin;
    [SerializeField] private AudioClip _sfxLose;
    
    public AudioClip Flipping => _sfxFlipping;
    public AudioClip Match => _sfxMatch;
    public AudioClip Mismatch => _sfxMismatch;
    public AudioClip Win => _sfxWin;
    public AudioClip Lose => _sfxLose;
}
