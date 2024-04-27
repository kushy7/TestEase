using TestEase.Models;
using TestEase.ViewModels;
using TestEase.Helpers;
using Microsoft.Maui.Platform;
using System.Diagnostics;

namespace TestEase.Views.ModbusViews;

public partial class RegisterSettings : ContentView
{

    public RegisterSettings()
    {
        InitializeComponent();
    }


    //manages the register settings that show up depending on the radiobutton selected
    private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton rb && rb.IsChecked)
        {
            // Reset visibility for all specific settings
            FloatConfiguration.IsVisible = ValueInput.IsVisible = RangeFloatConfiguration.IsVisible = false;
            LowerValueInput.IsVisible = UpperValueInput.IsVisible = false;
            StartingValueInput.IsVisible = EndingValueInput.IsVisible = PeriodEntry.IsVisible = false;
            LinearConfiguration.IsVisible = false;
            CurveConfiguration.IsVisible = false; 

            // Hide or show range-specific radio buttons based on the selection
            var isRangeSelected = RangeRadioButton.IsChecked;
            var isLineaerSelected = LinearRadioButton.IsChecked;
            var isCurveSelected = CurveRadioButton.IsChecked;
            var isRandomSelected = RandomRadioButton.IsChecked;
            LinearRadioButton.IsVisible = RandomRadioButton.IsVisible = CurveRadioButton.IsVisible = isRangeSelected;

            // Handle visibility based on selected mode
            switch (rb.Content.ToString())
            {
                case "Fixed":
                    FloatConfiguration.IsVisible = ValueInput.IsVisible = true;
                    break;
                case "Range":
                    RangeFloatConfiguration.IsVisible = isRangeSelected;
                    LowerValueInput.IsVisible = UpperValueInput.IsVisible = isRandomSelected;
                    StartingValueInput.IsVisible = EndingValueInput.IsVisible = PeriodEntry.IsVisible = CurveConfiguration.IsVisible = isCurveSelected;
                    LinearConfiguration.IsVisible = isLineaerSelected;
                    break;
                case "Random":
                    LowerValueInput.IsVisible = UpperValueInput.IsVisible = isRandomSelected;
                    RangeFloatConfiguration.IsVisible = isRangeSelected;
                    break;
                case "Curve":
                    StartingValueInput.IsVisible = EndingValueInput.IsVisible = PeriodEntry.IsVisible = CurveConfiguration.IsVisible = isCurveSelected;
                    RangeFloatConfiguration.IsVisible = isRangeSelected;
                    break;
                case "Linear":
                    LinearConfiguration.IsVisible = isLineaerSelected;
                    RangeFloatConfiguration.IsVisible = isRangeSelected;
                    break;
            }
        }
    }


    //save the registers settings after configuring the options
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

        // BOOL REGISTERS
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
            return;
        }
        else if (FixedRadioButton.IsChecked && FixedFloatConfiguration.IsChecked && float.TryParse(FixedValueEntry.Text, out float x))
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
        else if (RandomRadioButton.IsChecked && RangeRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked && float.TryParse(lowerrange.Text, out float lrf) && float.TryParse(upperrange.Text, out float urf))
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

                    // Check if address+1 was a float helper, if it was, reset it
                    if (vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper)
                    {
                        vm.SelectedServer.HoldingRegisters[register.Address].Value = (short) 0;
                        vm.SelectedServer.HoldingRegisters[register.Address].Name = "";
                        vm.SelectedServer.HoldingRegisters[register.Address].IsPlaying = false;
                        vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = false;
                        vm.SelectedServer.HoldingRegisters[register.Address].IsModified = false;
                    }
                    break;
                case RegisterType.InputRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n, false));
                    vm.SelectedServer.InputRegisters[register.Address - 1].Value = n;
                    vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;

                    // Write to Modbus
                    vm.SelectedServer.WriteInputRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {n}", "OK");

                    // Check if address+1 was a float helper, if it was, reset it
                    if (vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper)
                    {
                        vm.SelectedServer.InputRegisters[register.Address].Value = (short)0;
                        vm.SelectedServer.InputRegisters[register.Address].Name = "";
                        vm.SelectedServer.InputRegisters[register.Address].IsPlaying = false;
                        vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = false;
                        vm.SelectedServer.InputRegisters[register.Address].IsModified = false;
                    }
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

                        // Check if address+1 was a float helper, if it was, reset it
                        if (vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper)
                        {
                            vm.SelectedServer.HoldingRegisters[register.Address].Value = (short)0;
                            vm.SelectedServer.HoldingRegisters[register.Address].Name = "";
                            vm.SelectedServer.HoldingRegisters[register.Address].IsPlaying = false;
                            vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = false;
                            vm.SelectedServer.HoldingRegisters[register.Address].IsModified = false;
                        }
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur, false));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = randomValue;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name: {NameEntry.Text}\nValue: {randomValue}", "OK");

                        // Check if address+1 was a float helper, if it was, reset it
                        if (vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper)
                        {
                            vm.SelectedServer.InputRegisters[register.Address].Value = (short)0;
                            vm.SelectedServer.InputRegisters[register.Address].Name = "";
                            vm.SelectedServer.InputRegisters[register.Address].IsPlaying = false;
                            vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = false;
                            vm.SelectedServer.InputRegisters[register.Address].IsModified = false;
                        }
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

                        // Check if address+1 was a float helper, if it was, reset it
                        if (vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper)
                        {
                            vm.SelectedServer.HoldingRegisters[register.Address].Value = (short)0;
                            vm.SelectedServer.HoldingRegisters[register.Address].Name = "";
                            vm.SelectedServer.HoldingRegisters[register.Address].IsPlaying = false;
                            vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = false;
                            vm.SelectedServer.HoldingRegisters[register.Address].IsModified = false;
                        }
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, false, periodR));
                        vm.SelectedServer.InputRegisters[register.Address - 1].Value = nextValue;
                        vm.SelectedServer.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, nextValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{nextValue}", "OK");

                        // Check if address+1 was a float helper, if it was, reset it
                        if (vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper)
                        {
                            vm.SelectedServer.InputRegisters[register.Address].Value = (short)0;
                            vm.SelectedServer.InputRegisters[register.Address].Name = "";
                            vm.SelectedServer.InputRegisters[register.Address].IsPlaying = false;
                            vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = false;
                            vm.SelectedServer.InputRegisters[register.Address].IsModified = false;
                        }
                        break;

                    default:
                        Application.Current.MainPage.DisplayAlert("Error", "Invalid register type for Curve value.", "OK");
                        break;
                }
            }

        }
        else if (CurveRadioButton.IsChecked && RangeRadioButton.IsChecked && RangeFloatConfigurationCheck.IsChecked && 
            float.TryParse(startval.Text, out float lowerRF) && float.TryParse(endval.Text, out float upperRF) 
            && int.TryParse(PeriodEntry.Text, out int periodRF))
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
                //initially increasing
                bool increasing = true;
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

            if (float.TryParse(LinearStartValueEntry.Text, out float lsv) && float.TryParse(LinearEndValueEntry.Text, out float lev) && float.TryParse(LinearIncrementEntry.Text, out float inc))
            {
                
                bool increasing = true; 
                float nextValue = ValueGenerators.GenerateLinearValueFloat(lsv, lsv, lev, inc, ref increasing);
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

    private async void OnClearButtonClick(object sender, EventArgs args)
    {
        bool isUserSure = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to clear this register?", "Yes", "No");
        if (!isUserSure)
            return;

        var vm = this.BindingContext as ModbusPageViewModel;
        var register = vm.SelectedServer.SelectedRegister;

        // Find the index of the existing register, if it exists, remove it.
        int index = vm.SelectedServer.WorkingConfiguration.RegisterModels
            .FindIndex(r => r.Address == register.Address && r.Type == register.RegisterType);
        if (index != -1)
        {
            vm.SelectedServer.WorkingConfiguration.RegisterModels.RemoveAt(index);
        }

        if (register.RegisterType == RegisterType.InputRegister)
        {
            if (vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper)
            {
                // Clear the helper register from table
                vm.SelectedServer.InputRegisters[register.Address].Name = "";
                vm.SelectedServer.InputRegisters[register.Address].Value = (short) 0;
                vm.SelectedServer.InputRegisters[register.Address].IsFloatHelper = false;
                vm.SelectedServer.InputRegisters[register.Address].IsModified = false;
                vm.SelectedServer.InputRegisters[register.Address].IsPlaying = false;

                // Reset the helper to 0 in modbus
                vm.SelectedServer.WriteInputRegister(register.Address + 1, (short) 0);
            }
            // Clear the selected register from table
            register.Name = "";
            register.Value = (short) 0;
            register.IsFloatHelper = false;
            register.IsModified = false;
            register.IsPlaying = false;

            // Reset the selected register to 0 in modbus
            vm.SelectedServer.WriteInputRegister(register.Address, 0);
        } else if (register.RegisterType == RegisterType.HoldingRegister)
        {
            if (vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper)
            {
                // Clear the helper register from table
                vm.SelectedServer.HoldingRegisters[register.Address].Name = "";
                vm.SelectedServer.HoldingRegisters[register.Address].Value = (short) 0;
                vm.SelectedServer.HoldingRegisters[register.Address].IsFloatHelper = false;
                vm.SelectedServer.HoldingRegisters[register.Address].IsModified = false;
                vm.SelectedServer.HoldingRegisters[register.Address].IsPlaying = false;

                // Reset the helper to 0 in modbus
                vm.SelectedServer.WriteHoldingRegister(register.Address + 1, (short) 0);
            }
            // Clear the selected register from table
            register.Name = "";
            register.Value = (short) 0;
            register.IsFloatHelper = false;
            register.IsModified = false;
            register.IsPlaying = false;

            // Reset the selected register to 0 in modbus
            vm.SelectedServer.WriteHoldingRegister(register.Address, (short) 0);
        }

        // Display to user that changes have been made and the config will need saved
        vm.SelectedServer.IsNotSaved = true;

        // Selected register set to nothing
        vm.SelectedServer.SelectedRegister = null;
    }

}