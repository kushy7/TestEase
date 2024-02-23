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
        public T Value { get; set; }

        public Fixed(int address, RegisterType type, string name, T value)
            : base(address, type, name)
        {
            Value = value;
        }
    }

    public class Range<T> : RegisterModel
    {
        public T StartValue { get; set; }
        public T EndValue { get; set; }

        public Range(int address, RegisterType type, string name, T startValue, T endValue)
            : base(address, type, name)
        {
            StartValue = startValue;
            EndValue = endValue;
        }
    }

    public class Random<T> : Range<T>
    {
        public Random(int address, RegisterType type, string name, T startValue, T endValue)
            : base(address, type, name, startValue, endValue)
        {
        }
    }

    public class Curve<T> : Range<T>
    {
        public int Period { get; set; }

        public Curve(int address, RegisterType type, string name, T startValue, T endValue, int period)
            : base(address, type, name, startValue, endValue)
        {
            Period = period;
        }
    }
}
