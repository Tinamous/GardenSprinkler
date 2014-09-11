using GardenSprinkler.Domain;
using GardenSprinkler.Interfaces;
using Microsoft.SPOT;

namespace GardenSprinkler.Services
{
    public class NullIoTService : IInternetOfThingsService
    {
        public event EventHandler WaterRequested;

        public void Start()
        { }

        public void Stop()
        { }

        public void PostStatus(string message)
        { }

        public void PostMeasurements(Measurement currentMeasurement)
        { }
    }
}