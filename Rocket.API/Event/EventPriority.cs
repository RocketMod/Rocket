namespace Rocket.API.Event
{
    /// <summary>
    /// Listener methods with <see cref="EventHandler"/> attributes will be called based on the priority.
    /// <para/>
    /// <see cref="LOWEST"/> will be as called first, <see cref="MONITOR"/> will be called as last.
    /// </summary>
    public enum EventPriority
    {
        LOWEST = 0,
        LOW = 1,
        NORMAL = 2,
        HIGH = 3,
        HIGHEST = 4,
        MONITOR = 5
    }
}