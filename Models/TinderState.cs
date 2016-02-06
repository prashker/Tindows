using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class TinderState
    {
        Services.SettingsServices.SettingsService _settings = Services.SettingsServices.SettingsService.Instance;

        // Singleton
        public static TinderState Instance { get; }

        private Authentication authenticationResult;
        public TinderAPI api { get; }

        // Maintain state for last time we called getUpdates()
        private string last_activity_date = "";


        Updates _updates;
        public Updates Updates { get { return _updates; } set { _updates = value; } }

        LocalUser _me;
        public LocalUser Me { get { return _me; } set { _me = value; } }

        private Boolean looping = false;

        // Info
        public Boolean IsAuthenticated
        {
            get
            {
                return Me != null;
            }
        }


        static TinderState()
        {
            // implement singleton pattern
            Instance = Instance ?? new TinderState();
        }

        private TinderState()
        {
            api = new TinderAPI();
        }

        /// <summary>
        /// Login via existing token in Settings
        /// </summary>
        /// <returns>True if login successful</returns>
        public async Task<Boolean> loginViaSavedToken()
        {
            // Prevent re-logging in
            if (IsAuthenticated)
                return false;

            // Try xAuthToken, then try FB login
            api.authenticateViaXAuthToken(_settings.XAuthToken);

            Me = await api.me();

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

            authenticationResult = await api.authenticateViaFacebook(token);
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
            // Todo!
        }

        private async Task<Updates> getLatestUpdates()
        {
            // Call getUpdates(), update latest_update_fetch

            Updates temp = await api.getUpdates(last_activity_date);
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
            Updates.absorb(await getLatestUpdates(), true);

            if (!looping)
            {
                looping = true;
                while (true)
                {
                    // Every 3 seconds
                    await Task.Delay(5000);

                    Updates newUpdate = await getLatestUpdates();

                    // Merge matches from both Updates
                    // New messages are intersperced in here

                    Updates.absorb(newUpdate, false);
                }
            }
        }
    }



}

