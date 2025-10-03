using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App;    
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        private string email = "user3@mail.com"; 

        [ObservableProperty]
        private string password = "user3"; 
        [ObservableProperty]
        private string loginMessage;

        public LoginViewModel(IAuthService authService, GlobalViewModel global)
        {
            _authService = authService;
            _global = global;
        }

        [RelayCommand]
        private void Login()
        {
            Client? authenticatedClient = _authService.Login(Email, Password);

            if (authenticatedClient is not null)
            {
                // bewaar ingelogde gebruiker en dus role
                _global.Client = authenticatedClient;

                // vriendelijke melding
                LoginMessage = $"Welkom {authenticatedClient.Name}!";

                // schakel naar Shell uit DI (App.xaml.cs heeft ShowShell())
                ((App)Application.Current).ShowShell();
            }
            else
            {
                LoginMessage = "Ongeldige inloggegevens.";
            }
        }
    }
}
