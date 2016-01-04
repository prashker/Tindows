using Tindows.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Tindows.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        // strongly-typed view models enable x:bind
        public LoginPageViewModel ViewModel => this.DataContext as LoginPageViewModel;
    }
}
