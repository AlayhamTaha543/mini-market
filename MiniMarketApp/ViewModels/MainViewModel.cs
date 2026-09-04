using System.Windows.Input;
using MiniMarketApp.Helpers;
using MiniMarketApp.Models;

namespace MiniMarketApp.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private UserRole _currentRole = UserRole.Employee;
    private string _statusMessage = string.Empty;

    public MainViewModel()
    {
        NavigateCommand = new RelayCommand(Navigate);
    }

    public ICommand NavigateCommand { get; }

    public UserRole CurrentRole
    {
        get => _currentRole;
        set => SetProperty(ref _currentRole, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool CanAccess(AppScreen screen)
    {
        return CurrentRole switch
        {
            UserRole.Owner => true,
            UserRole.Employee => screen is AppScreen.PointOfSale or AppScreen.Inventory,
            _ => false,
        };
    }

    private void Navigate(object? parameter)
    {
        if (parameter is not AppScreen screen)
        {
            return;
        }

        if (!CanAccess(screen))
        {
            StatusMessage = LocalizedStrings.Get("AccessDeniedMessage");
            return;
        }

        StatusMessage = string.Empty;
    }
}
