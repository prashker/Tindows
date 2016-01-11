using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tindows.Externals;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Navigation;

namespace Tindows.ViewModels
{
    public class LoginPageViewModel : Mvvm.ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings = Services.SettingsServices.SettingsService.Instance;


        public LoginPageViewModel()
        {
        }


        // Per http://stackoverflow.com/questions/12144077/async-await-when-to-return-a-task-vs-void
        // The main exception should be when you need to have a void return type (for events). 
        
        public async void facebookLogin()
        {

            FBAuthTinder auth = new FBAuthTinder();
            TinderOAuthToken token = await auth.authenticateForTinder();
            TinderAPI t = new TinderAPI();

            AuthRootObject o = await t.authenticateViaFacebook(token);

            // Persist to settings
            _settings.XAuthToken = o.token;
        }
    }
}
