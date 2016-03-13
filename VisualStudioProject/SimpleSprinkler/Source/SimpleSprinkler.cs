using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SimpleSprinkler
{
	public static class SimpleSprinkler
	{
		public static void CalculateSimpleSprinkler(GameLocation farm, Vector2 start, float range)
		{
			Vector2 location = start;
			for (location.X = start.X - range; location.X <= start.X + range; location.X++)
			{
				for (location.Y = start.Y - range; location.Y <= start.Y + range; location.Y++)
				{
					//Circle Mode Clamp, AwayFromZero is used to get a cleaner look which creates longer outer edges
					if (SimpleConfig.Instance.UseCiruclarCalcuation && System.Math.Round(Vector2.Distance(start, location), System.MidpointRounding.AwayFromZero) > range)
						continue;
					SetWatered(farm, location);
				}
			}
		}

		public static void SetWatered(GameLocation farm, Vector2 location)
		{
			if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
			{
				(farm.terrainFeatures[location] as HoeDirt).state = HoeDirt.watered;
			}
		}
	}
}