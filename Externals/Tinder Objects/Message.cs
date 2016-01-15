using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindows.Externals.Tinder_Objects
{
    class Message
    {
        public string _id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string match_id { get; set; }
        public string sent_date { get; set; }
        public string message { get; set; }
        public string created_date { get; set; }

        // /updates/ call has another parameter
        public long timestamp { get; set; }
    }
}
