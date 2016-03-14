using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

using System.Reflection;

namespace SimpleSprinkler
{
	public class SimpleSprinklerMod
	{
		public const string _ModName = "Simple Sprinkler";
		public const string _ModAuthour = "Tobias Z";
		public const string _ModVersion = "1.1";
		public const string _ModDescription = "Better Sprinkler";

		private SimpleConfig Config;

		public SimpleSprinklerMod()
		{
			SetUpEmbededAssemblyResolving();
			Config = new SimpleConfig();
			Config.Init();
		}

		public static void Log(string message, params object[] values)
		{
			Console.WriteLine(string.Format("[SimpleSprinkler]:{0}", string.Format(message, values)));
		}

		public bool IsSimpleSprinkler(int parentSheetIndex, out float range)
		{
			foreach (var sprinkler in Config.SprinklerConfigs)
			{
				if (parentSheetIndex == sprinkler.ParentSheetIndex)
				{
					range = sprinkler.Range;
					return true;
				}
			}
			range = 0f;
			return false;
		}

		public void CalculateSimpleSprinkler(int parentSheetIndex, Vector2 start, Action<Vector2> wateringHandler)
		{
			if (wateringHandler == null)
				return;
			float range = 0f;
			if (IsSimpleSprinkler(parentSheetIndex, out range) == false)
				return;
			if (Config.CalculationMethod == (int)SimpleConfig.CalculationMethods.VANILLA)
			{
				return;
			}
			else if (Config.CalculationMethod == (int)SimpleConfig.CalculationMethods.BOX)
			{
				CalculateCircleAndBox(start, range, wateringHandler, false);
			}
			else if (Config.CalculationMethod == (int)SimpleConfig.CalculationMethods.CIRCLE)
			{
				CalculateCircleAndBox(start, range, wateringHandler, true);
			}
		}

		private void CalculateCircleAndBox(Vector2 start, float range, Action<Vector2> wateringHandler, bool circle)
		{
			Vector2 location = start;
			for (location.X = start.X - range; location.X <= start.X + range; location.X++)
			{
				for (location.Y = start.Y - range; location.Y <= start.Y + range; location.Y++)
				{
					//Circle Mode Clamp, AwayFromZero is used to get a cleaner look which creates longer outer edges
					if (circle && System.Math.Round(Vector2.Distance(start, location), System.MidpointRounding.AwayFromZero) > range)
						continue;
					wateringHandler.Invoke(location);
				}
			}
		}

		private void SetUpEmbededAssemblyResolving()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				string resourceName = new AssemblyName(args.Name).Name + ".dll";
				string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
				{
					Byte[] assemblyData = new Byte[stream.Length];
					stream.Read(assemblyData, 0, assemblyData.Length);
					return Assembly.Load(assemblyData);
				}
			};
		}
	}
}