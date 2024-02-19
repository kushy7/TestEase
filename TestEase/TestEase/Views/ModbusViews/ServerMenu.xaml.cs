using EasyModbus;
using TestEase.Models;
using TestEase.ViewModels;

namespace TestEase.Views.ModbusViews;

public partial class ServerMenu : ContentView
{
	public ServerMenu()
	{
		InitializeComponent();
    }

	private void AddServer(object sender, EventArgs args)
	{
        var vm = this.BindingContext as ModbusPageViewModel;
        if (vm.AppViewModel.ModbusServers.Count == 0)
		{
			vm.AppViewModel.ModbusServers.Add(new ModbusServerModel(502));
		} else
		{
			var port = vm.AppViewModel.ModbusServers[vm.AppViewModel.ModbusServers.Count - 1].Port + 1;
            vm.AppViewModel.ModbusServers.Add(new ModbusServerModel(port));
		}
	}

    private void OnTurnOnOffClicked(object sender, EventArgs e)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var menuItem = (MenuItem)sender;
        var item = (ModbusServerModel)menuItem.BindingContext;

        if (item != null)
        {
            // Reverses the boolean
            var server = vm.AppViewModel.ModbusServers.FirstOrDefault(s => s.Port == item.Port);
            if (server.IsRunning)
            {
                server.IsRunning = false;
                server.StopServer();
            } else if (!server.IsRunning)
            {
                server.IsRunning = true;
                server.StartServer();
            }
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var menuItem = (MenuItem)sender;
        var item = (ModbusServerModel)menuItem.BindingContext;

        if (item != null)
        {
            vm.AppViewModel.ModbusServers.Remove(vm.AppViewModel.ModbusServers.FirstOrDefault(s => s.Port == item.Port));
        }
    }
}