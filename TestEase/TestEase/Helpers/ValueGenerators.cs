using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Helpers
{

    //Value generators that aids in the register settings gernation of linear, random, and curve
    public class ValueGenerators
    {
        //random values 
        private static readonly Random random = new Random();

        public static short GenerateRandomValueShort(short lowerBound, short upperBound)
        {
            return (short) random.Next(lowerBound, upperBound + 1);
        }

        public static float GenerateRandomValueFloat(float lowerBound, float upperBound)
        {
            return (float)(random.NextDouble() * (upperBound - lowerBound) + lowerBound);
        }


        //creates the two arrays needed for converting floats into two shorts for CoreTec
        public static short[] GenerateShortArrayFromFloat(float floatInput)
        {
            byte[] floatBytes = BitConverter.GetBytes(floatInput);
            short firstShort = BitConverter.ToInt16(floatBytes, 0);
            short secondShort = BitConverter.ToInt16(floatBytes, 2);
            short[] shortArray = new short[2];
            //flip these values if you want to flip what registers the values go into (since coretec wants them flipped)
            shortArray[0] = firstShort;
            shortArray[1] = secondShort;
            return shortArray;
        }

        //curve value generation
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

        //linear values
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
