using GardenSprinkler.Domain;

namespace GardenSprinkler.Interfaces
{
    public interface IMeasurementService
    {
        Measurement GetCurrentMeasurement();
    }
}