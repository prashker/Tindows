using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindows.Externals.Tinder_Objects
{
    public class Updates
    {
        public List<Match> matches { get; set; }
        public List<string> blocks { get; set; }
        public List<object> lists { get; set; }
        public List<object> deleted_lists { get; set; }
        public string last_activity_date { get; set; }
    }

    // May be the same as Authentication.SelfPhoto
    public class Photo
    {
        public string url { get; set; }
        public List<ProcessedFile> processedFiles { get; set; }
        public string extension { get; set; }
        public string fileName { get; set; }
        public object main { get; set; }
        public double ydistance_percent { get; set; }
        public double yoffset_percent { get; set; }
        public double xoffset_percent { get; set; }
        public string id { get; set; }
        public double xdistance_percent { get; set; }
        public string shape { get; set; }
    }

    public class Person
    {
        public string _id { get; set; }
        public string bio { get; set; }
        public string birth_date { get; set; }
        public int gender { get; set; }
        public string name { get; set; }
        public string ping_time { get; set; }
        public List<Photo> photos { get; set; }
        public List<object> badges { get; set; }
    }

    public class Match
    {
        public string _id { get; set; }
        public bool closed { get; set; }
        public int common_friend_count { get; set; }
        public int common_like_count { get; set; }
        public string created_date { get; set; }
        public bool dead { get; set; }
        public string last_activity_date { get; set; }
        public int message_count { get; set; }
        public List<object> messages { get; set; }
        public List<string> participants { get; set; }
        public bool pending { get; set; }
        public bool is_super_like { get; set; }
        public bool following { get; set; }
        public bool following_moments { get; set; }
        public string id { get; set; }
        public Person person { get; set; }
        public string super_liker { get; set; }
    }
}
