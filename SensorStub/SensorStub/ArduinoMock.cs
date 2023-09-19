using System.Net.Http.Json;
using System.Text;

namespace SensorStub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var arduinoMock = new ArduinoMock();
            await arduinoMock.Run();
        }
    }

    class ArduinoMock
    {
        private readonly HttpClient _client;
        private readonly string _temperatureEndpoint = "temperature/setTemperature";
        private readonly string _humidityEndpoint = "humidity/setHumidity";
        private readonly string _sensorTitle = "LivingRoom";

        public ArduinoMock()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://NBNL865.rademaker.nl:5233/api/");
        }

        public async Task Run()
        {
            while (true)
            {
                double temperature = 12;
                double humidity = 25;

                await SendSensorData(_temperatureEndpoint, temperature, _sensorTitle);
                await SendSensorData(_humidityEndpoint, humidity, _sensorTitle);

                await Task.Delay(5000);
            }
        }

        private async Task SendSensorData(string serverUrl, double sensorValue, string sensorTitle)
        {        
            try
            {
                var apiUrl = $"{serverUrl}/{sensorTitle}";

                HttpResponseMessage response = await _client.PostAsJsonAsync(apiUrl, sensorValue);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Data sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

