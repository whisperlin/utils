
namespace CinemaDirector
{
    /// <summary>
    /// Options for Reverting data when Cutscenes go inactive.
    /// </summary>
    public enum RevertMode
    {
        Revert, // Revert to initial state.
        Finalize // Place into final state.
        // ToDo: AsIs for leaving state the way it is.
    }
}
