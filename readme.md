# Custom Tracker
A mod for the game Stardew Valley, allowing players to use a custom forage tracker icon of any size or shape. It includes options to display the tracker for players without the Tracker profession, display the specific forage being tracked by each icon, and more.

## Contents
* [Installation](#installation)
* [Customization](#customization)
	* [Changing settings](#changing-settings)
  * [Customizing the tracker icon](#customizing-the-tracker-icon)
  * [Creating a content pack](#creating-a-content-pack)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).**
3. **Download CustomTracker** from [the Releases page on GitHub](https://github.com/Esca-MMC/CustomTracker/releases), Nexus Mods, or ModDrop.
4. **Unzip CustomTracker** into your `Stardew Valley\Mods` folder.

## Customization

By default, CustomTracker will hide the original tracker icon and replace it with a large yellow arrow.

To replace the tracker icon or customize this mod's available settings, see the sections below.

### Changing settings

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\CustomTracker` folder.
2. **Exit the game** and open the **config.json** file. It is a text file, so any text editing program should be able to open it.

The available options are:

Name | Valid settings | Description
-----|----------------|------------
EnableTrackersWithoutProfession | true, **false** | When set to true, the player will always be able to see forage tracker icons, even without unlocking the Tracker profession.
ReplaceTrackersWithForageIcons | true, **false** | When set to true, the tracker icon will be replaced with an image of each forage item being tracked.
DrawBehindInterface | true, **false** | When set to true, the tracker icon will be drawn *behind* the game's interface. This makes it easier to see the interface, but harder to see the trackers.

### Customizing the tracker icon

CustomTracker's download includes the `[CP] CustomTracker` folder. This is a content pack for the mod Content Patcher. It loads **tracker.png** into the game and replaces the game's original tracker icon with **blank_cursor.png** to hide it.

If you want to replace your tracker icon with a new one, simply edit or replace **tracker.png** in the `[CP] CustomTracker\assets` folder.

### Creating a content pack

If you'd like to create your own content pack that changes CustomTracker's icon:

1. Copy and paste the `[CP] CustomTracker` folder. Choose a new name and rename the folder to `[CP] YOUR PACK NAME`.
2. Open the new folder, and then open the `assets` folder inside it. Replace **tracker.png** your customized icon.
3. Open the **content.json** text file.
4. Find the section below and replace `"Action": "Load",` with `"Action": "EditImage",`. This is necessary to prevent loading errors.
```
{
  "Action": "Load",
  "Target": "LooseSprites/CustomTracker",
  "FromFile": "assets/tracker.png"
}
```
5. Open the **manifest.json** file.
6. Replace *all* of the text with the template below. Replace the capitalized fields with your username and pack name. (For more information about manifests, see the Stardew Valley Wiki's [Manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) page.)
```
{
   "Name": "[CP] YOUR PACK NAME",
   "Author": "YOUR NAME",
   "Version": "1.0.0",
   "Description": "Loads a custom tracker image for CustomTracker and hides the base game's tracker",
   "UniqueID": "YOURNAME.YOURPACKNAME",
   "MinimumApiVersion": "2.0",
   "ContentPackFor": {
      "UniqueID": "Pathoschild.ContentPatcher",
      "MinimumVersion": "1.3.0"
   },
   "Dependencies": [
   {
      "UniqueID": "Esca.CustomTrackerCP",
      "IsRequired": false
   }
}
```
