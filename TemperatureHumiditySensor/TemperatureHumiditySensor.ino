#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>

// GPIO15 pin D8
#define DHTPIN 15
#define DHTTYPE 22
#define INTERNAL_LEDPIN 1

const char* ssid = "H369A8E1A7C";
const char* password = "653C3DDC7A62";
const char* dns = "NBNL865.rademaker.nl";
const int serverPort = 5233;
// LivingRoom, Bedroom, WorkSpace
const String sensorTitle = "LivingRoom";

DHT dht(DHTPIN, DHTTYPE);

WiFiClient wifiClient;
HTTPClient http;

void setup() {
  pinMode(INTERNAL_LEDPIN, OUTPUT);
  Serial.begin(9600);

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");
  blinkInternalLed(3);
  
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
    blinkInternalLed(2);
    delay(5000);
  } else {
    Serial.println("DHT22 sensor error!");
  }
}

void sendSensorData(double temperature, double humidity) {
  String serverUrlT = "http://" + String(dns) + ":" + String(serverPort) + "/api/temperature/setTemperature/" + sensorTitle;
  String serverUrlH = "http://" + String(dns) + ":" + String(serverPort) + "/api/humidity/setHumidity/" + sensorTitle;

  // send temperature
  http.begin(wifiClient, serverUrlT);
  http.addHeader("Content-Type", "application/json");

  int httpResponseCodeT = http.POST(String(temperature));
  responseCode(httpResponseCodeT);
  http.end();

  // send humidity
  http.begin(wifiClient, serverUrlH);
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

void blinkInternalLed(int number) {
    for (int i=0; i <=number; i++){
    digitalWrite(INTERNAL_LEDPIN, LOW);
    delay(1000);
    digitalWrite(INTERNAL_LEDPIN, HIGH);
    delay(1000);
  } 
}
