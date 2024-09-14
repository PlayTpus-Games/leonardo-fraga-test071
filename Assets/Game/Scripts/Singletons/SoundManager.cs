using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // Depending on the scope of the game the sounds comes from the object requesting it.
    // For smaller projects with very few sounds, storing the clips on the SoundManager provides a better flow.
    [SerializeField] private SoundClipsData _soundClipsData;
    [SerializeField] private Vector2 _pitchRandomBetweenXY;
    
    /// <summary>
    /// Multiple AudioSources are used to "isolate" the possible pitch and volume changes
    /// A single AudioSource would have its values overwritten when calling clip B while clip A was still playing
    /// </summary>
    private AudioSource[] _audioSources;
    private int _index;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(this);

        _audioSources = GetComponents<AudioSource>();
    }
    
    public void Play_CardFlip(float volume = 1f, bool randomize = true) => PlaySound(_soundClipsData.Flipping, volume, randomize);
    public void Play_Match(float volume = 1f, bool randomize = true) => PlaySound(_soundClipsData.Match, volume, randomize);
    public void Play_Mismatch(float volume = 1f, bool randomize = true) => PlaySound(_soundClipsData.Mismatch, volume, randomize);
    public void Play_Win(float volume = 1f, bool randomize = true) => PlaySound(_soundClipsData.Win, volume, randomize);
    public void Play_Lose(float volume = 1f, bool randomize = true) => PlaySound(_soundClipsData.Lose, volume, randomize);
    
    private void PlaySound(AudioClip clip, float volume = 1f, bool randomizePitch = true)
    {
        AudioSource source = _audioSources[_index++];
        if (_index >= _audioSources.Length)
            _index = 0;

        source.pitch = randomizePitch ? Random.Range(_pitchRandomBetweenXY.x, _pitchRandomBetweenXY.y) : 1f;
        source.PlayOneShot(clip, volume);
    }
}
