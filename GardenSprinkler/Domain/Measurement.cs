namespace GardenSprinkler.Domain
{
    public class Measurement
    {
        /// <summary>
        /// Moisture level of 1 is open.
        /// The higher the value the more moisture in the soil
        /// </summary>
        public int MoistureLevel { get; set; }

        /// <summary>
        /// Light level.
        /// 
        /// Dark ~0.2
        /// Light ~1
        /// </summary>
        public double LightLevel { get; set; }

        public override string ToString()
        {
            return "MoistureLevel: " + MoistureLevel + ", LightLevel: " + LightLevel;
        }
    }
}