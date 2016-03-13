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

		public enum APIModes
		{
			SMAPI = 1,
			STORM = 2
		}

		public KeyDataCollection ConfigRoot
		{
			get
			{
				return data.Global;
			}
		}

		public int APIMode
		{
			get
			{
				if (!ConfigRoot.ContainsKey("APIMode"))
					return 1;
				return int.Parse(ConfigRoot["APIMode"]);
			}
			set
			{
				ConfigRoot["APIMode"] = value.ToString();
			}
		}

		public bool UseCiruclarCalculation
		{
			get
			{
				if (!ConfigRoot.ContainsKey("UseCiruclarCalculation"))
					return true;
				return bool.Parse(ConfigRoot["UseCiruclarCalculation"]);
			}
			set
			{
				ConfigRoot["UseCiruclarCalculation"] = value.ToString();
			}
		}

		public int Level1SprinklerID
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level1SprinklerID"))
					return 0;
				return int.Parse(ConfigRoot["Level1SprinklerID"]);
			}
			set
			{
				ConfigRoot["Level1SprinklerID"] = value.ToString();
			}
		}

		public float Level1SprinklerRange
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level1SprinklerRange"))
					return 0;
				return float.Parse(ConfigRoot["Level1SprinklerRange"]);
			}
			set
			{
				ConfigRoot["Level1SprinklerRange"] = value.ToString();
			}
		}

		public int Level2SprinklerID
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level2SprinklerID"))
					return 0;
				return int.Parse(ConfigRoot["Level2SprinklerID"]);
			}
			set
			{
				ConfigRoot["Level2SprinklerID"] = value.ToString();
			}
		}

		public float Level2SprinklerRange
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level2SprinklerRange"))
					return 0;
				return float.Parse(ConfigRoot["Level2SprinklerRange"]);
			}
			set
			{
				ConfigRoot["Level2SprinklerRange"] = value.ToString();
			}
		}

		public int Level3SprinklerID
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level3SprinklerID"))
					return 0;
				return int.Parse(ConfigRoot["Level3SprinklerID"]);
			}
			set
			{
				ConfigRoot["Level3SprinklerID"] = value.ToString();
			}
		}

		public float Level3SprinklerRange
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Level3SprinklerRange"))
					return 0;
				return float.Parse(ConfigRoot["Level3SprinklerRange"]);
			}
			set
			{
				ConfigRoot["Level3SprinklerRange"] = value.ToString();
			}
		}

		public string LocationsToListenToString
		{
			get
			{
				if (!ConfigRoot.ContainsKey("Locations"))
					return "";
				return ConfigRoot["Locations"];
			}
			set
			{
				ConfigRoot["Locations"] = value;
			}
		}

		public List<string> LocationsToListenTo = new List<string>();

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
				SimpleModAPI.LogInfo("Default Config Created:{0}", LocalConfigPath);
			}
			else
			{
				LoadConfig(Instance);
				FixConfig(Instance);
				SimpleModAPI.LogInfo("Config Loaded:{0}", LocalConfigPath);
			}

			if (!string.IsNullOrEmpty(Instance.LocationsToListenToString))
			{
				string[] names = Instance.LocationsToListenToString.Split(',');
				if (names != null && names.Length > 0)
				{
					foreach (var name in names)
					{
						Instance.LocationsToListenTo.Add(name);
						SimpleModAPI.LogInfo("Configuration loaded for location:{0}", name);
					}
				}
				else
				{
					Instance.LocationsToListenTo.Add(Instance.LocationsToListenToString);
					SimpleModAPI.LogInfo("Configuration loaded for location:{0}", Instance.LocationsToListenToString);
				}
			}
			else
			{
				SimpleModAPI.LogInfo("No locations are configurated:{0}", Instance.LocationsToListenToString);
			}
		}

		/// <summary>
		/// Default values as plain input here
		/// Should be sufficient
		/// </summary>
		/// <param name="config"></param>
		private static void FixConfig(SimpleConfig config)
		{
			if (config.data == null)
				config.data = new IniParser.Model.IniData();

			if (!config.ConfigRoot.ContainsKey("APIMode"))
				config.ConfigRoot.AddKey("APIMode", ((int)APIModes.SMAPI).ToString());

			if (!config.ConfigRoot.ContainsKey("UseCiruclarCalculation"))
				config.ConfigRoot.AddKey("UseCiruclarCalculation", "true");

			if (!config.ConfigRoot.ContainsKey("Level1SprinklerID"))
				config.ConfigRoot.AddKey("Level1SprinklerID", "599");
			if (!config.ConfigRoot.ContainsKey("Level1SprinklerRange"))
				config.ConfigRoot.AddKey("Level1SprinklerRange", "2");

			if (!config.ConfigRoot.ContainsKey("Level2SprinklerID"))
				config.ConfigRoot.AddKey("Level2SprinklerID", "621");
			if (!config.ConfigRoot.ContainsKey("Level2SprinklerRange"))
				config.ConfigRoot.AddKey("Level2SprinklerRange", "3");

			if (!config.ConfigRoot.ContainsKey("Level3SprinklerID"))
				config.ConfigRoot.AddKey("Level3SprinklerID", "645");
			if (!config.ConfigRoot.ContainsKey("Level3SprinklerRange"))
				config.ConfigRoot.AddKey("Level3SprinklerRange", "5");

			if (!config.ConfigRoot.ContainsKey("Locations"))
				config.ConfigRoot.AddKey("Locations", "Farm");

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