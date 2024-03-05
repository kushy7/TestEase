using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEase.Models;

namespace TestEaseTest
{
    public class RegisterModelTest
    {
        [Fact]
        public void RangeModelShortInitializationTest()
        {
            // Testing Range model initialization with short values
            Range<short> rangeShort = new Range<short>(10, RegisterType.HoldingRegister, "ShortRangeRegister", 100, 200, false);

            Assert.Equal(10, rangeShort.Address);
            Assert.Equal(RegisterType.HoldingRegister, rangeShort.Type);
            Assert.Equal("ShortRangeRegister", rangeShort.Name);
            Assert.Equal(100, rangeShort.StartValue);
            Assert.Equal(200, rangeShort.EndValue);
            Assert.False(rangeShort.IsFloat);
        }

        [Fact]
        public void RangeModelFloatInitializationTest()
        {
            // Testing Range model initialization with float values
            Range<float> rangeFloat = new Range<float>(20, RegisterType.InputRegister, "FloatRangeRegister", 1.5f, 2.5f, true);

            Assert.Equal(20, rangeFloat.Address);
            Assert.Equal(RegisterType.InputRegister, rangeFloat.Type);
            Assert.Equal("FloatRangeRegister", rangeFloat.Name);
            Assert.Equal(1.5f, rangeFloat.StartValue);
            Assert.Equal(2.5f, rangeFloat.EndValue);
            Assert.True(rangeFloat.IsFloat);
        }

        [Fact]
        public void CurveModelShortInitializationTest()
        {
            // Testing Curve model initialization with short values
            Curve<short> curveShort = new Curve<short>(30, RegisterType.HoldingRegister, "ShortCurveRegister", 100, 200, false, 10);

            Assert.Equal(30, curveShort.Address);
            Assert.Equal(RegisterType.HoldingRegister, curveShort.Type);
            Assert.Equal("ShortCurveRegister", curveShort.Name);
            Assert.Equal(100, curveShort.StartValue);
            Assert.Equal(200, curveShort.EndValue);
            Assert.False(curveShort.IsFloat);
            Assert.Equal(10, curveShort.Period);
            Assert.Equal(0, curveShort.GetIterationStep()); // Initial iteration step should be 0
        }

        [Fact]
        public void RandomModelShortInitializationTest()
        {
            // Testing Random model initialization with short values
            Random<short> randomShort = new Random<short>(100, RegisterType.HoldingRegister, "ShortRandomRegister", 0, 100, false);

            Assert.Equal(100, randomShort.Address);
            Assert.Equal(RegisterType.HoldingRegister, randomShort.Type);
            Assert.Equal("ShortRandomRegister", randomShort.Name);
            Assert.Equal(0, randomShort.StartValue);
            Assert.Equal(100, randomShort.EndValue);
            Assert.False(randomShort.IsFloat);
        }

        [Fact]
        public void RandomModelFloatInitializationTest()
        {
            // Testing Random model initialization with float values
            Random<float> randomFloat = new Random<float>(200, RegisterType.InputRegister, "FloatRandomRegister", 0.0f, 1.0f, true);

            Assert.Equal(200, randomFloat.Address);
            Assert.Equal(RegisterType.InputRegister, randomFloat.Type);
            Assert.Equal("FloatRandomRegister", randomFloat.Name);
            Assert.Equal(0.0f, randomFloat.StartValue);
            Assert.Equal(1.0f, randomFloat.EndValue);
            Assert.True(randomFloat.IsFloat);
        }


        [Fact]
        public void CurveModelFloatInitializationTest()
        {
            // Testing Curve model initialization with float values
            Curve<float> curveFloat = new Curve<float>(40, RegisterType.InputRegister, "FloatCurveRegister", 1.5f, 2.5f, true, 20);

            Assert.Equal(40, curveFloat.Address);
            Assert.Equal(RegisterType.InputRegister, curveFloat.Type);
            Assert.Equal("FloatCurveRegister", curveFloat.Name);
            Assert.Equal(1.5f, curveFloat.StartValue);
            Assert.Equal(2.5f, curveFloat.EndValue);
            Assert.True(curveFloat.IsFloat);
            Assert.Equal(20, curveFloat.Period);
            Assert.Equal(0, curveFloat.GetIterationStep()); // Initial iteration step should be 0
        }

        [Fact]
        public void IncrementIterationStepTest()
        {
            // Testing IncrementIterationStep and reset functionality
            Curve<short> curve = new Curve<short>(50, RegisterType.Coil, "IncrementTestRegister", 500, 1000, false, 5);
            // Increment 5 times to reach the period limit
            for (int i = 0; i < 5; i++)
            {
                curve.IncrementIterationStep();
            }

            // Should reset to 0 after reaching the period
            Assert.Equal(0, curve.GetIterationStep());

            // Increment once more to verify it goes to 1
            curve.IncrementIterationStep();
            Assert.Equal(1, curve.GetIterationStep());
        }

    }
}
