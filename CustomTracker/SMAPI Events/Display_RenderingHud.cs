using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTracker
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed before rendering the HUD.</summary>
        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            RenderCustomTrackers(MConfig.ReplaceTrackersWithForageIcons);
        }
    }
}
