using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Invokes callback when an event is raised
/// </summary>
public abstract class GameEventListener: MonoBehaviour, IGameEventListener
{
    
    /// <summary>
    ///     Game event to listen on
    /// </summary>
    [SerializeField]
    private GameEvent _gameEvent;

    /// <summary>
    ///     Whether this listener uses channels (is not global)
    /// </summary>
    [SerializeField]
    private bool _useChannels;

    /// <summary>
    ///     Channels to listen on
    /// </summary>
    [SerializeField]
    private List<ushort> _channels;
    

    private void Awake()
    {
        if (_useChannels)
        {
            foreach (var channel in _channels)
            {
                _gameEvent.Register(this, channel);
            }
        }
        else
        {
            _gameEvent.RegisterGlobal(this);
        }
    }

    private void OnDestroy()
    {
        if (_useChannels)
        {
            foreach (var channel in _channels)
            {
                _gameEvent.Unregister(this, channel);
            }
        }
        else
        {
            _gameEvent.UnregisterGlobal(this);
        }
    }

    /// <summary>
    ///     Callback triggered when event raised
    /// </summary>
    /// <param name="value">Optional value passed by event</param>
    public abstract void OnEvent(object value = null);
}