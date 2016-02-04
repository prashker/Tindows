using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;
using Tindows.Toasts;

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

        LocalUser _me;
        public LocalUser Me { get { return _me; } set { _me = value; } }

        private Boolean looping = false;


        static TinderState()
        {
            // implement singleton pattern
            Instance = Instance ?? new TinderState();
        }

        private TinderState()
        {
            api = new TinderAPI();
        }

        public async void getProfileInfo()
        {
            Me = await api.me();
        }

        public async void prepareInitialState()
        {
            // Get updates starting from the beginning of time
            Updates = await api.getUpdates("");

            last_activity_date = Updates.last_activity_date;
        }

        public async Task<Updates> getLatestUpdates()
        {
            // Call getUpdates(), update latest_update_fetch

            Updates temp = await api.getUpdates(last_activity_date);
            last_activity_date = temp.last_activity_date;

            return temp;
        }

        // Is this the right way to do this?
        public async void startUpdatesLoop()
        {
            if (!looping)
            {
                looping = true;
                while (true)
                {
                    // Every 3 seconds
                    await Task.Delay(2000);

                    Updates newUpdate = await getLatestUpdates();


                    // Merge matches from both Updates
                    // New messages are intersperced in here

                    // Future: Offset to Updates.absorb()
                    foreach (Match m in newUpdate.matches)
                    {
                        if (m.isMessage())
                        {
                            for (int idx = 0; idx < Updates.matches.Count; idx++)
                            {
                                Match existing = Updates.matches[idx];

                                if (existing._id == m._id)
                                {
                                    foreach (Message message in m.messages)
                                    {
                                        // Add only if the last message isn't the same
                                        // We already add messages we send instantly
                                        //if (existing.messages.Last()._id != message._id)
                                        //{
                                        //    existing.messages.Add(message);
                                        //}

                                        existing.messages.Add(message);
                                    }

                                    // Propagate the changes up in the list
                                    Updates.matches.Move(idx, 0);
                                }
                            }
                        }
                        if (m.isMatch())
                        {
                            Updates.matches.Add(m);
                            PassToast.Do("Matched!", "You have matched " + m.person.name, "Chat em up!");
                        }
                    }

                }
            }
        }
    }



}

