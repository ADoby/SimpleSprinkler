using Microsoft.Xna.Framework;
using SimpleSprinkler;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace SimpleSprinkler_SMAPI
{
	internal class SMAPIMod : Mod
	{
	    private SimpleConfig Config;
		private SimpleSprinklerMod mod;
		private GameLocation location;

		public override void Entry(params object[] objects)
		{
			SimpleSprinklerMod.Log("SMAPI Loaded");
		    this.Config = this.Helper.ReadConfig<SimpleConfig>();

            mod = new SimpleSprinklerMod(this.Config);
			LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
		}

		private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
		{
			if (!Config.Locations.Contains(e.NewLocation.Name))
				return;
			location = e.NewLocation;
			foreach (var obj in location.Objects.Values)
			{
				mod.CalculateSimpleSprinkler(obj.ParentSheetIndex, obj.TileLocation, SetWatered);
			}
		}

		public void SetWatered(Vector2 position)
		{
			if (location.terrainFeatures.ContainsKey(position) && location.terrainFeatures[position] is HoeDirt)
			{
				(location.terrainFeatures[position] as HoeDirt).state = HoeDirt.watered;
			}
		}
	}
}