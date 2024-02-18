using TestEase.Models;
using TestEase.ViewModels;

namespace TestEase.Views.ModbusViews;

public partial class RegisterSettings : ContentView
{

    public RegisterSettings()
    {
        InitializeComponent();
    }

    private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        // Ensure the sender is a RadioButton and that one of the buttons is pressed
        if (sender is RadioButton rb && rb.IsChecked)
        {
            // Determine which RadioButton was checked
            switch (rb.Content.ToString())
            {
                case "Fixed":
                    // Show the Float and Value box components
                    FloatConfiguration.IsVisible = true;
                    ValueInput.IsVisible = true;
                    // hide the range value components
                    RangeFloatConfiguration.IsVisible = false;
                    LowerValueInput.IsVisible = false;
                    UpperValueInput.IsVisible = false;
                    RandomRadioButton.IsVisible = false;
                    CurveRadioButton.IsVisible = false;
                    break;
                case "Range":
                    //show the register setting for range
                    RangeFloatConfiguration.IsVisible = true;
                    LowerValueInput.IsVisible = true;
                    UpperValueInput.IsVisible = true;
                    RandomRadioButton.IsVisible = true;
                    CurveRadioButton.IsVisible = true;
                    // Hide the fixed value components
                    FloatConfiguration.IsVisible = false;
                    ValueInput.IsVisible = false;
                    break;
            }
        }
    }

    private void OnSaveButtonClick(object sender, EventArgs args)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var register = vm.SelectedRegister;
        // Fixed
        if (FixedRadioButton.IsChecked)
        {
            short.TryParse(FixedValueEntry.Text, out short n);
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.HoldingRegisters[register.Address - 1].Value = n;
                    vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.Service.WriteHoldingRegister(vm.SelectedServer.Port, register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{n}", "OK");
                    break;
            }
        } else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Incomplete settings.", "OK");
        }

    }
}
