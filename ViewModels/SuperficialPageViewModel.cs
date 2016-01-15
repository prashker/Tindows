using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;
using Tindows.Models;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Navigation;

namespace Tindows.ViewModels
{
    public class SuperficialPageViewModel : Mvvm.ViewModelBase
    {
        public Matches matches { get; set; }

        public ObservableCollection<Result> m = new ObservableCollection<Result>();

        public SuperficialPageViewModel()
        {
            Debug.WriteLine("loaded each time u initialize?");
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            matches = await TinderState.Instance.api.getMatches();

            foreach (Result poo in matches.results)
            {
                m.Add(poo);
            }

        }

    }
}