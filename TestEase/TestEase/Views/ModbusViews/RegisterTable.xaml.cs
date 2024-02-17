using TestEase.ViewModels;
using EasyModbus;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Platform;

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
                    viewModel?.SwitchTab(tabName);
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
            var itemToScrollTo = ((ModbusPageViewModel)this.BindingContext).CurrentItems
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

    private void onItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            ModbusPageViewModel.IRegister item = e.SelectedItem as ModbusPageViewModel.IRegister;
        }
        
    }
}