using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Tindows.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using System.Diagnostics;
using Tindows.Models;
using Tindows.Externals.Tinder_Objects;

namespace Tindows
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        ISettingsService _settings;
        TinderState state;

        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion

            // State Maintenance Logic
            state = TinderState.Instance;
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // setup hamburger shell
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            await Task.Yield();
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            state.startUpdatesLoop();
            state.startUpdatesLoop();
            state.startUpdatesLoop();
            state.startUpdatesLoop();
            state.startUpdatesLoop();
            state.startUpdatesLoop();


            // perform long-running load
            await Task.Delay(0);

            // Try logging in via XAuthToken
            state.api.authenticateViaXAuthToken(_settings.XAuthToken);

            // Update location (and verify the login worked!)
            // TODO: Actually acquire GPS coordinates
            Ping authenticated = await state.api.setLocation(45.3530996, -75.665127);

            state.getProfileInfo();


            // If authentication failed, go to Facebook Login Page
            if (authenticated == null)
            {
                // Todo: Invalid XAuth TOAST 
                NavigationService.Navigate(typeof(Views.LoginPage));
            }
            else
            {
                // Get updates of past info :)
                // Async
                state.prepareInitialState();
                NavigationService.Navigate(typeof(Views.SuperficialPage));
            }

        }
    }

}

