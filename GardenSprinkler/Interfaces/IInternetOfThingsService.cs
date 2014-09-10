using GardenSprinkler.Domain;

namespace GardenSprinkler.Interfaces
{
    public interface IInternetOfThingsService
    {
        void PostStatus(string message);
        void PostMeasurements(Measurement currentMeasurement);
    }
}