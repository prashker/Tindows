﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;
using Tindows.Services.SettingsServices;
using Tindows.Toasts;

namespace Tindows.Models
{
    /// <summary>
    /// Singleton class for maintaining state for the entire application :)
    /// Copies singleton logic from Template10 convention
    /// </summary>
    public class TinderState : BindableBase
    {
        Services.SettingsServices.SettingsService _settings = Services.SettingsServices.SettingsService.Instance;

        // Singleton
        public static TinderState Instance { get; }

        private Authentication authenticationResult;

        public TinderAPI Api { get; set; }

        // Maintain state for last time we called getUpdates()
        private string last_activity_date = "";


        Updates _updates;
        public Updates Updates { get { return _updates; } set { _updates = value; } }

        LocalUser _me;
        public LocalUser Me
        {
            get
            {
                return _me;
            }
            set
            {
                Set(ref _me, value);

                // Set authenticated based off this :)
                IsAuthenticated = value != null;
            }
        }

        private Boolean looping = false;

        Boolean _isAuthenticated;
        public Boolean IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
            set
            {
                Set(ref _isAuthenticated, value);
            }
        }


        static TinderState()
        {
            // implement singleton pattern
            Instance = Instance ?? new TinderState();
        }

        private TinderState()
        {
            Api = new TinderAPI();
        }

        /// <summary>
        /// Login via existing token in Settings
        /// </summary>
        /// <returns>True if login successful</returns>
        public async Task<Boolean> loginViaSavedToken()
        {
            // Prevent re-logging in
            if (_settings.XAuthToken == null)
                return false;
            if (IsAuthenticated)
                return true;

            // Try xAuthToken, then try FB login
            Api.authenticateViaXAuthToken(_settings.XAuthToken);

            Me = await Api.me();

            if (IsAuthenticated)
            {
                startUpdatesLoop();
            }

            // True if authenticated, false if failed
            return IsAuthenticated;
        }

        /// <summary>
        /// Login via facebook (fresh), and persist token to settings
        /// </summary>
        /// <returns>True if successfully logged in</returns>
        public async Task<Boolean> loginViaFacebook()
        {
            // Prevent re-logging in
            if (IsAuthenticated)
                return false;

            FBAuthTinder auth = new FBAuthTinder();
            TinderOAuthToken token = null;
            try
            {
                token = await auth.authenticateForTinder();
            }
            catch (FBAuthenticationError)
            {
                return false;
            }

            if (token == null)
                return false;

            authenticationResult = await Api.authenticateViaFacebook(token);
            Me = authenticationResult.user;

            // Persist to settings
            _settings.XAuthToken = authenticationResult.token;

            if (IsAuthenticated)
            {
                startUpdatesLoop();
            }

            return IsAuthenticated;
        }

        public void logout()
        {
            // Unauthenticate
            // New API
            // Reset XAuthToken
            IsAuthenticated = false;
            Api = new TinderAPI();
            _settings.XAuthToken = null;
            looping = false;
            last_activity_date = "";
            Updates = null;
        }

        private async Task<Updates> getLatestUpdates()
        {
            // Call getUpdates(), update latest_update_fetch

            Updates temp = await Api.getUpdates(last_activity_date);
            last_activity_date = temp.last_activity_date;

            return temp;
        }

        // Is this the right way to do this?
        public async void startUpdatesLoop()
        {
            if (Updates != null)
                return;

            // Set the initial state, without propagating the updates
            Updates = new Updates();
            Updates.absorb(await getLatestUpdates(), true, Me._id);

            if (!looping)
            {
                looping = true;
                while (true)
                {
                    // Every 3 seconds
                    await Task.Delay(3000);

                    if (looping)
                    {
                        Updates newUpdate = await getLatestUpdates();

                        // Merge matches from both Updates
                        // New messages are intersperced in here

                        Updates.absorb(newUpdate, false, Me._id);
                    }
                }
            }
        }
    }



}

