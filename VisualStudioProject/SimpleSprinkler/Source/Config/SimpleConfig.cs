using System.Collections.Generic;

namespace SimpleSprinkler
{
    public class SimpleConfig
    {
        public CalculationMethods CalculationMethod { get; set; } = CalculationMethods.CIRCLE;
        public string[] Locations { get; set; } = { "Farm", "Greenhouse" };
        public IDictionary<int, float> SprinklerConfiguration { get; set; } = new Dictionary<int, float>
        {
            [599] = 2,
            [621] = 3,
            [645] = 5
        };
    }
}
