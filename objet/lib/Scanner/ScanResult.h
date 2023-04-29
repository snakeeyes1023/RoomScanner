#pragma once
#include <iostream>
#include <vector>
#include "UltrasonicResult.h"
#include <Arduino.h>

/**
 * @brief Results d'un scan complet (tous les angles)
 * 
 * @param results Vecteur de UltrasonicResult
 */
namespace RoomScanner
{
    struct ScanResult
    {
        std::vector<UltrasonicResult> results;

        ScanResult(std::vector<UltrasonicResult> results)
        {
            this->results = results;
        }

        ScanResult()
        {
            this->results = std::vector<UltrasonicResult>();
        }

        /**
         * @brief Ajoute un UltrasonicResult au vecteur de résultats
         * 
         * @param result 
         */
        void addResult(UltrasonicResult result)
        {
            this->results.push_back(result);
        }

        /**
         * @brief Impression des résultats dans le moniteur série
         * 
         */
        void print()
        {
            // print the results in serial
            for (int i = 0; i < this->results.size(); i++)
            {
                Serial.print("Angle: ");
                Serial.print(this->results[i].angle);
                Serial.print(" Distance: ");
                Serial.println(this->results[i].distance);                
            }
        }
    };  
}