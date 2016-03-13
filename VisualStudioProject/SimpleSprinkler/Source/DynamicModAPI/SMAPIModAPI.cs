using StardewModdingAPI.Events;

namespace SimpleSprinkler
{
	public class SMAPIModAPI : SimpleModAPI
	{
		public override void LogInfoMessage(string format, params object[] values)
		{
			StardewModdingAPI.Log.Info(format, values);
		}

		public SMAPIModAPI() : base()
		{
			LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
		}

		private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
		{
			SendUpdateSprinkler(e.NewLocation);
		}
	}
}