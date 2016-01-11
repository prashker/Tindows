using System;
using Windows.UI.Xaml;

namespace Tindows.Services.SettingsServices
{
    // Implement the settings here (options)

    public interface ISettingsService
    {
        // Defaults
        bool UseShellBackButton { get; set; }
        ApplicationTheme AppTheme { get; set; }
        TimeSpan CacheMaxDuration { get; set; }
        
        // App Specifics
        string XAuthToken { get; set; }
    }
}
