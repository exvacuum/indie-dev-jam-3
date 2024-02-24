using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Invokes a Unity event when a given game event is raised
/// </summary>
public class InvokeUnityEventOnGameEvent: GameEventListener
{
    /// <summary>
    ///     Unity event to invoke
    /// </summary>
    [SerializeField]
    private UnityEvent _unityEvent;
    
    public override void OnEvent(object value = null)
    {
        _unityEvent.Invoke();
    }
}