/*
 * File: StartMusicOnEnable.cs
 * Author: Silas Bartha
 * Last Modified: 2024-02-23
 */

using UnityEngine;

/// <summary>
///     Begins playing music when this object is enabled
/// </summary>
public class StartMusicOnEnable: MonoBehaviour
{
    [SerializeField]
    private AudioClip _musicClip;
    [SerializeField]
    private AudioManager _audioManager;

    private void OnEnable()
    {
        _audioManager.PlayMusic(_musicClip);
    }
}
