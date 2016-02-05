using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tindows.Externals.Tinder_Objects
{
    public class Authentication
    {
        public string token { get; set; }
        public LocalUser user { get; set; }
        public Versions versions { get; set; }
        public Globals globals { get; set; }
    }

    public class CategoryList
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Interest
    {
        public string created_time { get; set; }
        public string id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public List<CategoryList> category_list { get; set; }
    }

    public class SelfPhoto
    {
        public string url { get; set; }
        public List<ProcessedFile> processedFiles { get; set; }
        public string extension { get; set; }
        public string fbId { get; set; }
        public string fileName { get; set; }
        public string shape { get; set; }
        public string main { get; set; }
        public string id { get; set; }
        public double? xdistance_percent { get; set; }
        public double? ydistance_percent { get; set; }
        public double? xoffset_percent { get; set; }
        public double? yoffset_percent { get; set; }
    }

    public class LocalUser
    {
        public string _id { get; set; }
        public string active_time { get; set; }
        public string create_date { get; set; }
        public int age_filter_max { get; set; }
        public int age_filter_min { get; set; }
        public string api_token { get; set; }
        public string bio { get; set; }
        public string birth_date { get; set; }
        public int connection_count { get; set; }
        public int distance_filter { get; set; }
        public string full_name { get; set; }
        public List<string> groups { get; set; }
        public int gender { get; set; }
        public int gender_filter { get; set; }
        public List<Interest> interests { get; set; }
        public string name { get; set; }
        public string ping_time { get; set; }
        public bool discoverable { get; set; }
        public List<SelfPhoto> photos { get; set; }
        public List<string> jobs { get; set; }
        public List<string> schools { get; set; }
        public List<string> purchases { get; set; }
    }

    public class Versions
    {
        public string active_text { get; set; }
        public string age_filter { get; set; }
        public string matchmaker { get; set; }
        public string trending { get; set; }
        public string trending_active_text { get; set; }
    }

    public class Globals
    {
        public bool friends { get; set; }
        public string invite_type { get; set; }
        public int recs_interval { get; set; }
        public int updates_interval { get; set; }
        public int recs_size { get; set; }
        public string matchmaker_default_message { get; set; }
        public string share_default_text { get; set; }
        public int boost_decay { get; set; }
        public int boost_up { get; set; }
        public int boost_down { get; set; }
        public bool sparks { get; set; }
        public bool kontagent { get; set; }
        public bool sparks_enabled { get; set; }
        public bool kontagent_enabled { get; set; }
        public bool mqtt { get; set; }
        public bool tinder_sparks { get; set; }
        public int moments_interval { get; set; }
        public bool fetch_connections { get; set; }
        public bool plus { get; set; }
    }

}
