using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Fader _fadeToBlackFader;
    [SerializeField]
    private Fader _mainMenuFader;
    [SerializeField]
    private Fader _settingsFader;
    [SerializeField]
    private GameManager _gameManager;

    public void StartGame()
    {
        _fadeToBlackFader.FadeIn().OnComplete(() => _gameManager.StartGame());
    }

    public void OnResume(InputValue value)
    {
        if (_settingsFader.IsIn)
        {
            _settingsFader.StartFadeOut(true);
            _mainMenuFader.StartFadeIn(true);
        }
    }
}
