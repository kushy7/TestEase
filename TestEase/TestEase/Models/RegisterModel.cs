using System;

namespace TestEase.Models
{
    

    public abstract class RegisterModel
    {
        public int Address { get; set; }
        public RegisterType Type { get; set; }
        public string Name { get; set; }
        public short LastValue { get; set; }

        protected RegisterModel(int address, RegisterType type, string name)
        {
            Address = address;
            Type = type;
            Name = name;
        }
    }

    public class CoilOrDiscrete : RegisterModel
    {
        public bool Value { get; set; }

        public CoilOrDiscrete(int address, RegisterType type, string name, bool value)
            : base(address, type, name)
        {
            Value = value;
        }
    }

    public class Fixed<T> : RegisterModel
    {

        public T value = value;
        public bool isFloat = false;
    }


        public Fixed(int address, RegisterType type, string name, T value)
            : base(address, type, name)
        {
            Value = value;
        }
    }

    public class Range<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : RegisterModel(address, type, name)
    {
        public T startValue = startValue;
        public T endValue = endValue;
        public bool isFloat = isFloat;
    }

    public class Random<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : Range<T>(address, type, name, startValue, endValue, isFloat)

    {
        public Random(int address, RegisterType type, string name, T startValue, T endValue)
            : base(address, type, name, startValue, endValue)
        {
        }
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
