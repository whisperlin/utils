
namespace CinemaDirector
{
    /// <summary>
    /// Implement this Interface in Timeline Items that can be optimized in some way before the Cutscene is played.
    /// </summary>
    interface IOptimizable
    {
        // Can the Item be optimized. In some cases users may need to disable the ability.
        bool CanOptimize { get; set; }
        
        // Perform optimization.
        void Optimize();
    }
}
