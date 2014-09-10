using System;
using System.Text;
using GardenSprinkler.Domain;
using GardenSprinkler.Interfaces;
using Microsoft.SPOT;
using uPLibrary.Networking.M2Mqtt;

namespace GardenSprinkler.Services
{
    /// <summary>
    /// Responsible for posting measurements and status messages to Tinamous
    /// to allow remote monitoring of the soil/watering.
    /// </summary>
    class TinamousMqttService : IInternetOfThingsService, IDisposable
    {
        public event EventHandler WaterRequested;

        // Username needs account suffix for MQTT
        private const string UserName = "Sprinkler.demo";
        private const string Password = "Passw0rd";
        private const string Uri = "demo.tinamous.com";
        private MqttClient _client;

        public void Start()
        {
            try
            {
                _client = new MqttClient(Uri);
                _client.MqttMsgPublishReceived += MqttMsgPublishReceived;

                _client.Connect("Plant1", UserName, Password);

                // Connect with a will to post if we become disconnected.
                //_client.Connect("Plant1", UserName, Password, false, 0, true, "/Tinamous/V1/Status", "Sprinker down!", true, 60);

                if (_client.IsConnected)
                {
                    Debug.Print("Connected to Tinamous MQTT service");
                    _client.Subscribe(new[] { "/Tinamous/V1/Status.To/Sprinkler" }, new byte[] { 0 });
                    PostStatus("Sprinkler Alive!");
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to connect to MQTT service." + ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (_client != null)
                {
                    _client.Disconnect();
                    _client.MqttMsgPublishReceived -= MqttMsgPublishReceived;
                    _client = null;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Failed to connect to MQTT service." + ex.Message);
            }
        }

        public void PostMeasurements(Measurement measurement)
        {
            if (!IsConnected())
            {
                return;
            }

            Debug.Print("Posting measurements to Tinamous");

            string json = BuildMeasurementsJson(measurement);
            byte[] encoded = Encoding.UTF8.GetBytes(json);

            var response = _client.Publish("/Tinamous/V1/Measurements", encoded);
        }

        public void PostStatus(string message)
        {
            if (!IsConnected())
            {
                return;
            }

            Debug.Print("Posting status to Tinamous");

            byte[] encoded = Encoding.UTF8.GetBytes(message);

            var response = _client.Publish("/Tinamous/V1/Status", encoded);
        }

        private static string BuildMeasurementsJson(Measurement measurement)
        {
            string postContent = "{ ";
            postContent += "\"Field1\" : \"" + measurement.MoistureLevel + "\", ";
            postContent += "\"Field2\" : \"" + measurement.LightLevel + "\", ";
            postContent += "\"Channel\" : \"0\" ";
            postContent += "}";

            return postContent;
        }

        private bool IsConnected()
        {
            if (_client == null)
            {
                return false;
            }

            if (!_client.IsConnected)
            {
                return false;
            }

            return true;
        }

        void MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Debug.Print("Received status message from Tinamous: ");
            char[] message = Encoding.UTF8.GetChars(e.Message);
            var statusMessage = new string(message);
            Debug.Print("Status post received: " + statusMessage);

            if (statusMessage.ToLower().IndexOf("#water") > 0)
            {
                // Status post requesting we water.
                RaiseWaterReqested();
            }
        }

        private void RaiseWaterReqested()
        {
            var handler = WaterRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Disconnect();
            }
        }
    }
}
