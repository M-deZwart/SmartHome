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

        public ArduinoMock()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.2.11:5233/api/");
        }

        public async Task Run()
        {
            while (true)
            {
                double temperature = 12;
                double humidity = 25;

                await SendSensorData(_temperatureEndpoint, temperature);
                await SendSensorData(_humidityEndpoint, humidity);

                await Task.Delay(5000);
            }
        }

        private async Task SendSensorData(string serverUrl, double sensorValue)
        {        
            try
            {           
                HttpResponseMessage response = await _client.PostAsJsonAsync(serverUrl, sensorValue);
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

