using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Tindows.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using System.Diagnostics;
using Tindows.Models;
using Tindows.Externals.Tinder_Objects;
using Newtonsoft.Json;
using Tindows.Toasts;
using Newtonsoft.Json.Linq;

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
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs e)
        {
            // perform long-running load
            await Task.Delay(0);

            // Determine if the app was already open or not (by checking the state)
            // Already logged in: Check for toasts
            // Not logged in: Login, check for toasts, then do default action (navigation)

            // Authenticate in all cases, then perform navigation options
            // authenticated = true (either already logged in, or true login)
            Boolean authenticated = await state.loginViaSavedToken();

            if (authenticated)
            {
                // Got a toast, determine type and navigate accordingly
                if (e is ToastNotificationActivatedEventArgs)
                {
                    var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                    dynamic args = JsonConvert.DeserializeObject<JObject>(toastActivationArgs.Argument);

                    // New message
                    // Navigate to that toast after login
                    if (args.source == typeof(NewMessageToast).ToString())
                    {
                        // args.args = conversation_id
                        NavigationService.Navigate(typeof(Views.ConversationsPage), args.args);
                    }
                    else if (args.source == typeof(NewMatchToast).ToString())
                    {
                        NavigationService.Navigate(typeof(Views.ConversationsPage), args.args);
                    }
                }
                else
                {
                    // Standard login, go to superficial page
                    NavigationService.Navigate(typeof(Views.SuperficialPage));
                }
            }
            else
            {
                // Todo: Invalid XAuth TOAST 
                NavigationService.Navigate(typeof(Views.LoginPage));
            }
        }

    }

}

