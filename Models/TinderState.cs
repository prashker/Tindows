using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;

namespace Tindows.Models
{
    /// <summary>
    /// Singleton class for maintaining state for the entire application :)
    /// Copies singleton logic from Template10 convention
    /// </summary>
    class TinderState
    {
        // Singleton
        public static TinderState Instance { get; }

        private Authentication profileInfo;
        public TinderAPI api { get; }

        // Maintain state for last time we called getUpdates()
        private string last_activity_date = "";


        Updates _updates;
        public Updates Updates { get { return _updates; } set { _updates = value; } }


        static TinderState()
        {
            // implement singleton pattern
            Instance = Instance ?? new TinderState();
        }

        private TinderState()
        {
            api = new TinderAPI();
        }

        public async void getInitialState()
        {
            // Get updates starting from the beginning of time
            Updates = await api.getUpdates("");

            last_activity_date = Updates.last_activity_date;
        }

        public async void getLatestUpdates()
        {
            // Call getUpdates(), update latest_update_fetch

            Updates temp = await api.getUpdates(last_activity_date);
            last_activity_date = temp.last_activity_date;

            // Merge matches from both Updates
            foreach (Match m in temp.matches)
            {
                Updates.matches.Add(m);
            }
        }
    }



}

