using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private AudioMixer _mixer;
    [SerializeField]
    private string _exposedVolumeProperty;
    
    public void SetLevel(float level)
    {
        _mixer.SetFloat(_exposedVolumeProperty, Mathf.Log(level) * 20f);
    }
}
