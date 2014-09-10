using System.Collections;
using GardenSprinkler.Domain;

namespace GardenSprinkler.Extensions
{
    public static class ArrayListExtensions
    {
        public static Measurement Average(this ArrayList measurements)
        {
            if (measurements.Count == 0)
            {
                return null;
            }

            double averageLight = 0D;
            int averageMoistureLevel = 0;

            foreach (var measurement in measurements)
            {
                var item = (Measurement) measurement;
                averageLight += item.LightLevel;
                averageMoistureLevel += item.MoistureLevel;
            }

            return new Measurement
            {
                LightLevel = averageLight/measurements.Count,
                MoistureLevel = averageMoistureLevel/measurements.Count
            };
        }
    }
}