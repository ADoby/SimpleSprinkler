using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace SimpleSprinkler
{
	public class SimpleSprinklerMod
	{
		private SimpleConfig Config;

		public SimpleSprinklerMod(SimpleConfig config)
		{
			SetUpEmbededAssemblyResolving();
			this.Config = config;
		}

		public static void Log(string message, params object[] values)
		{
			Console.WriteLine(string.Format("[SimpleSprinkler]:{0}", string.Format(message, values)));
		}

		public bool IsSimpleSprinkler(int parentSheetIndex, out float range)
		{
			foreach (var sprinkler in this.Config.SprinklerConfiguration)
			{
				if (parentSheetIndex == sprinkler.Key)
				{
					range = sprinkler.Value;
					return true;
				}
			}
			range = 0f;
			return false;
		}

		public bool IsSimpleSprinkler(string name, out float range)
		{
			if (name.Equals("Sprinkler"))
			{
				return IsSimpleSprinkler(599, out range);
			}
			if (name.Equals("Quality Sprinkler"))
			{
				return IsSimpleSprinkler(621, out range);
			}
			if (name.Equals("Iridium Sprinkler"))
			{
				return IsSimpleSprinkler(645, out range);
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
			CalculateSimpleSprinkler(range, start, wateringHandler);
		}

		public void CalculateSimpleSprinkler(string sprinklerName, Vector2 start, Action<Vector2> wateringHandler)
		{
			if (wateringHandler == null)
				return;
			float range = 0f;
			if (IsSimpleSprinkler(sprinklerName, out range) == false)
				return;
			CalculateSimpleSprinkler(range, start, wateringHandler);
		}

		public void CalculateSimpleSprinkler(float range, Vector2 start, Action<Vector2> wateringHandler)
		{
			if (wateringHandler == null)
				return;
			if (Config.CalculationMethod == (int)CalculationMethods.VANILLA)
			{
				return;
			}
			else if (Config.CalculationMethod == CalculationMethods.BOX)
			{
				CalculateCircleAndBox(start, range, wateringHandler, false);
			}
			else if (Config.CalculationMethod == CalculationMethods.CIRCLE)
			{
				CalculateCircleAndBox(start, range, wateringHandler, true);
			}
			else if (Config.CalculationMethod == CalculationMethods.HORIZONTAL)
			{
				CalculateHorizontal(start, range, wateringHandler, true);
			}
			else if (Config.CalculationMethod == CalculationMethods.VERTICAL)
			{
				CalculateVertical(start, range, wateringHandler, true);
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

		private void CalculateHorizontal(Vector2 start, float range, Action<Vector2> wateringHandler, bool circle)
		{
			Vector2 location = start;
			for (location.X = start.X - range; location.X <= start.X + range; location.X++)
			{
				wateringHandler.Invoke(location);
			}
		}

		private void CalculateVertical(Vector2 start, float range, Action<Vector2> wateringHandler, bool circle)
		{
			Vector2 location = start;
			for (location.Y = start.Y - range; location.Y <= start.Y + range; location.Y++)
			{
				wateringHandler.Invoke(location);
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
