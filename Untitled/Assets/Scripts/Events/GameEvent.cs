using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     A game event. Supports sending messages to specific channels, and passing values with messages.
/// </summary>
[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
public class GameEvent : ScriptableObject
{
    /// <summary>
    ///     Channel value representing the "global" channel.
    ///     Note: Raising an event on the global channel currently ONLY
    ///     invokes global listeners, it does NOT invoke all listeners
    ///     on all channels.
    /// </summary>
    public const int GlobalChannel = -1;
    
    /// <summary>
    ///     A dictionary mapping a set of unique listeners to a channel.
    ///     Listeners can be part of multiple channels.
    /// </summary>
    private readonly Dictionary<int, HashSet<IGameEventListener>> _channelListeners = new();
    
    /// <summary>
    ///     Invoke event on global channel with no data
    /// </summary>
    public void Invoke() => Invoke(GlobalChannel, null);
    
    /// <summary>
    ///     Invoke event on given channel with no data
    /// </summary>
    public void Invoke(int channel) => Invoke(channel, null);
    
    /// <summary>
    ///     Invoke event on given channel with given data
    /// </summary>
    public void Invoke(int channel, object value)
    {
        if (!_channelListeners.TryGetValue(channel, out var listeners))
        {
            return;
        }

        foreach (var listener in listeners!)
        {
            listener.OnEvent(value);
        }

        if (channel == GlobalChannel || !_channelListeners.TryGetValue(GlobalChannel, out var globalListeners))
            return;
        foreach (var listener in globalListeners)
        {
            listener.OnEvent(value);
        }
    }

    /// <summary>
    ///     Register listener on global channel
    /// </summary>
    /// <param name="gameEventListener">Listener to register</param>
    public void RegisterGlobal(IGameEventListener gameEventListener) => Register(gameEventListener, GlobalChannel);

    /// <summary>
    ///     Register listener on given channel
    /// </summary>
    /// <param name="gameEventListener">Listener to register</param>
    /// <param name="channel">Channel to register on</param>
    public void Register(IGameEventListener gameEventListener, int channel)
    {
        if (!_channelListeners.ContainsKey(channel))
        {
            _channelListeners.Add(channel, new HashSet<IGameEventListener>());
        }
        _channelListeners[channel].Add(gameEventListener);
    }

    /// <summary>
    ///     Unregister listener from global channel. Does NOT unregister from all channels.
    /// </summary>
    /// <param name="gameEventListener">Listener to unregister</param>
    public void UnregisterGlobal(IGameEventListener gameEventListener) => Unregister(gameEventListener, GlobalChannel);

    /// <summary>
    ///     Unregister listener from given channel.
    /// </summary>
    /// <param name="gameEventListener">Listener to unregister</param>
    /// <param name="channel">Channel to unregister from</param>
    public void Unregister(IGameEventListener gameEventListener, int channel)
    {
        if (!_channelListeners.ContainsKey(channel))
        {
            Debug.LogWarning("Attempted to unsubscribe from a nonexistent event channel.");
            return;
        }
        _channelListeners[channel].Remove(gameEventListener);
    }
}
