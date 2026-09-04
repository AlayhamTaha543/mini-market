using System.Windows;

namespace MiniMarketApp.Helpers;

public static class LocalizedStrings
{
    public static string Get(string key)
    {
        return Application.Current.TryFindResource(key) as string ?? key;
    }
}
