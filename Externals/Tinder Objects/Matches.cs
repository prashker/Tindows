﻿using Newtonsoft.Json;
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
        public List<Result> results { get; set; }
    }

    public class PhotosOfMatch
    {
        public string id { get; set; }
        public string url { get; set; }
        public List<ProcessedFile> processedFiles { get; set; }
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

    public class Result
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
        public List<PhotosOfMatch> photos { get; set; }
        public bool is_traveling { get; set; }
        public bool is_super_like { get; set; }
        public List<object> jobs { get; set; }
        public List<object> schools { get; set; }
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
}