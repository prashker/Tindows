using System.Collections.ObjectModel;
using System.Diagnostics;
using Tindows.Externals.Tinder_Objects;
using Tindows.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tindows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConversationsPage : Page
    {
        public ConversationsPage()
        {
            this.InitializeComponent();
        }

        // strongly-typed view models enable x:bind
        public ConversationsPageViewModel ViewModel => this.DataContext as ConversationsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If we passed a conversation_id to this, navigate straight to it, if possible

            string open_directly = e.Parameter?.ToString();

            if (open_directly != null)
            {
                foreach (Match m in ViewModel.Conversations)
                {
                    if (m._id == open_directly)
                    {
                        ViewModel.Selected = m;
                        break;
                    }
                }
            }
        }

    }
}
