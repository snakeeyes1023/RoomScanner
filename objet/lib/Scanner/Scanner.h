#pragma once

#include <Arduino.h>
#include "ScannerContext.h"
#include "UltrasonicResult.h"
#include "ScanResult.h"
#include <iostream>
#include <vector>
#include <Servo.h>

/**
 * @brief Contexte du scanner
 *
 */
namespace RoomScanner
{
    class Scanner
    {
    public:
        Scanner(ScannerContext context, Servo &servo);

        /**
         * @brief Calibre le scanner en faisant un scan de la pièce
         *
         */
        void calibrate();

        /**
         * @brief Fait un scan de la pièce. Appele
         *
         */
        int getMaximalVariation(ScanResult &scanResult);

        /**
         * @brief Scanne la pièce
         *
         * @return ScanResult
         */
        ScanResult scan();

    private:
        ScannerContext context; // contexte du scanner

        Servo servo; // servo-moteur

        int currentAngle = 0; // angle courant du servo-moteur

        /**
         * Lecture de la distance avec le capteur ultrasonique
         * @author Jonathan Côté
         * Inspiré de Christiane Lagacé <christiane.lagace@hotmail.com>
         * @return la distance en cm
         */
        UltrasonicResult getDistanceWithCurrentPos();

        /**
         * @brief Déplace le servo à l'angle spécifié
         *
         * @param angle
         */
        void moveServoTo(int angle);
    };
}