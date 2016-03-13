using Microsoft.Xna.Framework;
using Storm.ExternalEvent;
using Storm.StardewValley.Event;
using Storm.StardewValley.Wrapper;

namespace SimpleSprinkler.Source.DynamicModAPI
{
	[Mod]
	public class STORMMod : DiskResource
	{
		private SimpleConfig Config
		{
			get
			{
				return SimpleConfig.Instance;
			}
		}

		private SimpleSprinklerMod mod;

		[Subscribe]
		public void InitializeCallback(InitializeEvent @event)
		{
			SimpleSprinklerMod.Log("STORM Loaded");
			mod = new SimpleSprinklerMod();
		}

		[Subscribe]
		public void WarpFarmerCallback(WarpFarmerEvent @event)
		{
			if (!Config.LocationsToListenTo.Contains(@event.Location.Name))
				return;
			CalculateLocation(@event.Location);
		}

		public void CalculateLocation(GameLocation location)
		{
			//Not Working because STORM does not contain needed functions
			SimpleSprinklerMod.Log("Updating Location:{0}", location.Name);
			foreach (var obj in location.Objects.Values)
			{
				if (obj.SpecialVariable == Config.Level1SprinklerID)
				{
					CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level1SprinklerRange);
				}
				else if (obj.SpecialVariable == Config.Level2SprinklerID)
				{
					CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level2SprinklerRange);
				}
				else if (obj.SpecialVariable == Config.Level3SprinklerID)
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
			if (farm.TerrainFeatures.ContainsKey(location) && farm.TerrainFeatures[location] is HoeDirt)
			{
				(farm.TerrainFeatures[location] as HoeDirt).State = 1;
			}
		}
	}
}