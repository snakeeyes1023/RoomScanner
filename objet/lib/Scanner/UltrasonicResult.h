#pragma once

/**
 * @brief Résultat d'un scan (angle et distance)
 * 
 * @param distance Distance en cm
 * @param angle Angle en degrés
 */

namespace RoomScanner
{
    struct UltrasonicResult
    {
        int distance;
        int angle;
    };
}