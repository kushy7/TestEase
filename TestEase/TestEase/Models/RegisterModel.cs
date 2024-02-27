//using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestEase.Models
{
    public abstract class RegisterModel(int address, RegisterType type, string name)
    {
        public int Address { get; set; } = address;
        public RegisterType Type { get; set; } = type;
        public string Name { get; set; } = name;
        public short LastValue { get; set; } = 0;

    }

    public class CoilOrDiscrete(int address, RegisterType type, string name, bool value) : RegisterModel(address, type, name)
    {
        public bool value = value;
    }

    public class Fixed<T>(int address, RegisterType type, string name, T value) : RegisterModel(address, type, name)
    {
        public T value = value;
        public bool isFloat = false;
    }


    public class Range<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : RegisterModel(address, type, name)
    {
        public T startValue = startValue;
        public T endValue = endValue;
        public bool isFloat = isFloat;
    }

    public class Random<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : Range<T>(address, type, name, startValue, endValue, isFloat)
    {
    }

    public class Curve<T> : Range<T>
    {
        private int _iterationStep;

        public int Period { get; }

        public Curve(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat, int intervalStep, int period)
            : base(address, type, name, startValue, endValue, isFloat)
        {
            _iterationStep = 0; // Initialize iterationStep
            Period = period;
        }

        public void IncrementIterationStep()
        {
            _iterationStep++;
            if (_iterationStep >= Period)
            {
                _iterationStep = 0; // Reset iterationStep to 0
            }
        }

        public int GetIterationStep()
        {
            return _iterationStep;
        }
    }
}