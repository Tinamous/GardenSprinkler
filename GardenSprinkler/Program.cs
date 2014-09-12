using System;
using Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using GardenSprinkler.Interfaces;
using GardenSprinkler.Services;
using Microsoft.SPOT;

namespace GardenSprinkler
{
    public partial class Program
    {
        private IMeasurementService _measurementService;
        private IWateringService _wateringService;
        private WateringRulesEngine _wateringRulesEngine;
        private IInternetOfThingsService _tinamousService;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            //var tinamousService = new TinamousService();
            //_tinamousService = new TinamousMqttService();
            _tinamousService = new NullIoTService();
            _tinamousService.WaterRequested += tinamousService_WaterRequested;

            SetupEthernet();

            _measurementService = new MeasurementService(moisture, lightSense, _tinamousService);

            _wateringService = new WateringService(relayX1, _tinamousService);

            _wateringRulesEngine = new WateringRulesEngine(_measurementService, _wateringService, _tinamousService);
            _wateringRulesEngine.Start();
        }

        void tinamousService_WaterRequested(object sender, EventArgs e)
        {
            Debug.Print("IoT service requested plants be watered");
            _wateringService.Water();
        }

        /// <summary>
        /// Setup the ethernet connection
        /// </summary>
        private void SetupEthernet()
        {
            try
            {
                // Initialise here rather than using designer!
                ethernetENC28 = new EthernetENC28(6);
                ethernetENC28.DebugPrintEnabled = true;
                ethernetENC28.NetworkUp += ethernetENC28_NetworkUp;
                ethernetENC28.NetworkDown += ethernetENC28_NetworkDown;

                ethernetENC28.UseThisNetworkInterface();
                ethernetENC28.NetworkSettings.EnableDhcp();
                // this doesn't appear to work.
                //ethernetENC28.UseDHCP();
            }
            catch (Exception ex)
            {
                Debug.Print("Exception setting up network: " + ex.Message);
            }
        }

        void ethernetENC28_NetworkUp(Module.NetworkModule sender, Module.NetworkModule.NetworkState state)
        {
            if (ethernetENC28.IsNetworkConnected)
            {
                ethernetENC28.UseThisNetworkInterface();

                Debug.Print("*** Network connected!!! ***");
                Debug.Print(ethernetENC28.NetworkInterface.IPAddress);
                Debug.Print(ethernetENC28.NetworkInterface.SubnetMask);
                Debug.Print(ethernetENC28.NetworkInterface.CableConnected ? "Cable Connected" : "No cable");
                Debug.Print(ethernetENC28.NetworkInterface.DnsAddresses[0]);
                Debug.Print(ethernetENC28.NetworkInterface.NetworkInterfaceType.ToString());
                Debug.Print(ethernetENC28.NetworkInterface.NetworkIsAvailable ? "Network available" : "*** Network is not available ***");

                // Allow the network service to start when the network is up.
                _tinamousService.Start();
            }
        }

        void ethernetENC28_NetworkDown(Module.NetworkModule sender, Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down");
            _tinamousService.Stop();
        }
    }
}
