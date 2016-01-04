using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Navigation;

namespace Tindows.ViewModels
{
    public class LoginPageViewModel : Mvvm.ViewModelBase
    {
        public LoginPageViewModel()
        {
        }


        // Per http://stackoverflow.com/questions/12144077/async-await-when-to-return-a-task-vs-void
        // The main exception should be when you need to have a void return type (for events). 
        
        public async void facebookLogin()
        {
            var app_id = "464891386855067";
            var scope = "public_profile, email";

            // Generate URL
            var callbackUrl = "https://www.facebook.com/connect/login_success.html";
            var url = "https://www.facebook.com/dialog/oauth?client_id=" + app_id +
                        "&redirect_uri=https://www.facebook.com/connect/login_success.html" +
                        "&scope=" + scope + "&display=popup&response_type=token";

            Uri startUri = new Uri(url);
            Uri endUri = new Uri(callbackUrl);

            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
            // Example response:
            // result.ReponseData = "https://www.facebook.com/connect/login_success.html#access_token=[PARSE-FOR-THIS]&expires_in=5754"
        }
    }
}
