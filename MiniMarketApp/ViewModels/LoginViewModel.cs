using System.Windows.Input;
using MiniMarketApp.Helpers;
using MiniMarketApp.Services;

namespace MiniMarketApp.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        LoginCommand = new RelayCommand(_ => Login());
    }

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoginCommand { get; }

    private void Login()
    {
        var user = _authService.Login(UserName, Password);
        ErrorMessage = user is null ? LocalizedStrings.Get("InvalidCredentialsMessage") : string.Empty;
    }
}
