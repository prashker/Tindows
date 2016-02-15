using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Tindows.Externals;
using Tindows.Externals.Tinder_Objects;
using Tindows.Models;
using Tindows.Toasts;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Tindows.ViewModels
{
    public class SuperficialPageViewModel : Mvvm.ViewModelBase
    {
        enum PhotoPosition
        {
            OffscreenLeft,
            OffscreenRight,
            OffscreenTop,
            Other
        };

        public Matches matches { get; set; }

        // Model for View
        public Queue<AdvancedMatchInfo> m = new Queue<AdvancedMatchInfo>();

        // Image for the main reviewed contact
        // Needs to only be a URL, resolves issue #1
        // https://github.com/prashker/Tindows/issues/1
        private string _mainImageUrlForContact = "ms-appx:///Assets/LoadingMatches.png";
        public string MainImageUrl
        {
            get
            {
                return _mainImageUrlForContact;
            }
            set
            {
                Set(ref _mainImageUrlForContact, value);
                RaisePropertyChanged(nameof(MainImageUrl));
            }
        }

        private AdvancedMatchInfo _currentlyReviewing;
        public AdvancedMatchInfo CurrentlyReviewing
        {
            get { return _currentlyReviewing; }
            set
            {
                Set(ref _currentlyReviewing, value);
                RaisePropertyChanged(nameof(CurrentlyReviewing));
            }
        }

        // Maintain the information on superlikes
        private SuperLikes _superLikeStats;
        private SuperLikes SuperLikeStats
        {
            get
            {
                return _superLikeStats;
            }
            set
            {
                Set(ref _superLikeStats, value);
            }
        }

        //public ObservableCollection<AdvancedMatchInfo> m = new ObservableCollection<AdvancedMatchInfo>() ;x       

        public SuperficialPageViewModel()
        {
            Debug.WriteLine("loaded each time u initialize?");
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            m = await freshMeat();

            next();
        }

        public void photoDragStart(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Grid photo = (Grid)sender;
        }

        public void photoDragged(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Grid photo = (Grid)sender;
            CompositeTransform transforms = (CompositeTransform)photo.RenderTransform;

            // Move the image
            transforms.TranslateX += e.Delta.Translation.X;
            transforms.TranslateY += e.Delta.Translation.Y;

            // Rotate the image
            double angle = calculateImageAngle(e.Cumulative.Translation);
            transforms.Rotation = angle;

            if (e.IsInertial)
            {
                PhotoPosition m = getPhotoPosition(photo);

                if (m == PhotoPosition.OffscreenLeft)
                {
                    e.Complete();
                    passCurrent();
                }
                else if (m == PhotoPosition.OffscreenTop)
                {
                    e.Complete();
                    superlikeCurrent();
                }
                else if (m == PhotoPosition.OffscreenRight)
                {
                    e.Complete();
                    likeCurrent();
                }
            }
        }

        public void photoDragEnd(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Grid photo = (Grid)sender;

            // Reset
            CompositeTransform transforms = (CompositeTransform)photo.RenderTransform;
            transforms.TranslateX = 0;
            transforms.TranslateY = 0;
            transforms.Rotation = 0;

            // Future: Submit something to API?
        }

        /// <summary>
        ///     Determine how much the image should be tilted based on how much we've manipulated it
        /// </summary>
        /// <param name="cumulativeDrag">
        ///     Delta of drag
        /// </param>
        /// <returns>
        ///     Angle of rotation required
        /// </returns>
        private double calculateImageAngle(Point cumulativeDrag)
        {
            // Algorithm logic for rotating the image

            // for every 6 pixels you move away from the origin
            int forEvery = 10;
            // tilt the image 1 degree, up to +- 15 degrees
            int upTo = 15;

            // Negate forEvery to make the tilt angle reversed
            return Math.Min(Math.Max(cumulativeDrag.X / -forEvery, -upTo), upTo);
        }

        public async void passCurrent()
        {
            if (CurrentlyReviewing == null)
                return;

            // Pass on the currently reviewing
            Status response = await TinderState.Instance.Api.pass(CurrentlyReviewing._id);

            //PassToast.Do("You passed on " + CurrentlyReviewing.name, "", "");

            next();
        }

        public async void likeCurrent()
        {
            if (CurrentlyReviewing == null)
                return;

            LikeResponse response = await TinderState.Instance.Api.like(CurrentlyReviewing._id);

            next();
        }

        public async void superlikeCurrent()
        {
            if (CurrentlyReviewing == null)
                return;

            LikeResponse response = await TinderState.Instance.Api.superlike(CurrentlyReviewing._id);

            if (response.IsSuperLike)
            {
                SuperLikeStats = response.super_likes;
            }

            next();
        }

        private async void next()
        {
            if (m.Count > 0)
            {
                // Given a candidate, prepare ViewModel for review
                AdvancedMatchInfo r = m.Dequeue();

                if (r.photos.Count > 0)
                    MainImageUrl = r.photos[0].url;
                CurrentlyReviewing = r;
            }
            else
            {
                m = await freshMeat();
            }
        }

        private async Task<Queue<AdvancedMatchInfo>> freshMeat()
        {
            // Get new matches
            Matches matches = await TinderState.Instance.Api.getMatches();

            if (matches == null || matches.message != null)
            {
                return new Queue<AdvancedMatchInfo>();
            }

            return new Queue<AdvancedMatchInfo>(matches.results);
        }

        private PhotoPosition getPhotoPosition(Grid g)
        {
            var ttv =  g.TransformToVisual(Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            if (screenCoords.X - g.ActualWidth < g.ActualWidth * -2)
            {
                return PhotoPosition.OffscreenLeft;
            }

            if (screenCoords.X > Window.Current.Bounds.Width)
            {
                return PhotoPosition.OffscreenRight;
            }

            if (screenCoords.Y - g.ActualHeight < g.ActualHeight * -2)
            {
                return PhotoPosition.OffscreenTop;
            }

            return PhotoPosition.Other;
        }

    }


    /// TODO: http://stackoverflow.com/questions/6177499/how-to-determine-the-background-color-of-document-when-there-are-3-options-usin/6185448#6185448

}