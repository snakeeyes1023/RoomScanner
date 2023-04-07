#pragma once

#include <Arduino.h>
#include "ScannerContext.h"
#include "UltrasonicResult.h"
#include "ScanResult.h"
#include <iostream>
#include <vector>

/**
 * @brief Contexte du scanner
 *
 */
namespace RoomScanner
{
    class Scanner
    {
    public:
        Scanner(ScannerContext context);

        /**
         * @brief Calibre le scanner en faisant un scan de la pièce
         * 
         */
        void calibrate();

        /**
         * @brief Fait un scan de la pièce. Appele
         * 
         */
         bool verifyIntrusion();

    private:
        ScannerContext context; // contexte du scanner

        int currentAngle = 0; // angle courant du servo-moteur        

        /**
         * Lecture de la distance avec le capteur ultrasonique
         * @author Jonathan Côté
         * Inspiré de Christiane Lagacé <christiane.lagace@hotmail.com>
         * @return la distance en cm
         */
        UltrasonicResult getDistanceWithCurrentPos();

        /**
         * @brief Configure le servo-moteur
         *
         */
        void configureServo();

        /**
         * @brief Scanne la pièce
         *
         * @return ScanResult
         */
        ScanResult scan();        
    };
}