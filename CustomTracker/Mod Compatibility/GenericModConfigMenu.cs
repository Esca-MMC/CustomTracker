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
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

            if (api == null) //if the API is not available
                return;

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config)); //register "revert to default" and "write" methods for this mod's config

            //register an option for each of this mod's config settings
            api.RegisterSimpleOption(ModManifest, "Enable trackers without profession", "If this box is checked, you won't need the Foraging skill's \"Tracker\" perk to see trackers.", () => Config.EnableTrackersWithoutProfession, (bool val) => Config.EnableTrackersWithoutProfession = val);
            api.RegisterSimpleOption(ModManifest, "Replace trackers with forage icons", "If this box is checked, trackers will display the objects they're pointing to.", () => Config.ReplaceTrackersWithForageIcons, (bool val) => Config.ReplaceTrackersWithForageIcons = val);
            api.RegisterSimpleOption(ModManifest, "Draw trackers behind interface", "If this box is checked, trackers will be drawn behind the game's interface, making it easier to see the UI.", () => Config.DrawBehindInterface, (bool val) => Config.DrawBehindInterface = val);
            api.RegisterSimpleOption(ModManifest, "Tracker pixel scale", "The size of the tracker icon's pixels (default 4). Increase this to make trackers easier to see.", () => Config.TrackerPixelScale, (float val) => Config.TrackerPixelScale = val);

            api.RegisterSimpleOption(ModManifest, "Track default forage", "If this box is checked, the mod will track most types of forage spawned by the base game.", () => Config.TrackDefaultForage, (bool val) => Config.TrackDefaultForage = val);
            api.RegisterSimpleOption(ModManifest, "Track artifact spots", "If this box is checked, the mod will track buried artifact locations.", () => Config.TrackArtifactSpots, (bool val) => Config.TrackArtifactSpots = val);
            api.RegisterSimpleOption(ModManifest, "Track panning spots", "If this box is checked, the mod will track ore panning locations in the water.", () => Config.TrackPanningSpots, (bool val) => Config.TrackPanningSpots = val);
            api.RegisterSimpleOption(ModManifest, "Track spring onions", "If this box is checked, the mod will track harvestable spring onions.", () => Config.TrackSpringOnions, (bool val) => Config.TrackSpringOnions = val);
            api.RegisterSimpleOption(ModManifest, "Track berry bushes", "If this box is checked, the mod will track harvestable salmonberry and blackberry bushes.", () => Config.TrackBerryBushes, (bool val) => Config.TrackBerryBushes = val);
            api.RegisterSimpleOption(ModManifest, "Track walnut bushes", "If this box is checked, the mod will track harvestable walnut bushes.", () => Config.TrackWalnutBushes, (bool val) => Config.TrackWalnutBushes = val);
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
    }
}
