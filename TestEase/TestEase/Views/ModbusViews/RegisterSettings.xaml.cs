using TestEase.Models;
using TestEase.ViewModels;
using TestEase.Helpers;

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
        var register = vm.SelectedServer.SelectedRegister;
        // Fixed
        if (FixedFloatConfiguration.IsChecked && float.TryParse(FixedValueEntry.Text, out float x))
        {
            Console.WriteLine("HELLO");
        }
        if (FixedRadioButton.IsChecked && short.TryParse(FixedValueEntry.Text, out short n))
        {
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{n}", "OK");
                    break;
                case RegisterType.DiscreteInput:
                    // short boolShort = vm.SelectedBooleanValue ? (short)1 : (short)0;
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteDiscreteInput(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{BooleanNameEntry.Text}\nValue:{vm.SelectedServer.SelectedBooleanValue}", "OK");
                    break;
                case RegisterType.Coil:
                    // short boolShort = vm.SelectedBooleanValue ? (short)1 : (short)0;
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.Coils[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.Coils[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteCoil(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{BooleanNameEntry.Text}\nValue:{vm.SelectedServer.SelectedBooleanValue}", "OK");
                    break;
            }
        }
        else if (RangeRadioButton.IsChecked && RandomRadioButton.IsChecked)
        {
            if (short.TryParse(lowerrange.Text, out short lr) && short.TryParse(upperrange.Text, out short ur))
            {
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        // Generate a random value within the specified range
                        short randomValue = ValueGenerators.GenerateRandomValueShort(lr, ur);
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = randomValue;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{randomValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        // Generate a random value within the specified range
                        randomValue = ValueGenerators.GenerateRandomValueShort(lr, ur);
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = randomValue;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{randomValue}", "OK");
                        break;
                    default:
                        Application.Current.MainPage.DisplayAlert("Error", "Invalid register type for Range value.", "OK");
                        break;
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Incomplete settings.", "OK");
            }
        }
        else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Incomplete settings.", "OK");
        }
    }
}

    

