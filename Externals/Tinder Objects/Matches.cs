using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindows.Externals.Tinder_Objects
{
    public class Matches
    {
        public int status { get; set; }
        public List<AdvancedMatchInfo> results { get; set; }
    }

    public class Teaser
    {
        // "string" not valid variable name
        // Replaced to -> teaser
        [JsonProperty("string")]
        public string teaser { get; set; }

        public string type { get; set; }
    }

    public class InstagramPhotos
    {
        public string image { get; set; }
        public string thumbnail { get; set; }
        public string ts { get; set; }
        public string link { get; set; }
    }

    public class Instagram
    {
        public string username { get; set; }
        public string profile_picture { get; set; }
        public List<InstagramPhotos> photos { get; set; }
        public int media_count { get; set; }
        public string last_fetch_time { get; set; }
        public bool completed_initial_fetch { get; set; }
    }

    // This is a "queue" match
    public class AdvancedMatchInfo
    {
        public int distance_mi { get; set; }
        public List<object> common_connections { get; set; }
        public int connection_count { get; set; }
        public List<object> common_likes { get; set; }
        public List<object> common_interests { get; set; }
        public List<object> common_friends { get; set; }
        public string _id { get; set; }
        public string bio { get; set; }
        public string birth_date { get; set; }
        public int gender { get; set; }
        public string name { get; set; }
        public string ping_time { get; set; }
        public List<Photo> photos { get; set; }
        public bool is_traveling { get; set; }
        public bool is_super_like { get; set; }
        public List<Job> jobs { get; set; }
        public List<School> schools { get; set; }
        public Teaser teaser { get; set; }
        public string birth_date_info { get; set; }
        public Instagram instagram { get; set; }
        public string travel_location_name { get; set; }
        public string location_name { get; set; }
        public string location_proximity { get; set; }
        public List<object> uncommon_interests { get; set; }
        public List<object> badges { get; set; }

        public int Age
        {
            get
            {
                // Attempt to parse birth_date string
                // Subtract from current year to get age
                DateTime dt = DateTime.Parse(birth_date);
                return DateTime.Now.Year - dt.Year;
            }
        }

    }

    public class SuperLikes
    {
        public int remaining { get; set; }
        public int allotment { get; set; }
        public string resets_at { get; set; }
    }

    public class LikeResponse
    {
        public Match match { get; set; }
        public int likes_remaining { get; set; }
        public SuperLikes super_likes { get; set; } // If response was a superlike

        public Boolean IsSuperLike
        {
            get
            {
                return super_likes != null;
            }
        }
    }

    // When calling /user/{id}
    // Returns a AdvancedMatchInfo object in a small status wrapper
    public class MoreInfoResponse
    {
        public int status { get; set; }
        public AdvancedMatchInfo results { get; set; }
    }
}
