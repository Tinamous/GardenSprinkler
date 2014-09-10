using System;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using GardenSprinkler.Domain;
using GardenSprinkler.Interfaces;
using Microsoft.SPOT;

namespace GardenSprinkler.Services
{
    /// <summary>
    /// Responsible for getting the latest measurements.
    /// </summary>
    public class MeasurementService : IMeasurementService
    {
        private readonly Moisture _moistureSensor;
        private readonly LightSense _lightSensor;
        private readonly IInternetOfThingsService _internetOfThingsService;
        private Timer _measurementsTimer;
        private Measurement _currentMeasurement;
        private int _postCount;

        public MeasurementService(Moisture moistureSensor, LightSense lightSensor, IInternetOfThingsService internetOfThingsService)
        {
            _moistureSensor = moistureSensor;
            _lightSensor = lightSensor;
            _internetOfThingsService = internetOfThingsService;

            // Measure every second.
            _measurementsTimer = new Timer(new TimeSpan(0, 0, 0, 1));
            _measurementsTimer.Tick += measurementsTimer_Tick;
            _measurementsTimer.Start();
        }

        public Measurement GetCurrentMeasurement()
        {
            return _currentMeasurement;
        }

        void measurementsTimer_Tick(Timer timer)
        {
            //Debug.Print("Measuring moisture and light levels");

            _currentMeasurement = new Measurement
            {
                MoistureLevel = _moistureSensor.ReadMoisture(),
                LightLevel = _lightSensor.ReadVoltage()
            };

            Debug.Print(_currentMeasurement.ToString());

            PostMeasurement();
        }

        private void PostMeasurement()
        {
            _postCount++;

            // Post measurements only every xth measurement
            // allows for rapid internal update without overloading 
            // web service.
            if (_postCount >= 2)
            {
                try
                {
                    var currentMeasurement = GetCurrentMeasurement();
                    if (currentMeasurement != null)
                    {
                        _internetOfThingsService.PostMeasurements(currentMeasurement);
                    }
                }
                catch (Exception ex)
                {
                    // Sink the exception
                    Debug.Print("Exception posting average measurement: " + ex.Message);
                }
                finally
                {
                    _postCount = 0;
                }
            }
        }
    }
}