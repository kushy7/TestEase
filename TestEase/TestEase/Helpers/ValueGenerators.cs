using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Helpers
{
    public class ValueGenerators
    {

        private static readonly Random random = new Random();

        public static short GenerateRandomValueShort(short lowerBound, short upperBound)
        {
            return (short) random.Next(lowerBound, upperBound + 1);
        }

        public static float GenerateRandomValueFloat(float lowerBound, float upperBound)
        {
            return (float)(random.NextDouble() * (upperBound - lowerBound) + lowerBound);
        }


        public static short[] GenerateShortArrayFromFloat(float floatInput)
        {
            byte[] floatBytes = BitConverter.GetBytes(floatInput);
            short firstShort = BitConverter.ToInt16(floatBytes, 0);
            short secondShort = BitConverter.ToInt16(floatBytes, 2);
            short[] shortArray = new short[2];
            shortArray[0] = firstShort;
            shortArray[1] = secondShort;
            return shortArray;
        }

        //public static short GetNextSineValue(short currentValue, short startValue, short endValue, int interval)
        //{
        //    // Calculate the amplitude of the sine wave
        //    double amplitude = (endValue - startValue) / 2.0;

        //    // Calculate the midpoint of the sine wave
        //    double midPoint = (startValue + endValue) / 2.0;

        //    // Calculate the angular frequency (omega)
        //    double omega = 2 * Math.PI / interval;

        //    // Calculate the current phase theta based on the current value
        //    // This involves reverse calculation from the sine wave formula: currentValue = midPoint + amplitude * sin(theta)
        //    double theta = Math.Asin((currentValue - midPoint) / amplitude);

        //    // Increment the phase by omega to get the next point
        //    theta += omega;

        //    // Calculate the next value using the sine wave formula
        //    short nextValue = (short)(midPoint + amplitude * Math.Sin(theta));

        //    // Ensure the next value is within the limits
        //    nextValue = (short)Math.Max(startValue, Math.Min(endValue, nextValue));

        //    return nextValue;
        //}

        public static short GenerateNextSinValue(double startValue, double endValue, int iterationStep, int iterationTotal)
        {

            double range = endValue - startValue;
            double stepSize = 2 * Math.PI / iterationTotal;
            double angle = iterationStep * stepSize;

            double sinValue = Math.Sin(angle);
            short result = (short)(startValue + ((sinValue + 1) / 2) * range);

            return result;
        }

        public static float GetNextSineValueFloat(float startValue, float endValue, int iterationStep, int iterationTotal)
        {

            float range = endValue - startValue;
            float stepSize = 2f * (float)Math.PI / iterationTotal;
            float angle = (iterationStep + 1) * stepSize;

            float sinValue = (float)Math.Sin(angle);
            float result = (float) startValue + ((sinValue + 1f) / 2f) * range;

            return result;
        }

        public static short GenerateLinearValue(int current, int start, int end, int increment, ref bool increasing)
        {
            if (increasing)
            {
                current += increment;
                if (current >= end)
                {
                    current = end;
                    increasing = false;
                }
            }
            else
            {
                current -= increment;
                if (current <= start)
                {
                    current = start;
                    increasing = true;
                }
            }
            return (short) current;
        }

        public static float GenerateLinearValueFloat(float current, float start, float end, float increment, ref bool increasing)
        {
            if (increasing)
            {
                current += increment;
                if (current >= end)
                {
                    current = end;
                    increasing = false;
                }
            }
            else
            {
                current -= increment;
                if (current <= start)
                {
                    current = start;
                    increasing = true;
                }
            }
            return current;
        }


    }
}
