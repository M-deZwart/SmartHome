#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>

// GPIO12 pin is D5
#define DHTPIN 14
#define DHTTYPE 11

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";
const char* dns = "smarthome@NBNL865.rademaker.nl";
const int serverPort = 5233;

DHT dht(DHTPIN, DHTTYPE);

WiFiClient wifiClient;
HTTPClient http;

void setup() {
  Serial.begin(9600);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");

  dht.begin();
}

void loop() {
  int chk = dht.read();
  Serial.println(chk);

  double temperature = dht.readTemperature();
  double humidity = dht.readHumidity();

  if (!isnan(temperature) && !isnan(humidity)) {
    Serial.print("Humidity (%): ");
    Serial.println(humidity, 2);

    Serial.print("Temperature (C): ");
    Serial.println(temperature, 2);

    sendSensorData(temperature, humidity);
    delay(5000);
  } else {
    Serial.println("DHT11 sensor error!");
  }
}

void sendSensorData(double temperature, double humidity) {
  String serverUrlT = "http://" + String(dns) + ":" + String(serverPort) + "/api/temperature/setTemperature";
  String serverUrlH = "http://" + String(dns) + ":" + String(serverPort) + "/api/humidity/setHumidity";

  // send temperature
  http.begin(serverUrlT);
  http.addHeader("Content-Type", "application/json");

  int httpResponseCodeT = http.POST(String(temperature));
  responseCode(httpResponseCodeT);
  http.end();

  // send humidity
  http.begin(serverUrlH);
  http.addHeader("Content-Type", "application/json");

  int httpResponseCodeH = http.POST(String(humidity));
  responseCode(httpResponseCodeH);
  http.end();
}

void responseCode(int httpResponseCode) {
  if (httpResponseCode >= 200 && httpResponseCode <= 299) {
    String response = http.getString();
    Serial.println(httpResponseCode);
    Serial.println(response);
  } else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }
}
