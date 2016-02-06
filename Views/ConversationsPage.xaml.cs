using System.Collections.ObjectModel;
using System.Diagnostics;
using Tindows.Externals.Tinder_Objects;
using Tindows.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void TindowsChat_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("g222o");
        }
    }
}
