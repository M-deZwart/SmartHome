
using System.Text;

namespace SensorStub
{
    class ArduinoMock
    {
        private readonly static HttpClient _client = new HttpClient();
        private readonly string _serverUrlT = "http://192.168.2.11:5233/api/setTemperature/";
        private readonly string _serverUrlH = "http://192.168.2.11:5233/api/setHumidity/";

        public async Task Run()
        {
            while (true)
            {
                double temperature = 12;
                double humidity = 25;

                await SendSensorData(_serverUrlT, temperature);
                await SendSensorData(_serverUrlH, humidity);

                await Task.Delay(5000);
            }
        }

        private static async Task SendSensorData(string serverUrl, double sensorValue)
        {
            try
            {
                var content = new StringContent($"value={sensorValue}", Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await _client.PostAsync(serverUrl, content);
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

