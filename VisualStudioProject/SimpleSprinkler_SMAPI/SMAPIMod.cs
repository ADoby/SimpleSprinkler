using Microsoft.Xna.Framework;
using SimpleSprinkler;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;

namespace SimpleSprinkler_SMAPI
{
	internal class SMAPIMod : Mod
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
			get
			{
				return "1.3";
			}
		}

		public override string Description

		{
			get { return "Better Sprinkler"; }
		}

		private SimpleConfig Config
		{
			get
			{
				return SimpleConfig.Instance;
			}
		}

		private SimpleSprinklerMod mod;
		private GameLocation location;

		public override void Entry(params object[] objects)
		{
			SetUpEmbededAssemblyResolving();
			SimpleSprinklerMod.Log("SMAPI Loaded");
			mod = new SimpleSprinklerMod();
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