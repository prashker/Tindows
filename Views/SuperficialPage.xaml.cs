using Tindows.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Tindows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SuperficialPage : Page
    {
        public SuperficialPage()
        {
            this.InitializeComponent();
        }

        // strongly-typed view models enable x:bind
        public SuperficialPageViewModel ViewModel => this.DataContext as SuperficialPageViewModel;

        // When you click/tap the contact, go to the Info Page
        private void YesOrNoDisplay_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SuperficialPivot.SelectedIndex = 1;
        }
    }
}
