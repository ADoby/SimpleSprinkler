﻿using System.Linq;
using SimpleSprinkler.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SimpleSprinkler
{
    /// <summary>The mod entry class.</summary>
    internal class SimpleSprinklerMod : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private SimpleConfig Config;

        /// <summary>Encapsulates the logic for building sprinkler grids.</summary>
        private GridHelper GridHelper;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<SimpleConfig>();
            this.GridHelper = new GridHelper(this.Config);

            LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called when the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (this.Config.Locations.Contains(e.NewLocation.Name))
                this.ApplyWatering(e.NewLocation);
        }

        /// <summary>Apply watering for supported sprinklers in a location.</summary>
        /// <param name="location">The location whose sprinklers to apply.</param>
        private void ApplyWatering(GameLocation location)
        {
            // get sprinklers
            var sprinklers = location.Objects.Values.Where(obj => this.IsSprinkler(obj.parentSheetIndex));
            foreach (Object sprinkler in sprinklers)
            {
                foreach (var tile in this.GridHelper.GetGrid(sprinkler.parentSheetIndex, sprinkler.TileLocation))
                {
                    if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                        dirt.state = HoeDirt.watered;
                }
            }
        }

        /// <summary>Get whether the given object ID matches a supported sprinkler.</summary>
        /// <param name="objectId">The object ID.</param>
        private bool IsSprinkler(int objectId)
        {
            return this.Config.Radius.ContainsKey(objectId);
        }
    }
}
