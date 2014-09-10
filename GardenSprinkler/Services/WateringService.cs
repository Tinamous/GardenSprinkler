using System;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using GardenSprinkler.Interfaces;

namespace GardenSprinkler.Services
{
    public class WateringService : IWateringService
    {
        private readonly RelayX1 _relayX1;
        private readonly IInternetOfThingsService _internetOfThingsService;
        private Timer _wateringTimer;

        public WateringService(RelayX1 relayX1, IInternetOfThingsService internetOfThingsService)
        {
            _relayX1 = relayX1;
            _internetOfThingsService = internetOfThingsService;

            SetupWateringTimer();
        }

        public void Water()
        {
            _wateringTimer.Start();
            _relayX1.TurnOn();
            _internetOfThingsService.PostStatus("Watering.");
        }

        void wateringTimer_Tick(Timer timer)
        {
            // Stop watering
            _relayX1.TurnOff();
            _wateringTimer.Stop();
            _internetOfThingsService.PostStatus("Stopped watering.");
        }

        /// <summary>
        /// Setup the timer for how long the pump should run for.
        /// </summary>
        private void SetupWateringTimer()
        {
            // How long the watering should last.
            _wateringTimer = new Timer(new TimeSpan(0, 0, 0, 2));
            _wateringTimer.Tick += wateringTimer_Tick;
        }
    }
}