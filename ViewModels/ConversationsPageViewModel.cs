using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Tindows.Externals.Tinder_Objects;
using Tindows.Models;

namespace Tindows.ViewModels
{
    public class ConversationsPageViewModel : Mvvm.ViewModelBase
    {
        ObservableCollection<Match> _conversations;
        public ObservableCollection<Match> Conversations { get { return _conversations; }  set { Set(ref _conversations, value); } }

        Match _selected = default(Match);
        public object Selected
        {
            get { return _selected; }
            set
            {
                var convo = value as Match;
                Set(ref _selected, convo);
                //if (message != null)
                //   message.IsRead = true;

                foreach (Message m in convo.messages)
                {
                    Debug.WriteLine(m.message);
                }
            }
        }

        public ConversationsPageViewModel()
        {
            _conversations = TinderState.Instance.Updates.matches;
            Debug.WriteLine("loaded each time u initialize?");
        }
    }
}