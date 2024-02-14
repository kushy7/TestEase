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

        public static int GenerateRandomValueInt(int lowerBound, int upperBound)
        {
            return random.Next(lowerBound, upperBound + 1);
        }

        public static double GenerateRandomValueDouble(double lowerBound, double upperBound)
        {
            return random.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }

    }
}
