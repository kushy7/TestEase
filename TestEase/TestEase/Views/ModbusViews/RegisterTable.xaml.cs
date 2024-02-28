using TestEase.ViewModels;
using EasyModbus;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using TestEase.Models;

namespace TestEase.Views.ModbusViews;

public partial class RegisterTable : ContentView
{
	public RegisterTable()
	{
		InitializeComponent();
    }
    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var rButton = sender as RadioButton;        

        if (rButton == null)
            Application.Current.MainPage.DisplayAlert("Error", "Button not found.", "OK");
        else
        {
            if (!rButton.IsChecked)
            {
                return;
            } else
            {
                string tabName = "";

                if (rButton == DiscreteInputsRadioButton)
                {
                    tabName = "DiscreteInputs";
                }
                else if (rButton == CoilsRadioButton)
                {
                    tabName = "Coils";
                }
                else if (rButton == InputRegistersRadioButton)
                {
                    tabName = "InputRegisters";
                }
                else if (rButton == HoldingRegistersRadioButton)
                {
                    tabName = "HoldingRegisters";
                }

                // If there is a matching tab name, call SwitchTab
                if (!string.IsNullOrEmpty(tabName))
                {
                    var viewModel = this.BindingContext as ModbusPageViewModel;
                    viewModel?.SelectedServer.SwitchTab(tabName);
                    viewModel.SelectedServer.SelectedRegister = null;
                }
            }
        }
    }

    private async void OnJumpButtonClicked(object sender, EventArgs e)
    {
        if (int.TryParse(AddressEntry.Text, out int addressNumber))
        {
            // Assuming the AddressViewModel.Addresses is a list of AddressItem
            // and AddressItem has an 'Address' property.
            var itemToScrollTo = ((ModbusPageViewModel)this.BindingContext).SelectedServer.CurrentItems
                                 .FirstOrDefault(item => item.Address == addressNumber);
            if (itemToScrollTo != null)
            {
                AddressesListView.ScrollTo(itemToScrollTo, ScrollToPosition.Start, true);
            }
            else
            {
                // Handle the case where the address is not found. For example:
                await Application.Current.MainPage.DisplayAlert("Not Found", $"Address {addressNumber} not found.", "OK");
            }
        }
        else
        {
            // Handle invalid input
            await Application.Current.MainPage.DisplayAlert("Invalid Input", "Please enter a valid address number.", "OK");
            
            
        }
    }

    private object? lastSelectedItem = null;

    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (e.Item == null) return;

        var listView = (ListView)sender;

        var item = e.Item as ModbusServerModel.IRegister;
        if (item == null || item.IsFloatHelper)
        {
            return;
        }

        // Check if the tapped item is the same as the currently selected item
        if (e.Item == lastSelectedItem)
        {
            // Item is already selected, so unselect it
            listView.SelectedItem = null;
            lastSelectedItem = null;
            viewModel.SelectedServer.SelectedRegister = null;
        } else
        {
            // listView.SelectedItem = e.Item;
            lastSelectedItem = e.Item;
            OnItemSelected(sender, new SelectedItemChangedEventArgs(e.Item, e.ItemIndex));
        }
    }

    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (e.SelectedItem is ModbusServerModel.IRegister selectedRegister) 
        {
            viewModel.SelectedServer.SelectedRegister = selectedRegister;
            if (selectedRegister.RegisterType == Models.RegisterType.HoldingRegister || selectedRegister.RegisterType == Models.RegisterType.InputRegister)
            {
                // Holding Register or Input Register actions actions
                // FOR FILLING IN INPUT ENTRIES FOR CONFIGURED REGISTERS
                if (selectedRegister.IsModified)
                {
                    var modifiedRegister = viewModel.SelectedServer.WorkingConfiguration.RegisterModels.Find(x => x.Address == selectedRegister.Address);
                    if (modifiedRegister is Fixed<short> fixedShortRegister)
                    {
                        viewModel.FixedEntryText = fixedShortRegister.value.ToString();
                        viewModel.LowerRangeText = "";
                        viewModel.UpperRangeText = "";
                        viewModel.StartValText = "";
                        viewModel.EndValText = "";
                        viewModel.PeriodText = "";
                    } else if (modifiedRegister is Fixed<float> fixedFloatRegister)
                    {
                        viewModel.FixedEntryText = fixedFloatRegister.value.ToString();
                        viewModel.LowerRangeText = "";
                        viewModel.UpperRangeText = "";
                        viewModel.StartValText = "";
                        viewModel.EndValText = "";
                        viewModel.PeriodText = "";
                    } else if (modifiedRegister is Random<short> randomShortRegister)
                    {
                        viewModel.FixedEntryText = "";
                        viewModel.LowerRangeText = randomShortRegister.startValue.ToString();
                        viewModel.UpperRangeText = randomShortRegister.endValue.ToString();
                        viewModel.StartValText = "";
                        viewModel.EndValText = "";
                        viewModel.PeriodText = "";
                    } else if (modifiedRegister is Random<float> randomFloatRegister)
                    {
                        viewModel.FixedEntryText = "";
                        viewModel.LowerRangeText = randomFloatRegister.startValue.ToString();
                        viewModel.UpperRangeText = randomFloatRegister.endValue.ToString();
                        viewModel.StartValText = "";
                        viewModel.EndValText = "";
                        viewModel.PeriodText = "";
                    } else if (modifiedRegister is Curve<short> curveShortRegister)
                    {
                        viewModel.FixedEntryText = "";
                        viewModel.LowerRangeText = "";
                        viewModel.UpperRangeText = "";
                        viewModel.StartValText = curveShortRegister.startValue.ToString();
                        viewModel.EndValText = curveShortRegister.endValue.ToString();
                        viewModel.PeriodText = curveShortRegister.Period.ToString();
                    } else if (modifiedRegister is Curve<float> curveFloatRegister)
                    {
                        viewModel.FixedEntryText = "";
                        viewModel.LowerRangeText = "";
                        viewModel.UpperRangeText = "";
                        viewModel.StartValText = curveFloatRegister.startValue.ToString();
                        viewModel.EndValText = curveFloatRegister.endValue.ToString();
                        viewModel.PeriodText = curveFloatRegister.Period.ToString();
                    }
                } else
                {
                    viewModel.FixedEntryText = "";
                    viewModel.LowerRangeText = "";
                    viewModel.UpperRangeText = "";
                    viewModel.StartValText = "";
                    viewModel.EndValText = "";
                    viewModel.PeriodText = "";
                }

            } else if (selectedRegister.RegisterType == Models.RegisterType.DiscreteInput || selectedRegister.RegisterType == Models.RegisterType.Coil)
            {
                // Discrete Input or Coil actions
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Register Type not found. v2.", "OK");
            }
            
        } else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Register Type not found. v3.", "OK");
        }
        
    }

    private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        viewModel?.SelectedServer.FilterModifiedRegisters(e.Value);
    }
}