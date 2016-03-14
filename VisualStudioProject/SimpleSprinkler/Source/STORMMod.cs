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
		private GameLocation location;

		[Subscribe]
		public void InitializeCallback(InitializeEvent @event)
		{
			SimpleSprinklerMod.Log("STORM Loaded");
			mod = new SimpleSprinklerMod();
		}

		[Subscribe]
		public void WarpFarmerCallback(WarpFarmerEvent @event)
		{
			if (!Config.Locations.Contains(@event.Location.Name))
				return;
			location = @event.Location;
			foreach (var obj in location.Objects.Values)
			{
				//Does not work yet, because STORM needs "parentSheetIndex" property
				mod.CalculateSimpleSprinkler(obj.SpecialVariable, obj.TileLocation, SetWatered);
			}
		}

		public void SetWatered(Vector2 position)
		{
			if (location.TerrainFeatures.ContainsKey(position) && location.TerrainFeatures[position] is HoeDirt)
			{
				(location.TerrainFeatures[position] as HoeDirt).State = 1;
			}
		}
	}
}