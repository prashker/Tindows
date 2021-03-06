﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tindows.Externals
{
    static class RestHelpers
    {
        private const string ContentType = "application/json";

        //http://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-new-rest-api-preview-release-4/6117969#6117969
        public static StringContent preparePayload(JObject payload)
        {
            string json = payload.ToString(Newtonsoft.Json.Formatting.None);
            // http://stackoverflow.com/questions/6117101/
            return new StringContent(json, Encoding.UTF8, ContentType);
        }

        // JSON
        public static async Task<T> responseToObject<T>(HttpResponseMessage r)
        {
            string content = await r.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings()
            { 
                Error = HandleDeserializationError
            });

            /*
            For handling invalid/unexpected responses?
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                return default(T);
            }
            */
        }

        // http://stackoverflow.com/questions/26107656/ignore-parsing-errors-during-json-net-data-parsing
        private static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            // Todo: Log the errors in the global state!
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
