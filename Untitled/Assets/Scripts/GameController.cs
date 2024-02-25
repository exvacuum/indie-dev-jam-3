using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private Fader _pauseMenuFader;
    [SerializeField]
    private Fader _settingsFader;
    [SerializeField]
    private Fader _fadeToBlackFader;
    
    private bool _changedPauseStateThisFrame;

    private void Update()
    {
        _changedPauseStateThisFrame = false;
    }

    private void OnPause(InputValue value)
    {
        if (!_changedPauseStateThisFrame && !_gameManager.IsPaused)
        {
            Pause();
        }
    }

    public void Pause()
    {
        _changedPauseStateThisFrame = true;
        _gameManager.Pause();
        _pauseMenuFader.FadeIn().SetUpdate(true);
    }

    private void OnResume(InputValue value)
    {
        if (!_changedPauseStateThisFrame && _gameManager.IsPaused)
        {
            if (_pauseMenuFader.IsIn)
            {
                Resume();
            }
            else if (_settingsFader.IsIn)
            {
                _settingsFader.StartFadeOut(true);
                _pauseMenuFader.StartFadeIn(true);
            }
        }
    }

    public void Resume()
    {
        _changedPauseStateThisFrame = true;
        _gameManager.Resume();
        _pauseMenuFader.FadeOut().SetUpdate(true);
    }

    public void QuitToMenu()
    {
        _fadeToBlackFader.FadeIn().SetUpdate(true).OnComplete(() => _gameManager.QuitToTitle());
    }
}
