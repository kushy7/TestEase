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

        public static float GenerateRandomValueDouble(float lowerBound, float upperBound)
        {
            return (float) random.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }

    }
}
