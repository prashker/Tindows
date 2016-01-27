using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Tindows.Externals.Tinder_Objects;
using Tindows.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tindows.ViewModels
{
    public class ConversationsPageViewModel : Mvvm.ViewModelBase
    {
        ObservableCollection<Match> _conversations;
        public ObservableCollection<Match> Conversations { get { return _conversations; }  set { Set(ref _conversations, value); } }

        Match _selected = default(Match);
        public Match Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
            }
        }

        string _text = default(string);
        public string Text
        {
            get { return _text; }
            set
            {
                Set(ref _text, value);
                Debug.WriteLine(value);
            }
        }

        public ConversationsPageViewModel()
        {
            _conversations = TinderState.Instance.Updates.matches;
            Debug.WriteLine("loaded each time u initialize?");
        }

        public async void sendMessage()
        {
            // Get the contact we're sending to
            // Construct a message
            // Augment the Collection<Message>

            Message response = await TinderState.Instance.api.sendMessage(Selected._id, Text);
            //Selected.messages.Add(response);
            Text = "";
        }

    }

    // Todo: Open Source
    // https://github.com/rajat1saxena/chatbubbles
    public class ChatBubbleSelector : DataTemplateSelector
    {
        public DataTemplate toBubble { get; set; }
        public DataTemplate fromBubble { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var message = item as Message;

            if (message.from == TinderState.Instance.Me._id)
            {
                return fromBubble;
            }
            return toBubble;
        }
    }

    // http://stackoverflow.com/questions/16866309/listbox-scroll-into-view-with-mvvm
    // This is a HACK
    public class AutoScrollListBox : ListView
    {
        protected override void OnItemsChanged(object e)
        {
            /*
            int newItemCount = e.NewItems.Count;

            if (newItemCount > 0)
                this.ScrollIntoView(e.NewItems[newItemCount - 1]);
            */

            ObservableCollection<Message> mS = (ObservableCollection<Message>)this.ItemsSource;

            this.ScrollIntoView(mS[mS.Count - 1]);

            

            base.OnItemsChanged(e);
        }
    }

}