using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Tindows.Toasts;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Tindows.Externals.Tinder_Objects
{
    public class Updates
    {
        public SortedObservableCollection<Match> matches { get; set; }
        public List<string> blocks { get; set; }
        public List<string> lists { get; set; }
        public List<string> deleted_lists { get; set; }
        public string last_activity_date { get; set; }

        // New - TO DEAL WITH
        public List<object> liked_messages { get; set; }

        public Updates()
        {
            matches = new SortedObservableCollection<Match>();
            blocks = new List<string>();
            lists = new List<string>();
            deleted_lists = new List<string>();
            last_activity_date = "";
        }

        /// <summary>
        /// Augment the existing update with a new state
        /// </summary>
        /// <param name="newUpdate">The update to absorb</param>
        /// <param name="silent">Do not notify externally of changes</param>
        /// <param name="local_id">The ID associated with the local user</param>
        public void absorb(Updates newUpdate, Boolean silent, string local_id)
        {
            // Given an update, augment the existing one
            // Case 1 - isMatch() and hasMessages() - BACKLOG
            // Case 2 - hasMessages() - New Message Existing Constact
            // Case 3 - isMatch() - NEW CONSTACT

            foreach (Match m in newUpdate.matches)
            {
                if (m.isMatch())
                {
                    if (m.hasMessages())
                    {
                        // Case 1 - Backlog Messages
                        foreach (Message msg in m.messages)
                            msg.getImageOfGiphy();
                    }

                    // Add constact to state (with messages)
                    matches.Add(m);

                    if (!silent)
                    {
                        NewMatchToast.Do(m);
                    }
                }
                // Case 2 - New messages for exisiting user
                // We must find the "existing" user.
                else if (m.hasMessages()) {
                    for (int idx = 0; idx < matches.Count; idx++)
                    {
                        Match existing = matches[idx];
                        if (existing._id == m._id)
                        {
                            foreach (Message message in m.messages)
                            {
                                // Asynchronous
                                message.getImageOfGiphy();

                                // Add only if the last message isn't the same
                                // We already add messages we send instantly
                                //if (existing.messages.Last()._id != message._id)
                                //{
                                //    existing.messages.Add(message);
                                //}

                                existing.messages.Add(message);
                            }

                            // If it is an incoming message, do a toast
                            //if (m.messages.Last().from != local_id)
                            NewMessageToast.Do(existing, m.messages.Last());


                            // Propagate the changes up in the list if its not already at the top
                            //if (idx != 0)
                            //    matches.Move(idx, 0);
                        }
                    }
                }
            }
        }
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

    public class SimpleMatchInfo
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

    public class Match : BindableBase, IComparable
    {
        // Conversation ID between both users (OR?)
        public string _id { get; set; }

        public bool closed { get; set; }
        public int common_friend_count { get; set; }
        public int common_like_count { get; set; }
        public string created_date { get; set; }
        public bool dead { get; set; }
        public string last_activity_date { get; set; }
        public int message_count { get; set; }
        public ObservableCollection<Message> messages { get; set; }
        public List<string> participants { get; set; }
        public bool pending { get; set; }
        public bool is_super_like { get; set; }
        public bool following { get; set; }
        public bool following_moments { get; set; }
        public string id { get; set; }
        public SimpleMatchInfo person { get; set; }
        public string super_liker { get; set; }

        // New
        public bool is_new_message { get; set; }


        private DateTime _parsed_created_date = default(DateTime);
        public DateTime ParsedCreatedDate
        {
            get
            {
                if (_parsed_created_date == DateTime.MinValue)
                {
                    _parsed_created_date = DateTime.Parse(created_date);
                }
                return _parsed_created_date;
            }
        }

        public Boolean hasMessages()
        {
            return messages.Count > 0;
        }

        public Boolean isMatch()
        {
            return person != null;
        }

        public int CompareTo(object o)
        {
            Match other = o as Match;

            // Sort order:
            // Newest messages
            // Newest matches
            // Oldest Messages
            // Oldest Matches

            // Both have messages
            // Prioritize the one with the newest messages (descending)
            if (messages.Count > 0 && other.messages.Count > 0)
            {
                return other.messages.Last().ParsedSentDate.CompareTo(messages.Last().ParsedSentDate);
            }

            // Comparing both messageless messages
            // Prioritize created date (descending)
            if (messages.Count == 0 && other.messages.Count == 0)
            {
                return other.ParsedCreatedDate.CompareTo(this.ParsedCreatedDate);
            }

            // Comparing messageless messages (individual)
            // Prioritize the one with Max(created date, latest message date)
            if (messages.Count > 0)
                return other.ParsedCreatedDate.CompareTo(messages.Last().ParsedSentDate);
            else if (other.messages.Count > 0)
                return other.messages.Last().ParsedSentDate.CompareTo(this.ParsedCreatedDate);

            return 0;
        }
    }

    // Messages all are incoming through the form of Updates:{message}
    public class Message : BindableBase
    {
        public string _id { get; set; }
        public string match_id { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public string message { get; set; }
        public string sent_date { get; set; }
        public string created_date { get; set; }
        public long timestamp { get; set; }

        // New
        public string type { get; set; }
        public string fixed_height { get; set; }

        private DateTime _parsed_sent_date = default(DateTime);
        public DateTime ParsedSentDate
        {
            get
            {
                if (_parsed_sent_date == DateTime.MinValue)
                {
                    _parsed_sent_date = DateTime.Parse(sent_date);
                }
                return _parsed_sent_date;
            }
        }

        private string _messageImageURL;
        public string MessageImageURL
        {
            get {
                return _messageImageURL;
            }
            set
            {
                Set(ref _messageImageURL, value);
                RaisePropertyChanged(nameof(MessageImageURL));
            }
        }

        // New feature added since app was starting development
        public void getImageOfGiphy()
        {
            // Example
            // https://media1.giphy.com/media/tyttpHcuYVfzl0XisE0/giphy.gif?width=900&height=675

            // New JSON makes 
            // "type":"gif","fixed_height":"https://media1.giphy.com/media/mngbEYFPa3NbG/200.gif?width=356&height=200"}
            if (type != null && type.Equals("gif"))
            {
                MessageImageURL = message;
                message = null;
            }
            else
            {
                MessageImageURL = null;
            }
        }

    }
}
