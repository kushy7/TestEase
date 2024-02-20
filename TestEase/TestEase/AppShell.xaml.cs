namespace TestEase
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void OnTogged(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            } else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
    }
}
