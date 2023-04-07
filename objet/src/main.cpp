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

RoomScanner::Scanner *scanner;

/**
 * define the pins
 *
 */
#define TRIG_PIN 7
#define ECHO_PIN 6
#define SERVO_PIN 10

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

void setup()
{
  Serial.begin(9600);
  while (!Serial)
    ;

  pinMode(LED_BUILTIN, OUTPUT);
  blinkLed(LED_BUILTIN, 500, 3);

  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  RoomScanner::ScannerContext context(TRIG_PIN, ECHO_PIN, SERVO_PIN, 5);

  scanner = new RoomScanner::Scanner(context);

  delay(2000);

  scanner->calibrate();
}

void loop()
{
  delay(2000);

  if (scanner->verifyIntrusion())
  {
    blinkLed(LED_BUILTIN, 100, 10);
  }

}