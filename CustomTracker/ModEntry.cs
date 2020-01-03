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
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedHud += Display_RenderedHud;
        }

        private void Display_RenderedHud(object sender, RenderedHudEventArgs e)
        {
            RenderCustomTracker(); //call the custom tracker render method
        }

        /// <summary>Draws the custom tracker to Game1.spriteBatch, imitating code from the Game1.drawHUD method.</summary>
        private void RenderCustomTracker()
        {
            if (!Context.IsPlayerFree || !Game1.player.professions.Contains(17)) //if the player isn't free or doesn't have the Tracker profession
                return;

            if (!Game1.currentLocation.IsOutdoors || Game1.eventUp || Game1.farmEvent != null) //if the player is indoors or an event is happening
                return;

            Texture2D trackerSheet = Game1.content.Load<Texture2D>("LooseSprites\\CustomTracker"); //load the custom tracker spritesheet

            //define non-variable values used in the object loop (intended to be more efficient than the original Game1.drawHUD method)
            Rectangle rectangle = new Rectangle(0, 0, trackerSheet.Width, trackerSheet.Height); //create a rectangle covering the entire tracker spritesheet
            const float scale = 4f; //the intended scale of the sprite
            Vector2 renderSize = new Vector2((float)rectangle.Width * scale, (float)rectangle.Height * scale); //get the render size of the sprite

            //define minimum and maximum sprite positions for use in "renderPos"
            Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen
            float minX = 8f;
            float minY = 8f;
            float maxX = bounds.Right - 8;
            float maxY = bounds.Bottom - 8;

            //imitate SDV's Game1.drawHUD method to render a custom tracker cursor
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if (((bool)((NetFieldBase<bool, NetBool>)pair.Value.isSpawnedObject) || pair.Value.ParentSheetIndex == 590) && !Utility.isOnScreen(pair.Key * 64f + new Vector2(32f, 32f), 64))
                {
                    //removed the original "bounds" definition
                    Vector2 renderPos = new Vector2();
                    float rotation = 0.0f;

                    Vector2 centerOfObject = new Vector2((pair.Key.X * 64) + 32, (pair.Key.Y * 64) + 32); //get the center pixel of the object
                    Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

                    if (targetPixel.X > (double)(Game1.viewport.MaxCorner.X - 64)) //if the object is RIGHT of the screen
                    {
                        renderPos.X = maxX; //use the predefined max X
                        rotation = 1.570796f;
                        targetPixel.Y = centerOfObject.Y - (renderSize.X / 2); //adjust Y for rotation
                    }
                    else if (targetPixel.X < (double)Game1.viewport.X) //if the object is LEFT of the screen
                    {
                        renderPos.X = minX; //use the predefined min X
                        rotation = -1.570796f;
                        targetPixel.Y = centerOfObject.Y + (renderSize.X / 2); //adjust Y for rotation
                    }
                    else
                        renderPos.X = targetPixel.X - (float)Game1.viewport.X; //use the target X (adjusted for viewport)

                    if (targetPixel.Y > (double)(Game1.viewport.MaxCorner.Y - 64)) //if the object is DOWN from the screen
                    {
                        renderPos.Y = maxY; //use the predefined max Y
                        rotation = 3.141593f;
                        if (renderPos.X > minX) //if X is NOT min (i.e. this is NOT the bottom left corner)
                        {
                            renderPos.X = Math.Min(centerOfObject.X + (renderSize.X / 2) - (float)Game1.viewport.X, maxX); //adjust X for rotation (using renderPos, clamping to maxX, and adjusting for viewport)
                        }
                    }
                    else
                    {
                        renderPos.Y = targetPixel.Y >= (double)Game1.viewport.Y ? targetPixel.Y - (float)Game1.viewport.Y : minY; //if the object is UP from the screen, use the predefined min Y; otherwise, use the target Y (adjusted for viewport)
                    }

                    if (renderPos.X == minX && renderPos.Y == minY) //if X and Y are min (TOP LEFT corner)
                    {
                        renderPos.Y += rectangle.Height; //adjust DOWN based on sprite size
                        rotation += 0.7853982f;
                    }
                    else if (renderPos.X == minX && renderPos.Y == maxY) //if X is min and Y is max (BOTTOM LEFT corner)
                    {
                        renderPos.X += rectangle.Width; //adjust RIGHT based on sprite size
                        rotation += 0.7853982f;
                    }
                    else if (renderPos.X == maxX && renderPos.Y == minY) //if X is max and Y is min (TOP RIGHT corner)
                    {
                        renderPos.X -= rectangle.Width; //adjust LEFT based on sprite size
                        rotation -= 0.7853982f;
                    }
                    else if (renderPos.X == maxX && renderPos.Y == maxY) //if X and Y are max (BOTTOM RIGHT corner)
                    {
                        renderPos.Y -= rectangle.Height; //adjust UP based on sprite size
                        rotation -= 0.7853982f;
                    }

                    //removed the original "rectangle" and "renderSize" definitions
                    Vector2 position2 = Utility.makeSafe(renderPos, renderSize);
                    Game1.spriteBatch.Draw(trackerSheet, renderPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the trackerSheet on the game's main sprite batch
                }
            }
        }
    }
}
