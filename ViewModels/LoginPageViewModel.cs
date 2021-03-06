﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;
using Tindows.Models;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Navigation;

namespace Tindows.ViewModels
{
    public class LoginPageViewModel : Mvvm.ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings = Services.SettingsServices.SettingsService.Instance;
        TinderState state = TinderState.Instance;



        public LoginPageViewModel()
        {
        }


        // Per http://stackoverflow.com/questions/12144077/async-await-when-to-return-a-task-vs-void
        // The main exception should be when you need to have a void return type (for events). 
        
        public async void facebookLogin()
        {

            Boolean authenticated = await state.loginViaFacebook();

            //Ping authenticated = await state.api.setLocation(45.3530996, -75.665127);

            // If authentication failed, go to Facebook Login Page
            if (!authenticated)
            {
                // Todo: Invalid XAuth TOAST 
                // NavigationService.Navigate(typeof(Views.LoginPage));
            }
            else
            {
                NavigationService.Navigate(typeof(Views.SuperficialPage));
            }
        }

        public void facebookLogout()
        {
            state.logout();
        }
    }
}
