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
        private const string API = "https://api.gotinder.com";

        // Until we figure out how to change this?
        private const string Locale = "en";

        private const string ContentType = "application/json";
        private Encoding Charset = Encoding.UTF8;

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

        public async Task<AuthRootObject> authenticateViaFacebook(TinderOAuthToken fb)
        {
            // Call Auth, if successful, else error 500
            var url = API.AppendPathSegment("auth");

            dynamic payload = new JObject();
            payload.facebook_token = fb.facebook_token;
            payload.facebook_id = fb.facebook_uid;
            payload.locale = Locale;

            // POST /auth HTTP/1.1
            HttpResponseMessage response = await rest.PostAsync(url, preparePayload(payload));

            if (response.IsSuccessStatusCode) {
                AuthRootObject json = await responseToObject<AuthRootObject>(response);

                xAuthToken = json.token;
                return json;
            }

            return null;
        }

        //http://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-new-rest-api-preview-release-4/6117969#6117969
        private StringContent preparePayload(JObject payload)
        {
            string json = payload.ToString(Newtonsoft.Json.Formatting.None);
            // http://stackoverflow.com/questions/6117101/
            return new StringContent(json, Charset, ContentType);
        }

        // JSON
        private async Task<T> responseToObject<T>(HttpResponseMessage r)
        {
            string content = await r.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

    }
}
