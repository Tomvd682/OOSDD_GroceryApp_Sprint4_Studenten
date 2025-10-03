using Grocery.App.Views;
using Grocery.App.ViewModels;


namespace Grocery.App
{
    public partial class App : Application
    {
        private readonly AppShell _shell;

        // Haal AppShell en LoginViewModel binnen
        public App(AppShell shell, LoginViewModel viewModel)
        {
            InitializeComponent();
            _shell = shell;

            // Start op het login-scherm
            MainPage = new LoginView(viewModel);
        }

        // Helper om na login naar de shell te gaan
        public void ShowShell()
        {
            MainPage = _shell;
        }
    }
}
