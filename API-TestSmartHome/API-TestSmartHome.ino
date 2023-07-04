#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";

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
  String serverUrlT = "http://192.168.2.5:5233/api/temperature/" + String(temperature);
  String serverUrlH = "http://192.168.2.5:5233/api/humidity/" + String(humidity);

  http.begin(wifiClient, serverUrlT);
  int httpResponseCodeT = http.GET();
  responseCode(httpResponseCodeT);
  http.end();

  http.begin(wifiClient, serverUrlH);
  int httpResponseCodeH = http.GET();
  responseCode(httpResponseCodeH);
  http.end();
}

void responseCode(int httpResponseCode) {
  if (httpResponseCode > 0) {
    String response = http.getString();
    Serial.println(httpResponseCode);
    Serial.println(response);
  } else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }
}
