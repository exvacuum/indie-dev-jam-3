using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

/// <summary>
///     Audio Manager, manages a pool of audio sources for playing sound effects,
///     and contains methods for playing various types of audio
/// </summary>
[CreateAssetMenu(menuName = "Single-Instance SOs/Audio Manager")]
public class AudioManager: ScriptableObject
{
    [SerializeField]
    private AudioMixerGroup _musicMixerGroup;
    [SerializeField]
    private AudioMixerGroup _soundEffectMixerGroup;
    [SerializeField]
    private AudioMixerGroup _uiEffectMixerGroup;
    
    private AudioSource _musicSource;
    private IObjectPool<AudioSource> _sourcePool;

    private void OnEnable()
    {
        // Set up object pool
        _sourcePool = new ObjectPool<AudioSource>(
            () => {
                var instance = new GameObject("AudioManager Source").AddComponent<AudioSource>();
                DontDestroyOnLoad(instance.gameObject);
                return instance;
            });
    }

    /// <summary>
    ///     Plays a sound at a given location
    /// </summary>
    /// <param name="sound">Sound to play</param>
    /// <param name="position">Position to play sound at</param>
    /// <param name="randomize">Whether to randomize the pitch and volume of the sound</param>
    public void PlaySoundAt(AudioClip sound, Vector3 position, bool randomize)
    {
        var source = _sourcePool.Get();
        source.spatialize = true;
        source.transform.position = position;
        source.outputAudioMixerGroup = _soundEffectMixerGroup;
        if(randomize)
        {
            source.pitch = Random.Range(0.75f, 1.25f);
            source.volume = Random.Range(0.75f, 1.25f);
        }
        else
        {
            source.pitch = 1f;
            source.volume = 1f;
        }
        source.PlayOneShot(sound);
        _sourcePool.Release(source);
    }

    /// <summary>
    ///     Plays a sound effect on the UI channel
    /// </summary>
    /// <param name="sound">Sound to play</param>
    public void PlayUISound(AudioClip sound)
    {
        var source = _sourcePool.Get();
        source.outputAudioMixerGroup = _uiEffectMixerGroup;
        source.PlayOneShot(sound);
        _sourcePool.Release(source);
    }

    /// <summary>
    ///     Starts playing a sound as looping music on the music channel
    /// </summary>
    /// <param name="music">Music to play</param>
    public void PlayMusic(AudioClip music)
    {
        if (_musicSource == null)
        {
            _musicSource = _sourcePool.Get();
            _musicSource.outputAudioMixerGroup = _musicMixerGroup;
            _musicSource.loop = true;
            _musicSource.spatialize = false;
        } else if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
        }

        _musicSource.clip = music;
        _musicSource.Play();
    }

    /// <summary>
    ///     Stops any playing music
    /// </summary>
    public void StopMusic()
    {
        if (_musicSource == null) return;
        _musicSource.Stop();
    }
}
