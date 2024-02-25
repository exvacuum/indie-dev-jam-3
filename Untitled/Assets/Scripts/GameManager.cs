using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName= "Single-Instance SOs/Game Manager")]
public class GameManager : ScriptableObject
{
    [Header("Scenes")]
    [SerializeField]
    private SceneField _menuScene;
    [SerializeField]
    private SceneField _gameScene;

    [Header("Outgoing Events")]
    [SerializeField]
    private GameEvent _gamePausedEvent;
    [SerializeField]
    private GameEvent _gameResumedEvent;
    
    private bool _isPaused;
    public bool IsPaused => _isPaused;

    private void OnEnable()
    {
        _isPaused = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(_gameScene);
    }
    
    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0.0f;
        _gamePausedEvent.Invoke();
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1.0f;
        _gameResumedEvent.Invoke();
    }

    public void QuitToTitle()
    {
        if(_isPaused) Resume();
        SceneManager.LoadScene(_menuScene);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

}
