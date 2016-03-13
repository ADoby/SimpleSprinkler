using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SimpleSprinkler.Source.DynamicModAPI
{
	internal class SMAPIMod : Mod
	{
		public override string Name
		{
			get { return SimpleSprinklerMod._ModName; }
		}

		public override string Authour
		{
			get { return SimpleSprinklerMod._ModAuthour; }
		}

		public override string Version
		{
			get { return SimpleSprinklerMod._ModVersion; }
		}

		public override string Description
		{
			get { return SimpleSprinklerMod._ModDescription; }
		}

		private SimpleConfig Config
		{
			get
			{
				return SimpleConfig.Instance;
			}
		}

		private SimpleSprinklerMod mod;

		public override void Entry(params object[] objects)
		{
			SimpleSprinklerMod.Log("SMAPI Loaded");
			mod = new SimpleSprinklerMod();
			LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
		}

		private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
		{
			if (!Config.LocationsToListenTo.Contains(e.NewLocation.Name))
				return;
			CalculateLocation(e.NewLocation);
		}

		public void CalculateLocation(GameLocation location)
		{
			SimpleSprinklerMod.Log("Updating Location:{0}", location.Name);
			foreach (var obj in location.Objects.Values)
			{
				if (obj.ParentSheetIndex == Config.Level1SprinklerID)
				{
					CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level1SprinklerRange);
				}
				else if (obj.parentSheetIndex == Config.Level2SprinklerID)
				{
					CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level2SprinklerRange);
				}
				else if (obj.parentSheetIndex == Config.Level3SprinklerID)
				{
					CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level3SprinklerRange);
				}
			}
		}

		public void CalculateSimpleSprinkler(GameLocation farm, Vector2 start, float range)
		{
			if (farm == null)
				return;
			Vector2 location = start;
			for (location.X = start.X - range; location.X <= start.X + range; location.X++)
			{
				for (location.Y = start.Y - range; location.Y <= start.Y + range; location.Y++)
				{
					//Circle Mode Clamp, AwayFromZero is used to get a cleaner look which creates longer outer edges
					if (SimpleConfig.Instance.UseCircularCalculation && System.Math.Round(Vector2.Distance(start, location), System.MidpointRounding.AwayFromZero) > range)
						continue;
					SetWatered(farm, location);
				}
			}
		}

		public void SetWatered(GameLocation farm, Vector2 location)
		{
			if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
			{
				(farm.terrainFeatures[location] as HoeDirt).state = HoeDirt.watered;
			}
		}
	}
}