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
        private readonly static HttpClient _client = new HttpClient();
        private readonly string _serverUrlT = "http://192.168.2.11:5233/api/temperature/setTemperature";
        private readonly string _serverUrlH = "http://192.168.2.11:5233/api/humidity/setHumidity";

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
                var payload = sensorValue;
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
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

