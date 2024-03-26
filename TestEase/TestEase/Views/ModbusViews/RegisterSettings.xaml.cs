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
        if (sender is RadioButton rb && rb.IsChecked)
        {
            // Reset visibility for all specific settings
            FloatConfiguration.IsVisible = ValueInput.IsVisible = RangeFloatConfiguration.IsVisible = false;
            LowerValueInput.IsVisible = UpperValueInput.IsVisible = false;
            StartingValueInput.IsVisible = EndingValueInput.IsVisible = PeriodEntry.IsVisible = false;
            LinearConfiguration.IsVisible = false;

            // Hide range-specific radio buttons initially
            LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = false;

            // Handle visibility based on selected mode
            switch (rb.Content.ToString())
            {
                case "Fixed":
                    FloatConfiguration.IsVisible = ValueInput.IsVisible = true;

                    //TODO: bug where the interval/period text still shown when switching from range back to fixed

                    //PeriodEntry.IsVisible = false;
                    break;
                case "Range":
                    RangeFloatConfiguration.IsVisible = true;
                    // Only show these radio buttons under "Range" selection
                    LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = true;
                    break;
                case "Random":
                    LowerValueInput.IsVisible = UpperValueInput.IsVisible = true;
                    RangeFloatConfiguration.IsVisible = true;
                    LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = true;
                    break;
                case "Curve":
                    StartingValueInput.IsVisible = EndingValueInput.IsVisible = PeriodEntry.IsVisible = true;
                    RangeFloatConfiguration.IsVisible = true;
                    LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = true;
                    break;
                case "Linear":
                    LinearConfiguration.IsVisible = true;
                    RangeFloatConfiguration.IsVisible = true;
                    LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = true;
                    break;
            }
        }
    }


    private void OnSaveButtonClick(object sender, EventArgs args)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var register = vm.SelectedServer.SelectedRegister;

        // Find the index of the existing register, if it exists, remove it.
        int index = vm.SelectedServer.WorkingConfiguration.RegisterModels
            .FindIndex(r => r.Address == register.Address && r.Type == register.RegisterType);
        if (index != -1)
        {
            vm.SelectedServer.WorkingConfiguration.RegisterModels.RemoveAt(index);
        }

        if (register.RegisterType == RegisterType.Coil || register.RegisterType == RegisterType.DiscreteInput)
        {
            switch (register.RegisterType)
            {
                case RegisterType.DiscreteInput:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, BooleanNameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.DiscreteInputs[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteDiscreteInput(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedServer.SelectedBooleanValue}", "OK");
                    break;
                case RegisterType.Coil:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, BooleanNameEntry.Text, vm.SelectedServer.SelectedBooleanValue));
                    vm.SelectedServer.Coils[register.Address - 1].Value = vm.SelectedServer.SelectedBooleanValue;
                    vm.SelectedServer.Coils[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteCoil(register.Address, vm.SelectedServer.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {BooleanNameEntry.Text}\nValue: {vm.SelectedServer.SelectedBooleanValue}", "OK");
                    break;
            }
            // Display to user that changes have been made and the config will need saved
            vm.SelectedServer.IsNotSaved = true;
            return; // this is dumb
        }
        else if (FixedRadioButton.IsChecked && FixedFloatConfiguration.IsChecked && float.TryParse(FixedValueEntry.Text, out float x) && FixedValueEntry.Text.Contains('.'))
        {
            // FIXED FLOAT
            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(x);
            short lowBits = lowHighBits[0];
            short highBits = lowHighBits[1];

            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<float>(register.Address, register.RegisterType, NameEntry.Text, x, true));
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.HoldingRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);

                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {x} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<float>(register.Address, register.RegisterType, NameEntry.Text, x, true));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.InputRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                    vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);

                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {x} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
            }

        }
        else if (RandomRadioButton.IsChecked && RangeRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked && float.TryParse(lowerrange.Text, out float lrf) && float.TryParse(upperrange.Text, out float urf) && lowerrange.Text.Contains('.') && upperrange.Text.Contains('.'))
        {
            // RANDOM FLOAT
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
                    vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
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
                    vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                    vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
            }
        }
        else if (FixedRadioButton.IsChecked && short.TryParse(FixedValueEntry.Text, out short n) && !FixedFloatConfiguration.IsChecked)
        {
            // FIXED SHORT
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n, false));
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;

                    // Write to Modbus
                    vm.SelectedServer.WriteHoldingRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
                case RegisterType.InputRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n, false));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;

                    // Write to Modbus
                    vm.SelectedServer.WriteInputRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");
                    break;
            }
        }
        else if (RangeRadioButton.IsChecked && RandomRadioButton.IsChecked && !RangeFloatConfigurationCheck.IsChecked)
        {
            // RANDOM SHORT
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
                    default:
                        Application.Current.MainPage.DisplayAlert("Error", "Invalid register type for Range value.", "OK");
                        break;
                }
            }
        }
        else if (RangeRadioButton.IsChecked && CurveRadioButton.IsChecked && !RangeFloatConfigurationCheck.IsChecked)
        {
            // CURVE SHORT
            if (short.TryParse(startval.Text, out short lowerR) && short.TryParse(endval.Text, out short upperR)
                && int.TryParse(PeriodEntry.Text, out int periodR))
            {
                short nextValue = ValueGenerators.GenerateNextSinValue(lowerR, upperR, 0, periodR);
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, false, periodR));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = nextValue;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, nextValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{nextValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, false, periodR));
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
        else if (CurveRadioButton.IsChecked && RangeRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked && 
            float.TryParse(startval.Text, out float lowerRF) && float.TryParse(endval.Text, out float upperRF) 
            && int.TryParse(PeriodEntry.Text, out int periodRF) && startval.Text.Contains('.') && endval.Text.Contains('.'))
        {
            // CURVE FLOAT
            float nextValue = ValueGenerators.GetNextSineValueFloat(lowerRF, upperRF, 0, periodRF);
            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
            short lowBits = lowHighBits[0];
            short highBits = lowHighBits[1];
            switch (register.RegisterType)
            {
                case RegisterType.HoldingRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Curve<float>(register.Address, register.RegisterType, NameEntry.Text, lowerRF, upperRF, true, periodRF));
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.HoldingRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                    vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {nextValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
                case RegisterType.InputRegister:
                    //low bits
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Curve<float>(register.Address, register.RegisterType, NameEntry.Text, lowerRF, upperRF, true, periodRF));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = lowBits;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                    //high bits
                    vm.SelectedServer.InputRegisters[register.Address].Value = highBits;
                    vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                    vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {nextValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                    break;
            }

        }
        else if (LinearRadioButton.IsChecked && !RangeFloatConfigurationCheck.IsChecked)
        {
            //LINEAR SHORT

            if (short.TryParse(LinearStartValueEntry.Text, out short lsv) && short.TryParse(LinearEndValueEntry.Text, out short lev) && short.TryParse(LinearIncrementEntry.Text, out short inc))
            {
                // Assuming the logic for determining the current value and whether it's increasing is implemented
                bool increasing = true; // Example flag, you need to manage this based on actual logic
                short nextValue = (short) ValueGenerators.GenerateLinearValue(lsv, lsv, lev, inc, ref increasing);

                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Linear<short>(register.Address, register.RegisterType, NameEntry.Text, lsv, lev, inc, false));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = nextValue;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, nextValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{nextValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Linear<short>(register.Address, register.RegisterType, NameEntry.Text, lsv, lev, inc, false));
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
        else if (LinearRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked)
        {
            //LINEAR FLOAT

            if (short.TryParse(LinearStartValueEntry.Text, out short lsv) && short.TryParse(LinearEndValueEntry.Text, out short lev) && short.TryParse(LinearIncrementEntry.Text, out short inc))
            {
                
                bool increasing = true; 
                float nextValue = ValueGenerators.GenerateLinearValue(lsv, lsv, lev, inc, ref increasing);
                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                short lowBits = lowHighBits[0];
                short highBits = lowHighBits[1];

                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        //low bits
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Linear<float>(register.Address, register.RegisterType, NameEntry.Text, lsv, lev, inc, true));
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Value = lowBits;
                        vm.SelectedServer.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, lowBits);
                        //high bits
                        vm.SelectedServer.HoldingRegisters[register.Address].Value = highBits;
                        vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                        vm.SelectedServer.WriteHoldingRegister(register.Address + 1, highBits);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {nextValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        //low bits
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Linear<float>(register.Address, register.RegisterType, NameEntry.Text, lsv, lev, inc, true));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = lowBits;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, lowBits);
                        //high bits
                        vm.SelectedServer.InputRegisters[register.Address].Value = highBits;
                        vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = true; // HELPER REGISTER
                        vm.SelectedServer.WriteInputRegister(register.Address + 1, highBits);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {nextValue} Converted to {lowHighBits[0]} and {lowHighBits[1]}", "OK");
                        break;
                }
            }
        }

        else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Incomplete settings.", "OK");
            return;
        }
        // Display to user that changes have been made and the config will need saved
        vm.SelectedServer.IsNotSaved = true;
    }
}