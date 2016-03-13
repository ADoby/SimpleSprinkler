using System;

namespace SimpleSprinkler
{
	/// <summary>
	/// Keeps the mod seperated from modding apis
	/// easy update funktionality, or change of api completely
	/// </summary>
	public abstract class SimpleModAPI
	{
		public static SimpleModAPI Instance { get; protected set; }

		private Action<StardewValley.GameLocation> OnUpdateSprinkler;

		public static void LogInfo(string message, params object[] values)
		{
			if (Instance != null)
				Instance.LogInfoMessage("[SimpleSprinkler]:{0}", string.Format(message, values));
			else
				Console.WriteLine("[SimpleSprinkler]:{0}", string.Format(message, values));
		}

		protected void SendUpdateSprinkler(StardewValley.GameLocation location)
		{
			//If the config does not contain this location we don't update sprinklers
			if (!SimpleConfig.Instance.LocationsToListenTo.Contains(location.Name))
				return;
			if (OnUpdateSprinkler != null)
				OnUpdateSprinkler(location);
		}

		public SimpleModAPI()
		{
			Instance = this;
		}

		public abstract void LogInfoMessage(string format, params object[] values);

		public void AddUpdateCalculationHandler(Action<StardewValley.GameLocation> handler)
		{
			OnUpdateSprinkler -= handler;
			OnUpdateSprinkler += handler;
		}
	}
}