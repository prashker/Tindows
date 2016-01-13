using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Tindows.Externals.Tinder_Objects;

namespace Tindows.Externals
{
    class TinderOAuthToken
    {
        public string facebook_token { get; }
        public string facebook_uid { get; }

        public TinderOAuthToken(string token, string uid)
        {
            facebook_token = token;
            facebook_uid = uid;
        }
    }

    // TODO: Implement HTTP Client and calls!
    class TinderAPI
    {
        private const string UserAgent = "Tinder Android Version 4.4.4";
        private const string AuthHeaderKey = "X-Auth-Token";
        private const string API = "https://api.gotinder.com";

        // Until we figure out how to change this?
        private const string Locale = "en";

        private TinderOAuthToken auth;
        private string xAuthToken;
        private HttpClient rest;

        private bool authenticated = false;

        // Login via Facebook
        public TinderAPI()
        {
            rest = new HttpClient();
            rest.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }

        public async Task<Authentication> authenticateViaFacebook(TinderOAuthToken fb)
        {
            // Call Auth, if successful, else error 500
            var url = API.AppendPathSegment("auth");

            dynamic payload = new JObject();
            payload.facebook_token = fb.facebook_token;
            payload.facebook_id = fb.facebook_uid;
            payload.locale = Locale;

            // POST /auth HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode) {
                Authentication json = await RestHelpers.responseToObject<Authentication>(response);

                addXAuthHeader(json.token);

                return json;
            }

            return null;
        }

        public void authenticateViaXAuthToken(string xAuthToken)
        {
            addXAuthHeader(xAuthToken);
        }

        private void addXAuthHeader(string token)
        {
            // Precaution
            rest.DefaultRequestHeaders.Remove(AuthHeaderKey);

            // Modify instance varialbes
            this.xAuthToken = token;
            authenticated = true;

            // Augment HTTPClient
            rest.DefaultRequestHeaders.Add(AuthHeaderKey, token);
        }

        public async Task<Matches> getMatches()
        {
            var url = API.AppendPathSegment("recs").SetQueryParam("locale", "en");

            // GET /recs?locale=en HTTP/1.1
            HttpResponseMessage response = await rest.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Matches json = await RestHelpers.responseToObject<Matches>(response);
                return json;
            }

            return null;
        }

    }
}
