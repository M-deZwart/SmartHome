#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";
const char* serverIP = "192.168.2.11";
const int serverPort = 5233;

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
}

void loop() {
  sendSensorData(12, 25);
  delay(5000);
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
