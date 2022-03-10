using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using ChillPlayer.Web;
using Octane.Xamarin.Forms.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// http://www.genyoutube.net/formats-resolution-youtube-videos.html

namespace ChillPlayer.MarkupExtensions
{
    /// <summary>
    /// Converts a YouTube video ID into a direct URL that is playable by the Xamarin Forms VideoPlayer.
    /// </summary>
    [ContentProperty("VideoId")]
    public class YouTubeVideoIdExtension : IMarkupExtension
    {
        #region Properties

        /// <summary>
        /// The video identifier associated with the video stream.
        /// </summary>
        /// <value>
        /// The video identifier.
        /// </value>
        public string VideoId { get; set; }

        #endregion

        #region IMarkupExtension

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return VideoSource.FromUri("http://vjs.zencdn.net/v/oceans.mp4");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the specified video ID into a streamable YouTube URL.
        /// </summary>
        /// <param name="videoId">Video identifier.</param>
        /// <returns></returns>
        public static VideoSource Convert(string videoId)
        {
            var markupExtension = new YouTubeVideoIdExtension { VideoId = videoId };
            return (VideoSource)markupExtension.ProvideValue(null);
        }

        #endregion
    }
}
