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
    public class ModEntry : Mod
    {
        /// <summary>The "address" of the custom tracker's texture in Stardew's content manager.</summary>
        string TrackerLoadString { get; } = "LooseSprites\\CustomTracker";
        /// <summary>The "address" of the "forage mode" tracker's background texture in Stardew's content manager.</summary>
        string BackgroundLoadString { get; } = "LooseSprites\\CustomTrackerForageBG";

        /// <summary>The loaded texture for the custom tracker.</summary>
        Texture2D Spritesheet { get; set; } = null;
        /// <summary>The loaded texture for the "forage mode" tracker's background.</summary>
        Texture2D Background { get; set; } = null;
        /// <summary>A rectangle describing the spritesheet location of the custom tracker.</summary>
        Rectangle SpriteSource { get; set; }
        /// <summary>A rectangle describing the spritesheet location of the "forage mode" tracker's background.</summary>
        Rectangle BackgroundSource { get; set; }

        /// <summary>True after this mod has attempted to load its textures. Used to avoid unnecessary reloading.</summary>
        bool TexturesLoaded { get; set; } = false;
        /// <summary>True if forage icons are being displayed instead of the custom tracker. If the tracker texture cannot be loaded, this may be used as a fallback.</summary>
        bool ForageIconMode { get; set; } = false;

        /// <summary>The mod's config.json settings.</summary>
        ModConfig MConfig { get; set; } = null;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            /*** load config settings ***/

            MConfig = helper.ReadConfig<ModConfig>(); //load the mod's config.json file
            if (MConfig == null) //if loading failed
                return;

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            if (MConfig.DrawBehindInterface) //if the tracker should be drawn behind the HUD
            {
                helper.Events.Display.RenderingHud += Display_RenderingHud; //use the "rendering" event
            }
            else //if the tracker should be drawn in front of the HUD
            {
                helper.Events.Display.RenderedHud += Display_RenderedHud; //use the "rendered" event
            }
        }

        /// <summary>Loads textures </summary>
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!TexturesLoaded) //if textures have NOT been loaded yet
            {
                if (!MConfig.ReplaceTrackersWithForageIcons) //if the forage icons should NOT be used
                {
                    try
                    {
                        Spritesheet = Game1.content.Load<Texture2D>(TrackerLoadString); //load the custom tracker spritesheet
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to load the custom tracker texture \"{TrackerLoadString}\". There may be a problem with the Content Patcher pack or its settings.", LogLevel.Warn);
                        Monitor.Log($"Forage icons will be displayed instead. Original error message:", LogLevel.Warn);
                        Monitor.Log($"{ex.Message}", LogLevel.Warn);
                        return;
                    }
                }

                if (MConfig.ReplaceTrackersWithForageIcons || Spritesheet == null) //if the forage icons should be used (due to settings OR because the custom tracker failed to load)
                {
                    ForageIconMode = true; //indicate that forage icons are being used
                    Spritesheet = Game1.objectSpriteSheet; //get the object spritesheet

                    try
                    {
                        Background = Game1.content.Load<Texture2D>(BackgroundLoadString); //load the forage icons' custom background
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to load the forage mode background texture \"{BackgroundLoadString}\". There may be a problem with the Content Patcher pack or its settings.", LogLevel.Warn);
                        Monitor.Log($"Forage icons will be displayed without a background. Original error message:", LogLevel.Warn);
                        Monitor.Log($"{ex.Message}", LogLevel.Warn);
                        return;
                    }                    
                }

                TexturesLoaded = true;
            }
        }

        /// <summary>Tasks performed before rendering the HUD.</summary>
        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            RenderCustomTrackers(MConfig.ReplaceTrackersWithForageIcons);
        }

        /// <summary>Tasks performed after rendering the HUD.</summary>
        private void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            RenderCustomTrackers(MConfig.ReplaceTrackersWithForageIcons);
        }

        /// <summary>Renders all necessary custom trackers for the current frame.</summary>
        /// <param name="forageIcon">If true, render the targeted forage object instead of the custom tracker icon.</param>
        private void RenderCustomTrackers(bool forageIcon = false)
        {
            if (!Context.IsPlayerFree) //if the world isn't ready or the player isn't free
                return;

            if (!MConfig.EnableTrackersWithoutProfession && !Game1.player.professions.Contains(17)) //if the player needs to unlock the Tracker profession
                return;

            if (!Game1.currentLocation.IsOutdoors || Game1.eventUp || Game1.farmEvent != null) //if the player is indoors or an event is happening
                return;

            //render custom trackers for each relevant object at the player's current location
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if (pair.Value.isSpawnedObject.Value || pair.Value.ParentSheetIndex == 590) //if this is a "spawned object" or a buried artifact
                {
                    if (ForageIconMode) //if this is rendering forage icons
                    {
                        SpriteSource = GameLocation.getSourceRectForObject(pair.Value.ParentSheetIndex); //get this object's spritesheet source rectangle

                        if (Background != null) //if a background was successfully loaded
                        {
                            BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                        }
                    }
                    else //if this is rendering the custom tracker
                    {
                        SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                    }

                    DrawCustomTracker(pair.Key); //draw a tracker for this object
                }
            }

            if (Game1.currentLocation.orePanPoint.Value != Point.Zero) //if the current location has an ore panning site
            {
                Texture2D objectSheet = Spritesheet; //store the spritesheet, in case it needs to be changed during this process

                if (ForageIconMode) //if this is rendering forage icons
                {
                    Spritesheet = Game1.currentLocation.orePanAnimation.Texture; //get the ore animation's spritesheet
                    SpriteSource = Game1.currentLocation.orePanAnimation.sourceRect; //get the ore animation's current source rectangle

                    if (Background != null) //if a background was successfully loaded
                    {
                        BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                    }
                }
                else //if this is rendering the custom tracker
                {
                    SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                }

                Vector2 panVector = new Vector2(Game1.currentLocation.orePanPoint.Value.X, Game1.currentLocation.orePanPoint.Value.Y); //convert the point into a vector
                DrawCustomTracker(panVector); //draw a tracker for the panning site

                Spritesheet = objectSheet; //restore the previous spritesheet, in case it was changed for this process
            }
        }

        /// <summary>Draw a tracker pointing to the provided tile of the player's current location.</summary>
        /// <param name="targetTile">The coordinates of the tile this tracker icon should point toward.</param>
        /// <remarks>
        /// This method imitates code from Stardew's Game1.drawHUD method.
        /// It draws a rotated, rescaled tracker icon to Game1.spriteBatch at the edge of the player's screen.
        /// </remarks>
        public void DrawCustomTracker(Vector2 targetTile)
        {
            if (Utility.isOnScreen(targetTile * 64f + new Vector2(32f, 32f), 64)) //if the target tile is on the player's screen
                return; //do not track it

            float scale = MConfig.TrackerPixelScale; //get the intended scale of the sprite
            Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen

            //define relative minimum and maximum sprite positions
            float minX = 8f;
            float minY = 8f;
            float maxX = bounds.Right - 8;
            float maxY = bounds.Bottom - 8;

            Vector2 renderSize = new Vector2((float)SpriteSource.Width * scale, (float)SpriteSource.Height * scale); //get the render size of the sprite

            Vector2 trackerRenderPosition = new Vector2();
            float rotation = 0.0f;

            Vector2 centerOfObject = new Vector2((targetTile.X * 64) + 32, (targetTile.Y * 64) + 32); //get the center pixel of the object
            Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

            if (targetPixel.X > (double)(Game1.viewport.MaxCorner.X - 64)) //if the object is RIGHT of the screen
            {
                trackerRenderPosition.X = maxX; //use the predefined max X
                rotation = 1.570796f;
                targetPixel.Y = centerOfObject.Y - (renderSize.X / 2); //adjust Y for rotation
            }
            else if (targetPixel.X < (double)Game1.viewport.X) //if the object is LEFT of the screen
            {
                trackerRenderPosition.X = minX; //use the predefined min X
                rotation = -1.570796f;
                targetPixel.Y = centerOfObject.Y + (renderSize.X / 2); //adjust Y for rotation
            }
            else
                trackerRenderPosition.X = targetPixel.X - (float)Game1.viewport.X; //use the target X (adjusted for viewport)

            if (targetPixel.Y > (double)(Game1.viewport.MaxCorner.Y - 64)) //if the object is DOWN from the screen
            {
                trackerRenderPosition.Y = maxY; //use the predefined max Y
                rotation = 3.141593f;
                if (trackerRenderPosition.X > minX) //if X is NOT min (i.e. this is NOT the bottom left corner)
                {
                    trackerRenderPosition.X = Math.Min(centerOfObject.X + (renderSize.X / 2) - (float)Game1.viewport.X, maxX); //adjust X for rotation (using renderPos, clamping to maxX, and adjusting for viewport)
                }
            }
            else
            {
                trackerRenderPosition.Y = targetPixel.Y >= (double)Game1.viewport.Y ? targetPixel.Y - (float)Game1.viewport.Y : minY; //if the object is UP from the screen, use the predefined min Y; otherwise, use the target Y (adjusted for viewport)
            }

            if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == minY) //if X and Y are min (TOP LEFT corner)
            {
                trackerRenderPosition.Y += SpriteSource.Height; //adjust DOWN based on sprite size
                rotation += 0.7853982f;
            }
            else if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == maxY) //if X is min and Y is max (BOTTOM LEFT corner)
            {
                trackerRenderPosition.X += SpriteSource.Width; //adjust RIGHT based on sprite size
                rotation += 0.7853982f;
            }
            else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == minY) //if X is max and Y is min (TOP RIGHT corner)
            {
                trackerRenderPosition.X -= SpriteSource.Width; //adjust LEFT based on sprite size
                rotation -= 0.7853982f;
            }
            else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == maxY) //if X and Y are max (BOTTOM RIGHT corner)
            {
                trackerRenderPosition.Y -= SpriteSource.Height; //adjust UP based on sprite size
                rotation -= 0.7853982f;
            }

            if (Background != null) //if a background was successfully loaded
            {
                Game1.spriteBatch.Draw(Background, trackerRenderPosition, new Rectangle?(BackgroundSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the background on the game's main sprite batch (note: this will be off-center if spritesheet and background are different sizes)
            }

            Game1.spriteBatch.Draw(Spritesheet, trackerRenderPosition, new Rectangle?(SpriteSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the spritesheet on the game's main sprite batch
        }

        public class ModConfig
        {
            /// <summary>If true, trackers will be enabled even if the player doesn't have the Tracker profession.</summary>
            public bool EnableTrackersWithoutProfession = false;

            /// <summary>If true, an image of the forage being tracked will be displayed instead of the tracker icon.</summary>
            public bool ReplaceTrackersWithForageIcons = false;

            /// <summary>If true, trackers will be drawn behind the HUD. If false, they will be drawn in front of the HUD.</summary>
            public bool DrawBehindInterface = false;

            /// <summary>The scale at which the tracker's texture(s) will be rendered. The size of each pixel in the original texture is multiplied by this value.</summary>
            public float TrackerPixelScale = 4f;

            public ModConfig()
            {

            }
        }
    }
}
