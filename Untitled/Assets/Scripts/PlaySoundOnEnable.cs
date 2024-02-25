using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    [SerializeField]
    private AudioManager _audioManager;
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private bool _ui;
    [SerializeField]
    private bool _randomize;

    private void OnEnable()
    {
        if (_ui)
        {
            _audioManager.PlayUISound(_audioClip);
        }
        else
        {
            _audioManager.PlaySoundAt(_audioClip, transform.position, _randomize);
        }
    }
}
