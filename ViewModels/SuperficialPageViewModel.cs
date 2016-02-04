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
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Storage.Streams;
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
        public Queue<Result> m = new Queue<Result>();

        // Downloaded image for the main reviewed contact
        private ImageSource _mainImageForContact;
        public ImageSource MainImageForContact
        {
            get { return _mainImageForContact; }
            set
            {
                Set(ref _mainImageForContact, value);
                RaisePropertyChanged(nameof(MainImageForContact));
            }
        }

        private Result _currentlyReviewing;
        public Result CurrentlyReviewing
        {
            get { return _currentlyReviewing; }
            set
            {
                Set(ref _currentlyReviewing, value);
                RaisePropertyChanged(nameof(CurrentlyReviewing));
            }
        }

        //public ObservableCollection<Result> m = new ObservableCollection<Result>() ;x       

        public SuperficialPageViewModel()
        {
            Debug.WriteLine("loaded each time u initialize?");
        }

        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            m = await freshMeat();

            // Prepare the first :)
            prepareForReview(m.Dequeue());
        }

        private void prepareForReview(Result r)
        {
            // Given a candidate, prepare ViewModel for review

            setMatchPhotos(r);
            CurrentlyReviewing = r;
        }

        private async void setMatchPhotos(Result match)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(match.photos[0].url);
            byte[] img = await response.Content.ReadAsByteArrayAsync();
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            writer.WriteBytes(img);
            await writer.StoreAsync();
            BitmapImage b = new BitmapImage();
            b.SetSource(randomAccessStream);
            MainImageForContact = b;
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
                    // Todo: Super Like
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
            //return Math.Min(Math.Max(cumulativeDrag.X / -forEvery, -upTo), upTo);

            return 0.0;
        }

        private async void passCurrent()
        {
            // Pass on the currently reviewing
            Status response = await TinderState.Instance.api.pass(CurrentlyReviewing._id);

            if (m.Count > 0)
            {
                prepareForReview(m.Dequeue());
            }
            else
            {
                m = await freshMeat();
            }
        }

        private void likeCurrent()
        {
            //prepareForReview(m.Dequeue());
        }

        private async Task<Queue<Result>> freshMeat()
        {
            // Get new matches
            Matches matches = await TinderState.Instance.api.getMatches();
            return new Queue<Result>(matches.results);
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