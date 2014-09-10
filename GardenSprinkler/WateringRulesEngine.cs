using System;
using Gadgeteer;
using GardenSprinkler.Domain;
using GardenSprinkler.Interfaces;
using Microsoft.SPOT;

namespace GardenSprinkler
{
    public class WateringRulesEngine
    {
        private readonly IMeasurementService _measurementService;
        private readonly IWateringService _wateringService;
        private readonly IInternetOfThingsService _internetOfThingsService;
        private Timer _timer;

        public WateringRulesEngine(
            IMeasurementService measurementService, 
            IWateringService wateringService, 
            IInternetOfThingsService internetOfThingsService)
        {
            _measurementService = measurementService;
            _wateringService = wateringService;
            _internetOfThingsService = internetOfThingsService;

            _timer = new Timer(new TimeSpan(0, 0, 0, 20));
            _timer.Tick += timer_Tick;
        }

        public void Start()
        {
            _timer.Start();
        }

        void timer_Tick(Timer timer)
        {
            Debug.Print("Checking if plants need watering...");
            bool waterPlants = ShouldWater();

            if (waterPlants)
            {
                Debug.Print("Watering plants.");
                _wateringService.Water();
            }
        }

        private bool ShouldWater()
        {
            Measurement measurement = _measurementService.GetCurrentMeasurement();

            if (measurement == null)
            {
                return false;
            }

            // If really dry then ignore light/dark setting
            bool waterPlants = false;

            if (MustWater(measurement))
            {
                Debug.Print("Plants are REALLY dry. Water immediately.");
                //_internetOfThingsService.PostStatus("Plants are REALLY dry. Water immediately.");
                waterPlants = true;
            }

            // Check the state of the soil and decide if it needs watering.
            if (NeedsWatering(measurement))
            {
                Debug.Print("Plants are dry and need watering");

                if (IsItAGoodTimeToWater(measurement))
                {
                    waterPlants = true;
                }
                else
                {
                    Debug.Print("It's not a good time to water, holding off for now.");
                }
            }

            return waterPlants;
        }

        /// <summary>
        /// Determine if now is a good time to water.
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        /// <remarks>
        /// Checks light level and if it is going to rain
        /// </remarks>
        private bool IsItAGoodTimeToWater(Measurement measurement)
        {
            // If it is going to rain then don't bother watering
            if (IsItGoingToRain())
            {
                return false;
            }

            // Otherwise only water at night.
            return measurement.LightLevel < 0.8D;
        }

        private bool NeedsWatering(Measurement measurement)
        {
            // ~ 1000 is reasonable for watered plant
            // Lower the value the dryer the plant.
            return measurement.MoistureLevel < 750;
        }

        private bool MustWater(Measurement measurement)
        {
            return measurement.MoistureLevel < 250;
        }

        private bool IsItGoingToRain()
        {
            // Because we are in the UK ;-)
            return false;
        }
    }
}