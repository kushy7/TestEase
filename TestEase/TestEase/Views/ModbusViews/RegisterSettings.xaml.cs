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
                    PeriodEntry.IsVisible = false;
                    break;
                case "Range":
                    //show the register setting for range
                    CurveConfiguration.IsVisible = false;
                    RangeFloatConfiguration.IsVisible = true;
                    LowerValueInput.IsVisible = false;
                    UpperValueInput.IsVisible = false;
                    RandomRadioButton.IsVisible = true;
                    CurveRadioButton.IsVisible = true;
                    PeriodEntry.IsVisible = false;
                    // Hide the fixed value components
                    FloatConfiguration.IsVisible = false;
                    ValueInput.IsVisible = false;
                    break;
                case "Random":
                    CurveConfiguration.IsVisible = false;
                    // Hide other configurations
                    FloatConfiguration.IsVisible = false;
                    ValueInput.IsVisible = false;
                    LowerValueInput.IsVisible = true;
                    UpperValueInput.IsVisible = true;
                    StartingValueInput.IsVisible = false;
                    EndingValueInput.IsVisible = false;
                    PeriodEntry.IsVisible = false;
                    RandomRadioButton.IsVisible = true;
                    CurveRadioButton.IsVisible = true;
                    break;
                case "Curve":
                    // Show the Curve configuration
                    CurveConfiguration.IsVisible = true;
                    // Hide other configurations
                    FloatConfiguration.IsVisible = false;
                    ValueInput.IsVisible = false;
                    LowerValueInput.IsVisible = false;
                    UpperValueInput.IsVisible = false;
                    StartingValueInput.IsVisible = true;
                    EndingValueInput.IsVisible = true;
                    PeriodEntry.IsVisible = true;
                    RandomRadioButton.IsVisible = true;
                    CurveRadioButton.IsVisible = true;
                    break;
            }
        }
    }

    private void OnSaveButtonClick(object sender, EventArgs args)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var register = vm.SelectedServer.SelectedRegister;

        if (register.RegisterType == RegisterType.Coil || register.RegisterType == RegisterType.DiscreteInput)
        {
            switch (register.RegisterType)
            {
                case RegisterType.DiscreteInput:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteDiscreteInput(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedServer.SelectedBooleanValue}", "OK");
                    break;
                case RegisterType.Coil:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.Coils[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.Coils[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteCoil(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedServer.SelectedBooleanValue}", "OK");
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
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address + 1, register.RegisterType, NameEntry.Text, highBits));
                    vm.SelectedServer.HoldingRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.HoldingRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);

                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {x} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, lowBits));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address + 1, register.RegisterType, NameEntry.Text, highBits));
                    vm.SelectedServer.InputRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.InputRegisters[register.Address].Name = NameEntry.Text;
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
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.HoldingRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.HoldingRegisters[register.Address].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Random<float>(register.Address, register.RegisterType, NameEntry.Text, lrf, urf, true));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.InputRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.InputRegisters[register.Address].Name = NameEntry.Text;
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
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
                case RegisterType.InputRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
            }
        }
        else if (RangeRadioButton.IsChecked && RandomRadioButton.IsChecked && !RangeFloatConfigurationCheck.IsChecked)
        {
            if (short.TryParse(lowerrange.Text, out short lr) && short.TryParse(upperrange.Text, out short ur))
            {
                short randomValue = ValueGenerators.GenerateRandomValueShort(lr, ur);
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur, false));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = randomValue;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur, false));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = randomValue;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue}", "OK");
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
        }
        else if (RangeRadioButton.IsChecked && CurveRadioButton.IsChecked)
        {
            if (short.TryParse(startval.Text, out short lowerR) && short.TryParse(endval.Text, out short upperR)
                && int.TryParse(PeriodEntry.Text, out int periodR))
            {
                short nextValue = ValueGenerators.GenerateNextSinValue(lowerR, upperR, 0, periodR);
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, false, 0, periodR));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = nextValue;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, nextValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{nextValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, false, 0, periodR));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = nextValue;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, nextValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{nextValue}", "OK");
                        break;

                    default:
                        Application.Current.MainPage.DisplayAlert("Error", "Invalid register type for Curve value.", "OK");
                        break;
                }
            }

        }
        else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Incomplete settings.", "OK");
        }

    }
 }


    

