using System;
using Microsoft.Maui.Controls;
using TestEase.ViewModels;

namespace TestEase.Views.ModbusViews;

public partial class SavedConfigurations : ContentView
{
    public SavedConfigurations()
    {
        InitializeComponent();
        BindingContext = new SavedConfigurationsViewModel();
    }
}