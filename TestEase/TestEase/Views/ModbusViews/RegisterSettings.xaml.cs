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
        var register = vm.SelectedRegister;
        if (register.RegisterType == RegisterType.Coil || register.RegisterType == RegisterType.DiscreteInput)
        {
            switch (register.RegisterType)
            {
                case RegisterType.DiscreteInput:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedBooleanValue));
                    vm.DiscreteInputs[register.Address - 1].Value = vm.SelectedBooleanValue;
                    vm.DiscreteInputs[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteDiscreteInput(register.Address, vm.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedBooleanValue}", "OK");
                    break;
                case RegisterType.Coil:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedBooleanValue));
                    vm.Coils[register.Address - 1].Value = vm.SelectedBooleanValue;
                    vm.Coils[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteCoil(register.Address, vm.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedBooleanValue}", "OK");
                    break;
            }
            return; // this is dumb
        }

        // Fixed
        if (FixedRadioButton.IsChecked && FixedFloatConfiguration.IsChecked && float.TryParse(FixedValueEntry.Text, out float x) && FixedValueEntry.Text.Contains('.'))
        {
            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(x);
            short lowBits = lowHighBits[0];
            short highBits = lowHighBits[1];

            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, lowBits));
                    vm.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address + 1, register.RegisterType, NameEntry.Text, highBits));
                    vm.HoldingRegisters[register.Address].Value = highBits;
                    vm.HoldingRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);

                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {x} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, lowBits));
                    vm.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address + 1, register.RegisterType, NameEntry.Text, highBits));
                    vm.InputRegisters[register.Address].Value = highBits;
                    vm.InputRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {x} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
            }

        }
        else if (RangeRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked && float.TryParse(lowerrange.Text, out float lrf) && float.TryParse(upperrange.Text, out float urf) && lowerrange.Text.Contains('.') && upperrange.Text.Contains('.'))
        {
            float randomValue = ValueGenerators.GenerateRandomValueFloat(lrf, urf);
            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(randomValue);
            short lowBits = lowHighBits[0];
            short highBits = lowHighBits[1];
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Random<float>(register.Address, register.RegisterType, NameEntry.Text, lrf, urf, true));
                    vm.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.HoldingRegisters[register.Address].Value = highBits;
                    vm.HoldingRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Random<float>(register.Address, register.RegisterType, NameEntry.Text, lrf, urf, true));
                    vm.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.InputRegisters[register.Address].Value = highBits;
                    vm.InputRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
            }
        }
        else if (FixedRadioButton.IsChecked && short.TryParse(FixedValueEntry.Text, out short n) && !FixedFloatConfiguration.IsChecked)
        {
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.HoldingRegisters[register.Address - 1].Value = n;
                    vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
                case RegisterType.InputRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.InputRegisters[register.Address - 1].Value = n;
                    vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
            }
        }
        else if (RangeRadioButton.IsChecked && RandomRadioButton.IsChecked && !RangeFloatConfigurationCheck.IsChecked )
        {
            if (short.TryParse(lowerrange.Text, out short lr) && short.TryParse(upperrange.Text, out short ur))
            {
                short randomValue = ValueGenerators.GenerateRandomValueShort(lr, ur);
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur, false));
                        vm.HoldingRegisters[register.Address - 1].Value = randomValue;
                        vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur, false));
                        vm.InputRegisters[register.Address - 1].Value = randomValue;
                        vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue}", "OK");
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

    

