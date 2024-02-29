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

    // [JsonDerivedType(typeof(RegisterModel), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(CoilOrDiscrete), typeDiscriminator: "coilOrDiscrete")]
    [JsonDerivedType(typeof(Fixed<short>), typeDiscriminator: "fixedShort")]
    [JsonDerivedType(typeof(Fixed<float>), typeDiscriminator: "fixedFloat")]
    [JsonDerivedType(typeof(Random<short>), typeDiscriminator: "randomShort")]
    [JsonDerivedType(typeof(Random<float>), typeDiscriminator: "randomFloat")]
    [JsonDerivedType(typeof(Curve<short>), typeDiscriminator: "curveShort")]
    [JsonDerivedType(typeof(Curve<float>), typeDiscriminator: "curveFloat")]
    public abstract class RegisterModel(int address, RegisterType type, string name)
    {
        public int Address { get; set; } = address;
        public RegisterType Type { get; set; } = type;
        public string Name { get; set; } = name;
    }

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