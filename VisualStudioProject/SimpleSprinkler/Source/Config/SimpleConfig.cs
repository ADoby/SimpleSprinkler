using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace SimpleSprinkler
{
	public class SimpleConfig
	{
		private const string LocalConfigPath = "Mods/SimpleSprinkler/Config.ini";

		public static SimpleConfig Instance;

		private IniData data;

		public KeyDataCollection ConfigRoot
		{
			get
			{
				return data.Global;
			}
		}

		public enum CalculationMethods
		{
			VANILLA,
			BOX,
			CIRCLE,
			HORIZONTAL,
			VERTICAL
		}

		private const string OldCalculationMethod_Key = "UseCiruclarCalculation";

		private const string CalculationMethod_Key = "CalculationMethod";
		private const string CalculationMethod_DefaultValue = "2";

		public int CalculationMethod
		{
			get
			{
				if (!ConfigRoot.ContainsKey(CalculationMethod_Key))
					ConfigRoot.AddKey(CalculationMethod_Key, CalculationMethod_DefaultValue);
				int value = 0;
				if (!int.TryParse(ConfigRoot[CalculationMethod_Key], out value))
					SimpleSprinklerMod.Log("Failed to load CalculationMethod (must be a number)");
				return value;
			}
		}

		private const string SprinklerConfiguration_Key = "SprinklerConfiguration";
		private const char SprinklerConfiguration_ConfigSplit = ',';
		private const char SprinklerConfiguration_DataSplit = '/';
		private const string SprinklerConfiguration_DefaultValue = "599/2,621/3,645/5";

		public string SprinklerConfiguration
		{
			get
			{
				if (!ConfigRoot.ContainsKey(SprinklerConfiguration_Key))
					ConfigRoot.AddKey(SprinklerConfiguration_Key, SprinklerConfiguration_DefaultValue);
				return ConfigRoot[SprinklerConfiguration_Key];
			}
		}

		private const string Locations_Key = "Locations";
		private const char Locations_ConfigSplit = ',';
		private const string Locations_DefaultValue = "Farm,Greenhouse";

		public string LocationsString
		{
			get
			{
				if (!ConfigRoot.ContainsKey(Locations_Key))
					ConfigRoot.AddKey(Locations_Key, Locations_DefaultValue);
				return ConfigRoot[Locations_Key];
			}
		}

		public List<string> Locations = new List<string>();
		public List<SprinklerConfig> SprinklerConfigs = new List<SprinklerConfig>();

		public class SprinklerConfig
		{
			public int ParentSheetIndex;
			public float Range;

			public SprinklerConfig(int id, float range)
			{
				ParentSheetIndex = id;
				Range = range;
			}
		}

		public void Init()
		{
			Instance = this;
			InitConfig();
		}

		/// <summary>
		/// Loads existing config or creates default config
		/// </summary>
		private static void InitConfig()
		{
			//Create Config Directory if it does not exist
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LocalConfigPath));
			if (!System.IO.File.Exists(LocalConfigPath))
			{
				FixConfig(Instance);
				SimpleSprinklerMod.Log("Default Config Created:{0}", LocalConfigPath);
			}
			else
			{
				LoadConfig(Instance);
				FixConfig(Instance);
				SimpleSprinklerMod.Log("Config Loaded:{0}", LocalConfigPath);
			}

			ParseLocations();
			ParseSprinklerConfigs();
		}

		private static void ParseLocations()
		{
			if (!string.IsNullOrEmpty(Instance.LocationsString))
			{
				string[] names = Instance.LocationsString.Split(Locations_ConfigSplit);
				if (names != null && names.Length > 0)
				{
					foreach (var name in names)
					{
						AddLocation(name);
					}
				}
				else
				{
					AddLocation(Instance.LocationsString);
				}
			}
			else
			{
				SimpleSprinklerMod.Log("No Locations loaded");
			}
		}

		private static void AddLocation(string name)
		{
			Instance.Locations.Add(name);
			SimpleSprinklerMod.Log("Configuration loaded for location:{0}", name);
		}

		private static void ParseSprinklerConfigs()
		{
			if (!string.IsNullOrEmpty(Instance.SprinklerConfiguration))
			{
				string[] configs = Instance.SprinklerConfiguration.Split(SprinklerConfiguration_ConfigSplit);
				if (configs != null && configs.Length > 0)
				{
					foreach (var config in configs)
					{
						AddSprinklerConfig(config);
					}
				}
				else
				{
					AddSprinklerConfig(Instance.LocationsString);
				}
			}
			else
			{
				SimpleSprinklerMod.Log("No Sprinkler loaded");
			}
		}

		/// <summary>
		/// Adds a new sprinkler with given config string
		/// </summary>
		/// <param name="config"></param>
		private static void AddSprinklerConfig(string config)
		{
			var data = config.Split(SprinklerConfiguration_DataSplit);
			if (data != null && data.Length == 2)
			{
				int id = 0;
				if (!int.TryParse(data[0], out id))
					return;
				float range = 0f;
				if (!float.TryParse(data[1], out range))
					return;
				Instance.SprinklerConfigs.Add(new SprinklerConfig(id, range));
				SimpleSprinklerMod.Log("Added Sprinkler '{0}' Range '{1}'", id, range);
			}
		}

		/// <summary>
		/// Fixed or upgrades config
		/// </summary>
		/// <param name="config"></param>
		private static void FixConfig(SimpleConfig config)
		{
			if (config.data == null)
				config.data = new IniParser.Model.IniData();

			if (!config.ConfigRoot.ContainsKey(Locations_Key))
				config.ConfigRoot[Locations_Key] = Locations_DefaultValue;
			if (!config.ConfigRoot.ContainsKey(SprinklerConfiguration_Key))
				config.ConfigRoot[SprinklerConfiguration_Key] = SprinklerConfiguration_DefaultValue;

			//Check for old config
			if (config.ConfigRoot.ContainsKey(OldCalculationMethod_Key))
			{
				bool useCircular = false;
				if (bool.TryParse(config.ConfigRoot[OldCalculationMethod_Key], out useCircular))
					Instance.ConfigRoot.AddKey(CalculationMethod_Key, useCircular ? "2" : "1");
				config.ConfigRoot.RemoveKey(OldCalculationMethod_Key);
			}

			if (!config.ConfigRoot.ContainsKey(CalculationMethod_Key))
				Instance.ConfigRoot.AddKey(CalculationMethod_Key, CalculationMethod_DefaultValue);

			//Remove old config keys
			if (config.ConfigRoot.ContainsKey("UseCiruclarCalculation"))
				config.ConfigRoot.RemoveKey("UseCiruclarCalculation");
			if (config.ConfigRoot.ContainsKey("UseCircularCalculation"))
				config.ConfigRoot.RemoveKey("UseCircularCalculation");
			if (config.ConfigRoot.ContainsKey("Level1SprinklerID"))
				config.ConfigRoot.RemoveKey("Level1SprinklerID");
			if (config.ConfigRoot.ContainsKey("Level1SprinklerRange"))
				config.ConfigRoot.RemoveKey("Level1SprinklerRange");
			if (config.ConfigRoot.ContainsKey("Level2SprinklerID"))
				config.ConfigRoot.RemoveKey("Level2SprinklerID");
			if (config.ConfigRoot.ContainsKey("Level2SprinklerRange"))
				config.ConfigRoot.RemoveKey("Level2SprinklerRange");
			if (config.ConfigRoot.ContainsKey("Level3SprinklerID"))
				config.ConfigRoot.RemoveKey("Level3SprinklerID");
			if (config.ConfigRoot.ContainsKey("Level3SprinklerRange"))
				config.ConfigRoot.RemoveKey("Level3SprinklerRange");

			SaveConfig(config);
		}

		private static void LoadConfig(SimpleConfig config)
		{
			config.data = new FileIniDataParser().ReadFile(LocalConfigPath);
		}

		private static void SaveConfig(SimpleConfig config)
		{
			new FileIniDataParser().WriteFile(LocalConfigPath, config.data);
		}
	}
}