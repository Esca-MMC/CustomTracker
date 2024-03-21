using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed before rendering the HUD.</summary>
        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Config.DrawBehindInterface) //if trackers should be drawn "beneath" the interface
                RenderCustomTrackers(Config.ReplaceTrackersWithForageIcons);
        }
    }
}
