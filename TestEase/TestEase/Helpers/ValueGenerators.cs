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
            return (float) random.NextDouble() * (upperBound - lowerBound) + lowerBound;
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

    }
}
