/*
 * File: Fader.cs
 * Author: Silas Bartha
 * Last Modified: 2024-02-23
 */

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

/// <summary>
///     Controls a UI canvas group which can be faded in or out using DOTween
/// </summary>
public class Fader : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;
    [SerializeField]
    private bool _in;
    public bool IsIn => _in;
    [SerializeField]
    private float _duration = 1f;

    private void Start()
    {
        // Fade canvas group in or out at the start
        if (_in)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    public void StartFadeIn(bool update) => FadeIn().SetUpdate(update);
    public void StartFadeOut(bool update) => FadeOut().SetUpdate(update);
    
    /// <summary>
    ///     Fade canvas group in
    /// </summary>
    public TweenerCore<float, float, FloatOptions> FadeIn()
    {
        _in = true;
        return _fadeCanvasGroup.DOFade(1, _duration).OnComplete(() => {
            _fadeCanvasGroup.interactable = true;
            _fadeCanvasGroup.blocksRaycasts = true;
        });
    }
    
    /// <summary>
    ///     Fade canvas group out
    /// </summary>
    public TweenerCore<float, float, FloatOptions> FadeOut()
    {
        _in = false;
        return _fadeCanvasGroup.DOFade(0, _duration).OnComplete(() => {
            _fadeCanvasGroup.interactable = false;
            _fadeCanvasGroup.blocksRaycasts = false;
        });
    }
}
