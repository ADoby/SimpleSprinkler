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
		public const string _ModVersion = "1.0";
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