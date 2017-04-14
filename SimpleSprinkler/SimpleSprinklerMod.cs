using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SimpleSprinkler.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SimpleSprinkler
{
    internal class SimpleSprinklerMod : Mod
    {
        /*********
        ** Properties
        *********/
        private SimpleConfig Config;
        private GameLocation Location;


        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<SimpleConfig>();

            LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (!Config.Locations.Contains(e.NewLocation.Name))
                return;
            Location = e.NewLocation;
            foreach (var obj in Location.Objects.Values)
                this.CalculateSimpleSprinkler(obj.ParentSheetIndex, obj.TileLocation, SetWatered);
        }

        /****
        ** Methods
        ****/
        private void SetWatered(Vector2 position)
        {
            if (Location.terrainFeatures.ContainsKey(position) && Location.terrainFeatures[position] is HoeDirt)
                ((HoeDirt)Location.terrainFeatures[position]).state = HoeDirt.watered;
        }

        private bool IsSimpleSprinkler(int parentSheetIndex, out float range)
        {
            foreach (var sprinkler in this.Config.Radius)
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

        private void CalculateSimpleSprinkler(int parentSheetIndex, Vector2 start, Action<Vector2> wateringHandler)
        {
            if (wateringHandler == null)
                return;
            float range;
            if (IsSimpleSprinkler(parentSheetIndex, out range) == false)
                return;
            CalculateSimpleSprinkler(range, start, wateringHandler);
        }

        private void CalculateSimpleSprinkler(float range, Vector2 start, Action<Vector2> wateringHandler)
        {
            if (wateringHandler == null)
                return;
            switch (Config.CalculationMethod)
            {
                case CalculationMethods.BOX:
                    CalculateCircleAndBox(start, range, wateringHandler, false);
                    break;
                case CalculationMethods.CIRCLE:
                    CalculateCircleAndBox(start, range, wateringHandler, true);
                    break;
                case CalculationMethods.HORIZONTAL:
                    CalculateHorizontal(start, range, wateringHandler, true);
                    break;
                case CalculationMethods.VERTICAL:
                    CalculateVertical(start, range, wateringHandler, true);
                    break;
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
                    if (circle && Math.Round(Vector2.Distance(start, location), MidpointRounding.AwayFromZero) > range)
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
    }
}
