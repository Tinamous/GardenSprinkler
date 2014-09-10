using GardenSprinkler.Domain;
using Microsoft.SPOT;

namespace GardenSprinkler.Interfaces
{
    public interface IInternetOfThingsService
    {
        event EventHandler WaterRequested;

        void Start();
        void Stop();
        void PostStatus(string message);
        void PostMeasurements(Measurement currentMeasurement);
    }
}