/// <summary>
///     Invokes callback when an event is raised
/// </summary>
public interface IGameEventListener
{
    /// <summary>
    ///     Callback triggered when event raised
    /// </summary>
    /// <param name="value">Optional value passed by event</param>
    public void OnEvent(object value = null);
}