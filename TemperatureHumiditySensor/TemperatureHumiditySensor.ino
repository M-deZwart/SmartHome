#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>

// GPIO12 pin is D5
#define DHTPIN 14
#define DHTTYPE 11

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";

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

  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

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

void sendSensorData(float temperature, float humidity) {
  String serverUrlT = "http://192.168.2.14:5233/api/temperature/" + String(temperature);
  String serverUrlH = "http://192.168.2.14:5233/api/humidity/" + String(humidity);

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
