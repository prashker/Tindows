using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindows.Externals
{
    class TinderAuthToken
    {
        public string facebook_token { get; }
        public string facebook_uid { get; }

        public TinderAuthToken(string token, string uid)
        {
            facebook_token = token;
            facebook_uid = uid;
        }
    }

    // TODO: Implement HTTP Client and calls!
    class TinderAPI
    {
        private TinderAuthToken auth;

        public TinderAPI(TinderAuthToken auth)
        {
            this.auth = auth;
        }
    }
}
