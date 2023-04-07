#include "Scanner.h"

namespace RoomScanner
{

    Scanner::Scanner(RoomScanner::ScannerContext context)
    {
        configureServo();
        this->context = context;
    }

    ScanResult Scanner::scan()
    {
        // foreach step, move the servo and get the distance (360 / step)
        int angleStep = 360 / context.step;

        ScanResult scanResult = ScanResult();

        for (int i = 0; i < context.step; i++)
        {
            // move the servo to the next position
            currentAngle += angleStep;

            // get the distance
            UltrasonicResult result = this->getDistanceWithCurrentPos();

            // add the result to the scan result
            scanResult.addResult(result);
        }

        currentAngle = 0;

        scanResult.print();

        return scanResult;
        
    }

    void Scanner::calibrate()
    {
        context.initialResults = this->scan();
    }

    UltrasonicResult Scanner::getDistanceWithCurrentPos()
    {
        // Pour assurer qu'il n'y a pas de signal au départ
        digitalWrite(context.trigPin, LOW);
        delayMicroseconds(2);

        // Envoie un signal
        digitalWrite(context.trigPin, HIGH);
        delayMicroseconds(10);
        digitalWrite(context.trigPin, LOW);

        // lit la réponse du capteur
        int totalTime = pulseIn(context.echoPin, HIGH); // durée en microsecondes. pulseIn() attend que le signal passe de LOW à HIGH puis mesure le temps requis pour qu'il revienne à LOW

        int totalInch = totalTime * 0.034 / 2; // retourne la distance en pouces

        UltrasonicResult result = UltrasonicResult();
        result.distance = totalInch;
        result.angle = currentAngle;
        return result;
    }

    void Scanner::configureServo()
    {
    }

    bool Scanner::verifyIntrusion()
    {
        Serial.println("Verifying intrusion...");
        ScanResult scanResult = this->scan();

        for (int i = 0; i < scanResult.results.size(); i++)
        {
            // get 
            UltrasonicResult current = scanResult.results[i];
            
            // search for the same angle in the initial results
            for (int j = 0; j < context.initialResults.results.size(); j++)
            {
                UltrasonicResult initial = context.initialResults.results[j];
                if (initial.angle == current.angle)
                {
                    // if the distance is greater than the initial distance, then there is an intrusion
                    int distance = current.distance - initial.distance;
                    if (abs(distance) > 10)
                    {
                        Serial.println("Intrusion detected!");
                        Serial.print("Distance: ");
                        Serial.println(distance);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}