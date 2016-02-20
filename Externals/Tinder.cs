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
    public class TinderOAuthToken
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
    public class TinderAPI
    {
        // Headers - Spoofing Tinder Android
        private const string UserAgent = "Tinder/4.6.1 (iPhone; iOS 9.0.1; Scale/2.00)";
        private const string AuthHeaderKey = "X-Auth-Token";
        private const string AppVersion = "371";
        private const string OSVersion = "90000000001";
        private const string Platform = "android";

        // API
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

            // Optional (for now), seems Tinder doesn't enforce these
            rest.DefaultRequestHeaders.Add("app-version", AppVersion);
            rest.DefaultRequestHeaders.Add("os-version", OSVersion);
            rest.DefaultRequestHeaders.Add("platform", Platform);
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

            // Augment HTTPClient
            rest.DefaultRequestHeaders.Add(AuthHeaderKey, token);
        }

        public async Task<Updates> getUpdates(string last_activity_date)
        {
            var url = API.AppendPathSegment("updates");
            dynamic payload = new JObject();
            payload.last_activity_date = last_activity_date;

            // POST /updates HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                Updates json = await RestHelpers.responseToObject<Updates>(response);
                return json;
            }

            return null;
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

        public async Task<Message> sendMessage(string conversation_id, string content)
        {
            // conversation_id is merely the concatenation of:
            // Message.to + Message.from
            var url = API.AppendPathSegments(new string[] { "user", "matches", conversation_id });
            dynamic payload = new JObject();
            payload.message = content;

            // POST /user/matches/{_id} HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                Message json = await RestHelpers.responseToObject<Message>(response);
                return json;
            }

            return null;
        }

        public async Task<Ping> setLocation(double lat, double lon)
        {
            var url = API.AppendPathSegments(new string[] { "user", "ping" });
            dynamic payload = new JObject();
            payload.lat = lat;
            payload.lon = lon;

            // POST /user/ping HTTPP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                Ping json = await RestHelpers.responseToObject<Ping>(response);
                return json;
            }

            return null;
        }

        public async Task<LocalUser> me()
        {
            // Assuming we're already logged in but want our Authentication object

            // Call Auth, if successful, else error 500
            var url = API.AppendPathSegment("profile");

            dynamic payload = new JObject();
            // See line #467 of TinderPartTwo-FINAL.saz
            payload.discoverable = true;


            // POST /auth HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                LocalUser json = await RestHelpers.responseToObject<LocalUser>(response);
                return json;
            }

            return null;
        }

        public async Task<Status> pass(string id_to_pass)
        {
            // Given the ID of the user to pass, 
            var url = API.AppendPathSegments(new string[] { "pass", id_to_pass });

            dynamic payload = new JObject();

            // POST /pass/{id} HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                Status json = await RestHelpers.responseToObject<Status>(response);
                return json;
            }

            return null;
        }

        public async Task<LikeResponse> like(string id_to_like)
        {
            // Given the ID of the user to like, 
            var url = API.AppendPathSegments(new string[] { "like", id_to_like });

            // GET /like/{id} HTTP/1.1
            HttpResponseMessage response = await rest.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                LikeResponse json = await RestHelpers.responseToObject<LikeResponse>(response);
                return json;
            }

            return null;
        }

        public async Task<LikeResponse> superlike(string id_to_like)
        {
            // Given the ID of the user to like, 
            var url = API.AppendPathSegments(new string[] { "like", id_to_like, "super"});

            dynamic payload = new JObject();

            // POST /like/{id}/super HTTP/1.1
            // Why the superlike is a POST and like is GET, we'll never know!
            HttpResponseMessage response = await rest.PostAsync(url, RestHelpers.preparePayload(payload));

            if (response.IsSuccessStatusCode)
            {
                LikeResponse json = await RestHelpers.responseToObject<LikeResponse>(response);
                return json;
            }

            return null;
        }

        public async Task<AdvancedMatchInfo> getAdvancedProfile(string id_of_user)
        {
            // Given the ID of the user to like, 
            var url = API.AppendPathSegments(new string[] { "user", id_of_user });

            // POST /user/{id} HTTP/1.1
            HttpResponseMessage response = await rest.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                MoreInfoResponse json = await RestHelpers.responseToObject<MoreInfoResponse>(response);
                return json.results;
            }

            return null;
        }

        // getCommonConnections()
        // To Be Determined
    }
}
