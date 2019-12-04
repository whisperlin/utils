// Cinema Suite 2014

namespace CinemaDirector
{
    /// <summary>
    /// Enum of Unity playback modes
    /// </summary>
    public enum PlaybackMode
    {
        Disabled = 0, // Base case, we don't want whatever to happen at all.
        Runtime = 1, // Unity is in runtime mode
        EditMode = 2, // Unity is in edit mode
        RuntimeAndEdit = 3, // Both runtime and edit mode.
    }
}
