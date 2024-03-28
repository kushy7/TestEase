using TestEase.ViewModels;
using TestEase.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace TestEase.Views
{
    public partial class MQTTBrokerPage : ContentPage
    {
        private MQTTBrokerPageViewModel _viewModel;

        public MQTTBrokerPage(MQTTBrokerPageViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            this.BindingContext = _viewModel;

            // Subscribe to StatusChanged event
            _viewModel.StatusChanged += ViewModel_StatusChanged;
        }

        [Obsolete]
        private void ViewModel_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                statusIndicator.Content = e.IsRunning ? CreateStatusIndicator(e.GreenColor, "Online") : CreateStatusIndicator(e.RedColor, "Offline");
            });
        }

        private Grid CreateStatusIndicator(CustomColor color, string text)
        {
            return new Grid
            {
                Children =
                {
                    new Ellipse { WidthRequest = 50, HeightRequest = 50, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromRgb(color.R, color.G, color.B) },
                    new Label { Text = text, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                }
            };
        }

        public void ToggleCommand(object sender, EventArgs e)
        {
            var greenColor = new CustomColor(0, 255, 0);
            var redColor = new CustomColor(255, 0, 0);
            _viewModel.ToggleCommand(greenColor, redColor);
        }
    }
}