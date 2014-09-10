using System;
using System.Text;
using Gadgeteer.Networking;
using GardenSprinkler.Domain;
using GardenSprinkler.Interfaces;
using Microsoft.SPOT;

namespace GardenSprinkler.Services
{
    /// <summary>
    /// Responsible for posting measurements and status messages to Tinamous
    /// to allow remote monitoring of the soil/watering.
    /// </summary>
    class TinamousService : IInternetOfThingsService
    {
        private const string UserName = "Sprinkler";
        private const string Password = "Passw0rd";
        private const string UrlBase = "http://demo.Tinamous.com/Api/V1/";

        public void GetGoogle()
        {
            const string url = "http://www.Google.com";
            HttpRequest request = HttpHelper.CreateHttpGetRequest(url);
            request.ResponseReceived += ResponseReceived;
            request.SendRequest();
        }

        public void PostMeasurements(Measurement measurement)
        {
            Debug.Print("Posting measurements to Tinamous");
            const string url = UrlBase+ "Measurements";
            POSTContent postContent = BuildMeasurementsPostBody(measurement);
            const string contentType = "application/json";

            HttpRequest postRequest = HttpHelper.CreateHttpPostRequest(url, postContent, contentType);
            AddAuthorizationHeaders(postRequest);
            postRequest.ResponseReceived += ResponseReceived;
            postRequest.SendRequest();
        }

        public void PostStatus(string message)
        {
            Debug.Print("Posting status to Tinamous");
            const string url = UrlBase + "Status";
            POSTContent postContent = BuildStatusPostBody(message);
            const string contentType = "application/json";

            HttpRequest postRequest = HttpHelper.CreateHttpPostRequest(url, postContent, contentType);
            AddAuthorizationHeaders(postRequest);
            postRequest.ResponseReceived += ResponseReceived;
            postRequest.SendRequest();
        }

        private static void ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            Debug.Print("Network Response");
            Debug.Print("Status Code: " + response.StatusCode);
            //Debug.Print("Text: " + response.Text);
        }

        private static POSTContent BuildMeasurementsPostBody(Measurement measurement)
        {
            string postContent = "{ ";
            postContent += "\"Field1\" : \"" + measurement.MoistureLevel + "\", ";
            postContent += "\"Field2\" : \"" + measurement.LightLevel + "\", ";
            postContent += "\"Channel\" : \"0\", ";
            postContent += "\"Lite\" : \"true\" ";
            postContent += "}";

            return POSTContent.CreateTextBasedContent(postContent);
        }

        private POSTContent BuildStatusPostBody(string message)
        {
            string postContent = "{ ";
            postContent += "\"Message\" : \"" +message + "\", ";
            postContent += "\"Lite\" : \"true\" ";
            postContent += "}";

            return POSTContent.CreateTextBasedContent(postContent);
        }

        private static void AddAuthorizationHeaders(HttpRequest getRequest)
        {
            var headerValue = GetAuthHeaderValue(UserName, Password);
            getRequest.AddHeaderField("Authorization", headerValue);
        }

        private static string GetAuthHeaderValue(string user, string password)
        {
            string userPassword = user + ":" + password;
            byte[] bytes = Encoding.UTF8.GetBytes(userPassword);
            return "Basic " + Convert.ToBase64String(bytes);
        }
    }
}
