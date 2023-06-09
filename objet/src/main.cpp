#pragma once

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
#include <SD.h>

char ssid[] = "mesloc";
char pass[] = "8199981542";
int status = WL_IDLE_STATUS;

WiFiClient wifi;
WiFiServer server(80);

// ip du PI
char ip[] = "172.20.10.2";
int port = 45456;

HttpClient client = HttpClient(wifi, ip, port);

RoomScanner::Scanner *scanner;

Servo servo;

File scannerDataFile;
char *scannerDataFileName = "capteur.txt";

/**
 * define the pins
 */
#define TRIG_PIN 7
#define ECHO_PIN 6
#define SERVO_PIN 9
#define LUMINOSITY_PIN A0
#define SD_CARD_PIN 5

unsigned long scanDelayMillis = 1000 * 60 * 5;
unsigned long lastScanMillis = 0;

bool boxIsOpen = false;
int luminosityOpenState = 900;

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
 * @brief Enregistre le scan (json) dans le fichier capteur.txt
 */
void saveScanInFile(String scanInJson)
{
  scannerDataFile = SD.open(scannerDataFileName, FILE_WRITE);

  if (scannerDataFile)
  {
    scannerDataFile.println(scanInJson);
    Serial.println(scanInJson);
    scannerDataFile.close();
  }
  else
  {
    Serial.println("Impossible d'ouvrir le fichier.");
  }
}

/**
 * Envoie la valeur valeur au PI
 * @author Jonathan Côté
 * Inspiré de Christiane Lagacé <christiane.lagace@hotmail.com>
 */
bool sendScanToPI(String json)
{
  // tentative d'enregistrement du scan
  String contentType = "application/json";

  client.beginRequest();
  client.post("/api/scans/add");
  client.sendHeader("Content-Type", contentType);
  client.sendHeader("Content-Length", json.length());
  client.beginBody();
  client.print(json);
  client.endRequest();

  int statusCode = client.responseStatusCode();
  String response = client.responseBody();
  Serial.println(response);

  return statusCode == 200;
}

/**
 * @brief Synchronise le fichier capteur.txt avec le PI
 */
void syncScanFile()
{
  scannerDataFile = SD.open(scannerDataFileName, FILE_READ);

  if (scannerDataFile)
  {
    while (scannerDataFile.available())
    {
      String line = scannerDataFile.readStringUntil('\n');

      if (!sendScanToPI(line))
      {
        scannerDataFile.close();
        return;
      }
    }
    scannerDataFile.close();
    SD.remove(scannerDataFileName);
  }
  else
  {
    Serial.println("Impossible d'ouvrir le fichier");
  }
}

/**
 * @brief Envoie une requête au PI pour indiquer que le scanner a été infiltré
 *
 */
bool sendInfiltrationToPI()
{
  // tentative d'enregistrement du scan
  String contentType = "application/json";

  client.beginRequest();
  client.post("/api/scans/infiltrations");
  client.sendHeader("Content-Type", contentType);
  client.sendHeader("Content-Length", 2);
  client.beginBody();
  client.print("{}");
  client.endRequest();

  int statusCode = client.responseStatusCode();
  String response = client.responseBody();
  Serial.println(response);

  return statusCode == 200;
}

/**
 * @brief Vérifie si le scanner a été infiltré grace à la luminosité
 *
 * @return true  si le scanner a été infiltré
 * @return false  si le scanner n'a pas été infiltré
 */
bool getCurrentBoxState()
{
  int luminosity = analogRead(LUMINOSITY_PIN);
  return luminosity > luminosityOpenState;
}

/**
 * @brief Vérifie si le PI a envoyé une requête pour lancer un scan
 *
 * @return true  si le PI a envoyé une requête pour lancer un scan
 * @return false  si le PI n'a pas envoyé de requête pour lancer un scan
 */
bool hasReceivedScanRequest()
{
  WiFiClient client = server.available();
  bool triggerScan = false;

  if (client)
  {
    String currentLine = "";

    while (client.connected())
    {
      if (client.available())
      {
        char c = client.read();

        if (c == '\n')
        {
          if (currentLine.length() == 0)
          {
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/html");
            client.println("Connection: close");
            client.println("Content-Length: 2");
            client.println("\r\nok");
            break;
          }
          else
          {
            currentLine = "";
          }
        }
        else if (c != '\r')
        {
          currentLine += c;
        }

        if (currentLine.endsWith("POST /trigger-scan"))
        {
          triggerScan = true;
        }
      }
    }

    client.flush();
    client.stop();
  }

  return triggerScan;
}

/**
 * @brief Lance un scan et envoie les données au PI
 **/
void startScan()
{
  RoomScanner::ScanResult scanResult = scanner->scan();

  // preparation du json de scan
  DynamicJsonDocument doc(2048);
  doc["MaximalVariationDistance"] = scanner->getMaximalVariation(scanResult);
  String json = "";
  serializeJson(doc, json);

  if (sendScanToPI(json))
  {
    syncScanFile();
  }
  else
  {
    Serial.print("(SCAN) Le PI n'est pas disponible : ");
    saveScanInFile(json);
    Serial.println("Scan enregistré dans le fichier");
  }
}

void setup()
{
  Serial.begin(9600);

  servo.attach(SERVO_PIN);

  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  if (!SD.begin(SD_CARD_PIN))
  {
    Serial.println("Impossible de communiquer avec le lecteur de carte.");
    while (1)
      ;
  }

  // branchement au Wi-Fi
  while (status != WL_CONNECTED)
  {
    Serial.print("Tentative de connexion au réseau : ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass);
  }

  server.begin();

  printWifiStatus();

  RoomScanner::ScannerContext context(TRIG_PIN, ECHO_PIN, 5);

  scanner = new RoomScanner::Scanner(context, servo);

  delay(2000);

  // Fait un scan afin de calibrer le scanner
  scanner->calibrate();
}

void loop()
{
  unsigned long millisActu = millis();
  bool delaiDepasse = millisActu - lastScanMillis > scanDelayMillis;

  // si le PI a envoyé une requête pour lancer un scan ou si le délai est dépassé
  if (hasReceivedScanRequest() || delaiDepasse)
  {
    startScan();
    lastScanMillis = millisActu;
  }

  // vérifie si le scanner a été infiltré
  if (getCurrentBoxState() != boxIsOpen)
  {

    boxIsOpen = !boxIsOpen;

    if (boxIsOpen)
    {
      if (!sendInfiltrationToPI())
      {
        Serial.print("(INFILTRATION) Le PI n'est pas disponible : ");
      }
    }
  }
}