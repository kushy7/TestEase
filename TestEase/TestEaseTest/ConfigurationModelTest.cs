using EasyModbus;
using System.Runtime.CompilerServices;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.Services;
using Xunit.Sdk;

namespace TestEaseTest
{
    public class ConfigurationModelTest
    {

        [Fact]
        public void ConfigurationModelWithParameter()
        {
            string expectedName = "Test Config";
            ConfigurationModel configuration = new ConfigurationModel(expectedName);
            Assert.Equal(expectedName, configuration.Name);
        }

    }
}