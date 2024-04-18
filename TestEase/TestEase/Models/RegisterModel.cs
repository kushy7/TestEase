//using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestEase.Models
{

    //this is how the json files for configs knows how to label each register type
    [JsonDerivedType(typeof(CoilOrDiscrete), typeDiscriminator: "coilOrDiscrete")]
    [JsonDerivedType(typeof(Fixed<short>), typeDiscriminator: "fixedShort")]
    [JsonDerivedType(typeof(Fixed<float>), typeDiscriminator: "fixedFloat")]
    [JsonDerivedType(typeof(Random<short>), typeDiscriminator: "randomShort")]
    [JsonDerivedType(typeof(Random<float>), typeDiscriminator: "randomFloat")]
    [JsonDerivedType(typeof(Curve<short>), typeDiscriminator: "curveShort")]
    [JsonDerivedType(typeof(Curve<float>), typeDiscriminator: "curveFloat")]
    [JsonDerivedType(typeof(Linear<short>), typeDiscriminator: "LinearShort")]
    [JsonDerivedType(typeof(Linear<float>), typeDiscriminator: "LinearFloat")]

    //the common fields for all the registers
    public abstract class RegisterModel(int address, RegisterType type, string name)
    {
        public int Address { get; set; } = address;
        public RegisterType Type { get; set; } = type;
        public string Name { get; set; } = name;
        public bool IsPlaying { get; set; } = true;
    }



    //coil and discrete only takes a boolean value
    public class CoilOrDiscrete(int address, RegisterType type, string name, bool value) : RegisterModel(address, type, name)
    {
        public bool Value { get; set; } = value;
    }

    public class Fixed<T>(int address, RegisterType type, string name, T value, bool isFloat) : RegisterModel(address, type, name)
    {
        public T Value { get; set; } = value;
        // public bool isFloat = isFloat;
        public bool IsFloat { get; set; } = isFloat;
    }


    public class Range<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : RegisterModel(address, type, name)
    {
        public T StartValue { get; set; } = startValue;
        public T EndValue { get; set; } = endValue;
        // public bool isFloat = isFloat;
        public bool IsFloat { get; set; } = isFloat;
    }

    public class Random<T>(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat) : Range<T>(address, type, name, startValue, endValue, isFloat)
    {
    }


    //linear increase model that increases up to the upper limit and then back down to the lower limit
    public class Linear<T> : Range<T> where T : IComparable, IConvertible
    {
        public T Increment { get; set; }
        private T currentValue;
        private bool increasing = true;

        public Linear(int address, RegisterType type, string name, T startValue, T endValue, T increment, bool isFloat)
            : base(address, type, name, startValue, endValue, isFloat)
        {
            Increment = increment;
            currentValue = startValue;
        }

        public T GetCurrentValue()
        {
            UpdateValue();
            return currentValue;
        }

        private void UpdateValue()
        {
            dynamic current = currentValue;
            dynamic start = StartValue;
            dynamic end = EndValue;
            dynamic step = Increment;

            if (increasing)
            {
                current += step;
                if (current.CompareTo(end) >= 0)
                {
                    current = end;
                    increasing = false;
                }
            }
            else
            {
                current -= step;
                if (current.CompareTo(start) <= 0)
                {
                    current = start;
                    increasing = true;
                }
            }

            currentValue = (T) current;
        }
    }

    //curve model has a period that determines how fast "one" curve takes to complete
    public class Curve<T> : Range<T>
    {
        private int _iterationStep;

        public int Period { get; set; }

        public Curve(int address, RegisterType type, string name, T startValue, T endValue, bool isFloat, int period)
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