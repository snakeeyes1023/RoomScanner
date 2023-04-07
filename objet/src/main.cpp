/**
 * @file main.cpp
 * @author Jonathan Côté
 * @brief  Programme qui détecte si des objets sont présents dans une zone (pièce)
 * @version 0.1
 * @date 2023-04-06
 *
 * @copyright Copyright (c) 2023
 *
 */

#include <Arduino.h>
#include "Scanner.h"
#include <Servo.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>
#include <WiFiClient.h>
#include <SPI.h>
#include "ScanResult.h"
#include <ArduinoJson.h>

char ssid[] = "jomysecure";
char pass[] = "jomysecure$";
int status = WL_IDLE_STATUS;

WiFiClient wifi;

// ip du PI
char ip[] = "192.168.0.238";
int port = 9010;

HttpClient client = HttpClient(wifi, ip, port);

RoomScanner::Scanner *scanner;

Servo servo;

/**
 * define the pins
 *
 */
#define TRIG_PIN 7
#define ECHO_PIN 6
#define SERVO_PIN 9

/**
 * @brief Fait clignoter une LED pendant un certain temps et un certain nombre de fois
 *
 * @param pin  Broche de la LED
 * @param delayTime  Temps en millisecondes entre chaque clignotement
 * @param repeat Nombre de fois que la LED clignote
 */
void blinkLed(pin_size_t pin, int delayTime, int repeat = 3)
{
  for (int i = 0; i < repeat; i++)
  {
    digitalWrite(pin, HIGH);
    delay(delayTime);
    digitalWrite(pin, LOW);
    delay(delayTime);
  }
}

/**
 * Affiche les informations de connexion au réseau Wi-Fi
 *
 * Inspiré de Tom Igoe
 */
void printWifiStatus()
{
  // imprime le SSID du réseau auquel vous êtes connecté :
  Serial.print("SSID : ");
  Serial.println(WiFi.SSID());
  // imprime l'adresse IP de votre carte :
  IPAddress ip = WiFi.localIP();
  Serial.print("Adresse IP : ");
  Serial.println(ip);
  // imprime la force du signal reçue :
  long rssi = WiFi.RSSI();
  Serial.print("Force du signal (RSSI) : ");
  Serial.print(rssi);
  Serial.println(" dBm");
  // imprime où aller dans un navigateur :
  Serial.print("Pour voir cette page en action, ouvrez un navigateur à l'adresse suivante : http://");
  Serial.println(ip);
}

/**
 * Envoie la valeur valeur au PI
 * @author Jonathan Côté
 * Inspiré de Christiane Lagacé <christiane.lagace@hotmail.com>
 */
void sendDataToPI(int maximalVariation, bool isLocalEmpty)
{
  DynamicJsonDocument doc(2048);
  doc["isLocalEmpty"] = isLocalEmpty;
  doc["maximalVariationDistance"] = maximalVariation;

  String content_type = "application/json";
  String json = "";

  serializeJson(doc, json);

  client.post("/api/scans", content_type, json);

  // code d'état HTTP
  int etat_http = client.responseStatusCode();
  Serial.print("Code d'état HTTP : ");
  Serial.println(etat_http);

  // response
  String reponse = client.responseBody();
  Serial.print("Réponse : ");
  Serial.println(reponse);
}

void setup()
{
  Serial.begin(9600);
  while (!Serial)
    ;

  servo.attach(SERVO_PIN);

  pinMode(LED_BUILTIN, OUTPUT);
  blinkLed(LED_BUILTIN, 500, 3);

  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  // branchement au Wi-Fi
  while (status != WL_CONNECTED)
  {
    Serial.print("Tentative de connexion au réseau : ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass);
  }

  RoomScanner::ScannerContext context(TRIG_PIN, ECHO_PIN, 5);

  scanner = new RoomScanner::Scanner(context, servo);

  delay(2000);

  scanner->calibrate();
}

void loop()
{
  delay(2000);

  RoomScanner::ScanResult scanResult = scanner->scan();

  int maxVariance = scanner->getMaximalVariation(scanResult);

  bool isLocalEmpty = maxVariance < 10;

  sendDataToPI(maxVariance, isLocalEmpty);
}