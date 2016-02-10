using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
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

        Match _prevSelected = default(Match);
        Match _selected = default(Match);
        public Match Selected
        {
            get {
                if (_selected == null)
                    return _prevSelected;
                return _selected;
            }
            set
            {
                // May get fired too often
                // http://stackoverflow.com/questions/16194211/2-way-binding-in-wpf-calls-property-twice

                // Do not fire events if value == selected

                if (_selected == value)
                {
                    return;
                }


                Set(ref _prevSelected, _selected);
                Set(ref _selected, value);

                // Async
                fetchAdvancedProfileForSelected(value);
            }
        }

        // Advanced information about a selected user
        Result _selectedAdvanced;
        public Result SelectedAdvanced
        {
            get
            {
                return _selectedAdvanced;
            }
            set
            {
                Set(ref _selectedAdvanced, value);
            }
        }

        public async void fetchAdvancedProfileForSelected(Match m)
        {
            SelectedAdvanced = await TinderState.Instance.Api.getAdvancedProfile(m.person._id);
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

            Message response = await TinderState.Instance.Api.sendMessage(Selected._id, Text);
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
            this.UpdateLayout();
            if (this.Items.Count > 0)
                this.ScrollIntoView(this.Items[this.Items.Count - 1]);
            base.OnItemsChanged(e);
        }

    }

    public class PersistSelectedItemListView : ListView
    {
        protected override void OnItemsChanged(object e)
        {
            this.UpdateLayout();
            // Scroll to Top
            if (this.Items.Count > 0)
             this.ScrollIntoView(this.Items[0]);
            base.OnItemsChanged(e);
        }
    }

}