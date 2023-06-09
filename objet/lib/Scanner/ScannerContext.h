#pragma once

#include <iostream>
#include <vector>
#include "ScanResult.h"

/**
 * @brief Contexte du scanner (Les configurations du scanner)
 * 
 * @param trigPin Pin trig du capteur ultrasonique
 * @param echoPin Pin echo du capteur ultrasonique
 * @param step Total des angles à scanner
 */
namespace RoomScanner
{
    struct ScannerContext
    {
    public:
        ScannerContext(int trigPin, int echoPin, int step)
        {
            this->trigPin = trigPin;
            this->echoPin = echoPin;
            this->step = step;

            this->initialResults = ScanResult();
        }

        ScannerContext()
        {
            this->trigPin = 0;
            this->echoPin = 0;
            this->step = 0;

            this->initialResults = ScanResult();
        }

        int trigPin;
        int echoPin;
        int step;

        ScanResult initialResults;
    };
}
