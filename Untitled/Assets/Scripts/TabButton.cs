using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [SerializeField]
    private AudioManager _audioManager;
    [SerializeField]
    private AudioClip _audioClip;

    public void PlaySoundOnSelect(bool value)
    {
        if (value)
        {
            _audioManager.PlayUISound(_audioClip);
        }
    }
}
