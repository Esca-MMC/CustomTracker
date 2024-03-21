using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed at the start of each in-game day.</summary>
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            LoadTrackerSprites();
        }
    }
}
