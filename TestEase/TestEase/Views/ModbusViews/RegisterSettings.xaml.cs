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
                    RangeFloatConfiguration.IsVisible = false;
                    LowerValueInput.IsVisible = false;
                    UpperValueInput.IsVisible = false;
                    break;
                case "Range":
                    //show the register setting for range
                    RangeFloatConfiguration.IsVisible = true;
                    LowerValueInput.IsVisible = true;
                    UpperValueInput.IsVisible = true;
                    // Hide the Float and Value components
                    FloatConfiguration.IsVisible = false;
                    ValueInput.IsVisible = false;
                    break;
            }
        }
    }
}
