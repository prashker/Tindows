using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Flurl;
using Newtonsoft.Json;

namespace Tindows.Externals
{
    public class FBAuthenticationError : Exception { }

    /// <summary>
    /// Class responsible for preparing authentication information for Tinder - Either new account or existing login.
    /// </summary>
    class FBAuthTinder
    {
        // Follows the logic for manually building a login flow
        // https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
        // Browser Exclusive - No Facebook "App" Required
        // Tinder Requires: AuthToken (Through OAuth Login) and UserID (from GraphAPI)

        private static string FacebookAuth = "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}&display=popup&response_type=token";
        private static string CallbackURL = "https://www.facebook.com/connect/login_success.html";
        private static string GraphAPIUrl = "https://graph.facebook.com/v2.5/";

        /// <summary>
        /// Parses out the authentication token from the OAuth response
        /// </summary>
        /// <param name="response">
        /// OAuth response from FB Login
        /// </param>
        /// <returns>
        /// Extracted token
        /// </returns>
        private string extractTokenFromResponse(string response)
        {
            // Expected response is of the form
            // https://www.facebook.com/connect/login_success.html#access_token=[PARSE-FOR-THIS]&expires_in=5754
            // We need to extract the #access_token=[] portion?

            // End the pattern at the first &

            var pattern = new System.Text.RegularExpressions.Regex("access_token=([^&]+)");

            var m = pattern.Match(response);

            if (m.Success)
            {
                return m.Groups[1].Captures[0].Value;
            }

            throw new FBAuthenticationError();
        }


        /// <summary>
        /// Internal function for acquiring an AuthToken from Facebook
        /// </summary>
        private async Task<string> getTokenFromFacebook()
        {
            // Tinder's APP ID
            var app_id = "464891386855067";

            // What fields does Tinder need access to from Facebook to fully function?
            var scope = "public_profile, email";

            var url = string.Format(FacebookAuth, app_id, CallbackURL, scope);

            Uri startUri = new Uri(url);
            Uri endUri = new Uri(CallbackURL);
            
            // Login to Facebook and ask for permission to Tinder
            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);

            return extractTokenFromResponse(result.ResponseData);
        }

        private async Task<string> getUidFromFacebook(string authToken)
        {
            // TODO: COMMENT
            var client = new HttpClient();

            var prepared_url = GraphAPIUrl.AppendPathSegment("me").SetQueryParam("access_token", authToken);

            // TODO: Catch Exceptions??
            var response = await client.GetStringAsync(prepared_url);

            dynamic json = JsonConvert.DeserializeObject<dynamic>(response);

            return json.id;
        }

        public async Task<TinderOAuthToken> authenticateForTinder()
        {
            var token = await getTokenFromFacebook();
            var uid = await getUidFromFacebook(token);

            return new TinderOAuthToken(token, uid);
        }
    }
}
