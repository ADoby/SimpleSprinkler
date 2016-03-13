using StardewModdingAPI;
using StardewValley;
using System;

using System.Reflection;

namespace SimpleSprinkler
{
	public class SimpleSprinklerMod : Mod
	{
		public override string Name
		{
			get { return "Simple Sprinkler"; }
		}

		public override string Authour
		{
			get { return "Tobias Z"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string Description
		{
			get { return "Sprinkler now work circular"; }
		}

		private SimpleModAPI ModApi;
		private SimpleConfig Config;

		public override void Entry(params object[] objects)
		{
			SetUpEmbededAssemblyResolving();
			Config = new SimpleConfig();
			Config.Init();
			if (Config.APIMode == (int)SimpleConfig.APIModes.SMAPI)
				ModApi = new SMAPIModAPI();
			if (ModApi == null)
			{
				SimpleModAPI.LogInfo("Disabled, probably wrong ModAPI selected");
				return;
			}

			ModApi.AddUpdateCalculationHandler(CalculateLocation);
		}

		private void CalculateLocation(GameLocation location)
		{
			foreach (StardewValley.Object obj in location.Objects.Values)
			{
				if (obj.ParentSheetIndex == Config.Level1SprinklerID)
				{
					SimpleSprinkler.CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level1SprinklerRange);
				}
				else if (obj.parentSheetIndex == Config.Level2SprinklerID)
				{
					SimpleSprinkler.CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level2SprinklerRange);
				}
				else if (obj.parentSheetIndex == Config.Level3SprinklerID)
				{
					SimpleSprinkler.CalculateSimpleSprinkler(location, obj.TileLocation, Config.Level3SprinklerRange);
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