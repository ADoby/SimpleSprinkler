using Microsoft.Xna.Framework;
using SimpleSprinkler;
using Storm;
using Storm.ExternalEvent;
using Storm.StardewValley.Wrapper;
using System;
using System.Reflection;

namespace SimpleSprinkler_STORM
{
	[Mod]
	internal class STORMMod : DiskResource
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
		public void InitializeCallback()
		{
			Logging.Logs("STORM loaded");
			mod = new SimpleSprinklerMod();
		}

		[Subscribe]
		public void WarpFarmerCallback()
		{
			/*location = @event.Location;
			foreach (var obj in location.Objects.Values)
			{
				//WorkAround using ObjectAccessor
				mod.CalculateSimpleSprinkler(obj.Cast<ObjectAccessor>()._GetParentSheetIndex(), obj.TileLocation, SetWatered);
			}*/
		}

		/*
		public void SetWatered(Vector2 position)
		{
			if (!location.TerrainFeatures.ContainsKey(position))
			{
				return;
			}
			if (location.TerrainFeatures[position].Is<HoeDirtAccessor>())
			{
				location.TerrainFeatures[position].Cast<HoeDirtAccessor>()._SetState(1);
			}
		}*/
	}
}