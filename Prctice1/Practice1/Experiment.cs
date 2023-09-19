using System;

namespace Practice1
{
    public class Experiment
    {
        /*
        x0, y0 - координаты центра окружности; r0 - радиус окружности
        experimentNumber - число экспериментов для генератора случайных чисел
        */
        public static double CalculatePi(double x0, double y0, double r0, int experimentsNumber)
        {
            double xMin = x0 - r0;
            double xMax = x0 + r0;
            double yMin = y0 - r0;
            double yMax = y0 + r0;
            double numberOfPositiveOutcomes = 0;

            Random random = new Random();

            for (int experiment = 0; experiment < experimentsNumber; experiment++)
            {
                double randomNumber = random.NextDouble();
                double x = (xMax - xMin) * randomNumber + xMin;
                randomNumber = random.NextDouble();
                double y = (yMax - yMin) * randomNumber + yMin;

                bool isInCircle = (x - x0) * (x - x0) + (y - y0) * (y - y0) < r0 * r0;
                
                if (isInCircle)
                {
                    numberOfPositiveOutcomes++;
                }
            }

            double pi = numberOfPositiveOutcomes / experimentsNumber * 4;
            
            return pi;
        }
    }
}