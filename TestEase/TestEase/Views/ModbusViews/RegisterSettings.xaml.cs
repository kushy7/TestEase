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
                    RangeFloatConfiguration.IsVisible = false;
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
                    RangeFloatConfiguration.IsVisible = false;
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
                    vm.SelectedServer.WriteHoldingRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{n}", "OK");
                    break;
                case RegisterType.InputRegister:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                        .Add(new Fixed<short>(register.Address, register.RegisterType, NameEntry.Text, n));
                    vm.InputRegisters[register.Address - 1].Value = n;
                    vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                    vm.SelectedServer.WriteInputRegister(register.Address, n);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{n}", "OK");
                    break;
                case RegisterType.DiscreteInput:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedBooleanValue));
                    vm.DiscreteInputs[register.Address - 1].Value = vm.SelectedBooleanValue;
                    vm.DiscreteInputs[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteDiscreteInput(register.Address, vm.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{BooleanNameEntry.Text}\nValue:{vm.SelectedBooleanValue}", "OK");
                    break;
                case RegisterType.Coil:
                    vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new CoilOrDiscrete(register.Address, register.RegisterType, NameEntry.Text, vm.SelectedBooleanValue));
                    vm.Coils[register.Address - 1].Value = vm.SelectedBooleanValue;
                    vm.Coils[register.Address - 1].Name = BooleanNameEntry.Text;
                    vm.SelectedServer.WriteCoil(register.Address, vm.SelectedBooleanValue);
                    Application.Current.MainPage.DisplayAlert("Saved", $"Name:{BooleanNameEntry.Text}\nValue:{vm.SelectedBooleanValue}", "OK");
                    break;
            }
        }
        else if (RangeRadioButton.IsChecked && RandomRadioButton.IsChecked)
        {
            if (short.TryParse(lowerrange.Text, out short lr) && short.TryParse(upperrange.Text, out short ur))
            {
                short randomValue = ValueGenerators.GenerateRandomValueShort(lr, ur);
                switch (register.RegisterType)
                {
                    case RegisterType.HoldingRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur));
                        vm.HoldingRegisters[register.Address - 1].Value = randomValue;
                        vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, randomValue);
                        Application.Current.MainPage.DisplayAlert("Saved", $"Name:{NameEntry.Text}\nValue:{randomValue}", "OK");
                        break;
                    case RegisterType.InputRegister:
                        vm.SelectedServer.WorkingConfiguration.RegisterModels
                            .Add(new Random<short>(register.Address, register.RegisterType, NameEntry.Text, lr, ur));
                        vm.InputRegisters[register.Address - 1].Value = randomValue;
                        vm.InputRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteInputRegister(register.Address, randomValue);
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
                            .Add(new Curve<short>(register.Address, register.RegisterType, NameEntry.Text, lowerR, upperR, periodR));
                        vm.HoldingRegisters[register.Address - 1].Value = nextValue;
                        vm.HoldingRegisters[register.Address - 1].Name = NameEntry.Text;
                        vm.SelectedServer.WriteHoldingRegister(register.Address, nextValue);
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


    

