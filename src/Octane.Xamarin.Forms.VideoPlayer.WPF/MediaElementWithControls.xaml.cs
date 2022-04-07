using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Octane.Xamarin.Forms.VideoPlayer.WPF
{
  
    public partial class MediaElementWithControls : Grid
    {
        System.Windows.Threading.DispatcherTimer timer;
        bool isPlayed = false;
        bool canChangeSlider = true;
        bool isLeaveEventFired = false;
        public bool isFullScreen = false;
        public EventHandler<EventArgs> FullScreenChanged;

        public MediaElementWithControls()
        {
            this.InitializeComponent();
        }

        void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
        {
            isPlayed = true;
            InitializePropertyValues();
            mainMediaElement.Play();
        }

        void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
        {
            isPlayed = false;
            mainMediaElement.Pause();
        }

        void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {
            isPlayed = false;
            mainMediaElement.Stop();
        }

        void OnMouseDownFullScreen(object sender, MouseButtonEventArgs args)
        {
            if (isFullScreen)
            {
                Uri path = new Uri(@"images/fullscreen.png", UriKind.Relative);
                btnFullScreen.Source = new BitmapImage(path);
            }
            else
            {
                Uri path = new Uri(@"images/exitfullscreen.png", UriKind.Relative);
                btnFullScreen.Source = new BitmapImage(path);
            }
            isFullScreen = !isFullScreen;
            FullScreenChanged?.Invoke(sender, args);
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            mainMediaElement.Volume = (double)volumeSlider.Value;
        }

        private void Element_MediaOpened(object sender, EventArgs e)
        {
            isPlayed = true;
            timelineSlider.Maximum = (int)mainMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            tbDuration.Text = GetVideoTimeInString((int)mainMediaElement.NaturalDuration.TimeSpan.TotalSeconds);
            SetVideoGridSize();
            global::Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), UpdatePosition);
        }

        private bool UpdatePosition()
        {
            if (isPlayed)
            {
                int value = (int)mainMediaElement.Position.TotalSeconds;
                if (canChangeSlider)
                {
                    timelineSlider.Value = value;
                }
                tbCurrentPosition.Text = GetVideoTimeInString(value);
            }
            return isPlayed;
        }

        private void Element_MediaEnded(object sender, EventArgs e)
        {
            isPlayed = false;
            mainMediaElement.Stop();
        }

        void InitializePropertyValues()
        {
            mainMediaElement.Volume = (double)volumeSlider.Value;
        }

        private string GetVideoTimeInString(int duration)
        {
            int minuts = duration / 60;
            int seconds = duration % 60;
            if (minuts > 60)
            {
                int hours = minuts / 60;
                minuts -= hours * 60;
                string strTime = $"{hours.ToString("00")}:{minuts.ToString("00")}:{seconds.ToString("00")}";
                return strTime;
            }
            return $"{minuts.ToString("00")}:{seconds.ToString("00")}";
        }

        private void SetVideoGridSize()
        {
            double videoWidth = mainMediaElement.NaturalVideoWidth;
            double videoHeight = mainMediaElement.NaturalVideoHeight;
            double controlWidth = mainGrid.ActualWidth;
            double controlHeight = mainGrid.ActualHeight;
            if (controlWidth <= 0 || controlHeight <= 0 || videoWidth <= 0 || videoHeight <= 0)
            {
                return;
            }
            double aspectRatio = videoWidth / videoHeight;

            double newWidth, newHeight;

            if (controlHeight >= (controlWidth / aspectRatio))
            {
                newWidth = controlWidth;
                newHeight = controlWidth / aspectRatio;
            }
            else
            {
                newWidth = controlHeight * aspectRatio;
                newHeight = controlHeight;
            }
            grdForVideo.RowDefinitions[1].Height = new GridLength(newHeight);
            grdForVideo.ColumnDefinitions[1].Width = new GridLength(newWidth);

            if (controlWidth <= 700)
            {
                grdControls.Height = 100;
                timelineSlider.Width = 100;
                volumeSlider.Width = 50;
                grdControls.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                Grid.SetRow(spSeek, 1);
                Grid.SetColumn(spSeek, 0);
                Grid.SetColumnSpan(spSeek, 3);
                Grid.SetColumnSpan(spButtons, 2);
                Grid.SetColumn(spVolume, 2);
            }
            else
            {
                grdControls.Height = 50;
                timelineSlider.Width = 300;
                volumeSlider.Width = 100;
                grdControls.RowDefinitions[1].Height = new GridLength(0);
                Grid.SetRow(spSeek, 0);
                Grid.SetColumn(spSeek, 2);
                Grid.SetColumn(spButtons, 0);
                Grid.SetColumn(spVolume, 1);
            }
        }

        private void TimelineSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int SliderValue = (int)timelineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, SliderValue);
            mainMediaElement.Position = ts;
            canChangeSlider = true;
        }

        private void TimelineSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            canChangeSlider = false;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 100)
            {
                return;
            }
            SetVideoGridSize();
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            grdControls.Visibility = Visibility.Visible;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isLeaveEventFired)
            {
                isLeaveEventFired = true;
                global::Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(5), HideVideoPanel);
            }
        }

        private bool HideVideoPanel()
        {
            grdControls.Visibility = Visibility.Hidden;
            isLeaveEventFired = false;
            return false;
        }
    }
}
