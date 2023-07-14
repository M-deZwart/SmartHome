#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>

// GPIO12 pin is D5
#define DHTPIN 14
#define DHTTYPE 11

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";
const char* serverIP = "192.168.2.11";
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
  String serverUrlT = "http://" + String(serverIP) + ":" + String(serverPort) + "/api/setTemperature";
  String serverUrlH = "http://" + String(serverIP) + ":" + String(serverPort) + "/api/setHumidity";

  http.begin(serverUrlT);
  int httpResponseCodeT = http.POST("value=" + String(temperature));
  responseCode(httpResponseCodeT);
  http.end();

  http.begin(serverUrlH);
  int httpResponseCodeH = http.POST("value=" + String(humidity));
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
