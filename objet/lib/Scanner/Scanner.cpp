#include "Scanner.h"

namespace RoomScanner
{
    Scanner::Scanner(RoomScanner::ScannerContext context, Servo &servo)
    {
        this->context = context;
        this->servo = servo;
        this->servo.write(0);
    }

    ScanResult Scanner::scan()
    {
        int angleStep = 180 / context.step;

        ScanResult scanResult = ScanResult();

        moveServoTo(0);

        for (int i = 0; i < context.step; i++)
        {
            // déplace le servo à l'angle suivant
            moveServoTo(currentAngle + angleStep);

            // lit la distance avec le capteur ultrasonique et l'ajoute au résultat
            UltrasonicResult result = this->getDistanceWithCurrentPos();
            scanResult.addResult(result);
        }

        moveServoTo(90);

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

    void Scanner::moveServoTo(int angle)
    {
        this->servo.write(angle);
        this->currentAngle = angle;
        delay(300);
    }

    int Scanner::getMaximalVariation(ScanResult &scanResult)
    {
        int max = 0;

        for (int i = 0; i < scanResult.results.size(); i++)
        {
            UltrasonicResult current = scanResult.results[i];

            // parcoure la distance initiale pour cet angle
            for (int j = 0; j < context.initialResults.results.size(); j++)
            {
                UltrasonicResult initial = context.initialResults.results[j];
                if (initial.angle == current.angle)
                {
                    // calcule la différence entre les deux distances
                    int distance = current.distance - initial.distance;

                    if (abs(distance) > max)
                    {
                        max = abs(distance);
                    }
                }
            }
        }

        return max;
    }
}