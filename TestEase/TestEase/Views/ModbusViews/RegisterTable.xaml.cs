using TestEase.ViewModels;
using EasyModbus;
using CommunityToolkit.Maui.Core;
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

    private object? lastSelectedItem = null;

    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (e.Item == null) return;

        var listView = (ListView)sender;

        // Check if the tapped item is the same as the currently selected item
        if (e.Item == lastSelectedItem)
        {
            // Item is already selected, so unselect it
            listView.SelectedItem = null;
            lastSelectedItem = null;
            viewModel.SelectedRegister = null;
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
        if (e.SelectedItem is ModbusPageViewModel.IRegister selectedRegister) 
        {
            viewModel.SelectedRegister = selectedRegister;
            selectedRegister.Name = "changed";
            if (selectedRegister.RegisterType == Models.RegisterType.HoldingRegister)
            {
                selectedRegister.Value = (short) ((short) selectedRegister.Value + 1);
            } else if (selectedRegister.RegisterType == Models.RegisterType.InputRegister)
            {
                selectedRegister.Value = (short)((short)selectedRegister.Value + 1);
            } else if (selectedRegister.RegisterType == Models.RegisterType.DiscreteInput)
            {
                selectedRegister.Value = !( (bool)selectedRegister.Value);
            }
            else if (selectedRegister.RegisterType == Models.RegisterType.Coil)
            {
                selectedRegister.Value = !( (bool) selectedRegister.Value);
            } else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Register Type not found. v2.", "OK");
            }
            
        } else
        {
            Application.Current.MainPage.DisplayAlert("Error", "Register Type not found. v3.", "OK");
        }
        
    }
}