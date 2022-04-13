using System;
using System.ComponentModel;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using Octane.Xamarin.Forms.VideoPlayer;
using Octane.Xamarin.Forms.VideoPlayer.Interfaces;
using Octane.Xamarin.Forms.VideoPlayer.WPF.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(VideoPlayerRenderer))]
namespace Octane.Xamarin.Forms.VideoPlayer.WPF.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, MediaElementWithControls>, IVideoPlayerRenderer
    {

        MediaElementWithControls _mediaElementWithControls;
        MediaElement _mediaElement;
        System.Windows.Threading.DispatcherTimer timer;
        bool isMuted;

        public object UpdateSliderPosition { get; private set; }

        #region IVideoPlayerRenderer
        public void Play()
        {
            if (CanPlay())
            {
                _mediaElement.Play();
            }
        }

        public bool CanPlay()
        {
            return _mediaElement != null;
        }

        public void Pause()
        {
            if (CanPause())
            {
                _mediaElement.Pause();
            }
        }

        public bool CanPause()
        {
            return _mediaElement != null && _mediaElement.CanPause;
        }

        public void Stop()
        {
            if (CanStop())
            {
                _mediaElement.Stop();
            }
        }

        public bool CanStop()
        {
            return _mediaElement != null;
        }

        public void Seek(int value)
        {
            if (CanSeek(value))
            {
                _mediaElement.Position = TimeSpan.FromSeconds(value);
            }
        }

        public bool CanSeek(int time)
        {
            var absoluteTime = Math.Abs(time);
            return _mediaElement != null &&
                ((time > 0 && (_mediaElement.Position.Add(TimeSpan.FromSeconds(absoluteTime)) <= _mediaElement.NaturalDuration))
                || (time < 0 && (_mediaElement.Position.Subtract(TimeSpan.FromSeconds(absoluteTime)) >= TimeSpan.Zero)));
        }
        #endregion

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                _mediaElementWithControls = new MediaElementWithControls();
                _mediaElement = _mediaElementWithControls.mainMediaElement;
                FileVideoSource source = (FileVideoSource)Element.Source;
                _mediaElement.Source = new Uri("file:///" + source.File);
                _mediaElement.LoadedBehavior = MediaState.Manual;
                //_mediaElement.MediaOpened += MediaElement_MediaOpened;
                //_mediaElement.MediaEnded += MediaElement_MediaEnded;
                SetNativeControl(_mediaElementWithControls);
                UpdateDisplayControls();
                _mediaElement.Play();
                _mediaElementWithControls.FullScreenChanged += ChangeFullScreen;
            }
        }

        //private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    Element.OnEnded?.Invoke();
        //}

        //private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        //{
        //    Element.VideoHeight = _mediaElement.NaturalVideoHeight;
        //    Element.VideoWidth = _mediaElement.NaturalVideoWidth;
        //    Element.OnOpened?.Invoke();
        //}

        private void ChangeFullScreen(object sender, EventArgs e)
        {
            Element.OnChangeFullScreen?.Invoke(_mediaElementWithControls.isFullScreen);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null) return;

            if (e.PropertyName == VideoPlayer.AutoPlayProperty.PropertyName)
            {
                UpdateAutoPlay();
            }
            else if (e.PropertyName == VideoPlayer.DisplayControlsProperty.PropertyName)
            {
                UpdateDisplayControls();
            }
            else if (e.PropertyName == VideoPlayer.FillModeProperty.PropertyName)
            {
                UpdateFillMode();
            }
            else if (e.PropertyName == VideoPlayer.TimeElapsedIntervalProperty.PropertyName)
            {
                UpdateTimeElapsedInterval();
            }
            else if (e.PropertyName == VideoPlayer.VolumeProperty.PropertyName)
            {
                UpdateVolume();
            }
            else if (e.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                await UpdateSource();
            }
            else if (e.PropertyName == VideoPlayer.RepeatProperty.PropertyName)
            {
                UpdateRepeat();
            }
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
            {
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            Control.Visibility = Element.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateRepeat()
        {
        }

        private Task UpdateSource()
        {
            FileVideoSource source = (FileVideoSource)Element.Source;
            _mediaElement.Source = new Uri("file:///" + source.File);
            return Task.CompletedTask;
        }

        private void UpdateVolume()
        {
            int volume = Element.Volume;
            _mediaElement.Volume = volume;
        }

        private void UpdateTimeElapsedInterval()
        {
            
        }

        private void UpdateFillMode()
        {
            
        }

        private void UpdateDisplayControls()
        {
            if (Element.DisplayControls)
            {
                _mediaElementWithControls.grdControls.Visibility = Visibility.Visible;
            }
            else
            {
                _mediaElementWithControls.grdControls.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateAutoPlay()
        {
            
        }
    }
}
