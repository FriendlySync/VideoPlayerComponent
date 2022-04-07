using System.Text;
using System.Windows;
using Octane.Xamarin.Forms.VideoPlayer.Interfaces;

namespace Octane.Xamarin.Forms.VideoPlayer.WPF
{
    /// <summary>
    /// Platform native methods needed in cross-platform code.
    /// </summary>
    /// <seealso cref="IPlatformFeatures" />
    internal class PlatformFeatures : IPlatformFeatures
    {
        #region Properties

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        public string PackageName => null;//=> Package.Current.Id.Name;

        #endregion

        #region Methods

        /// <summary>
        /// Hashes the specified value using SHA-1.
        /// </summary>
        /// <param name="value">The value to hash.</param>
        /// <returns>
        /// A base-64 encoded SHA-1 hash.
        /// </returns>
        public string HashSha1(string value)
        {
            //var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            //var byteData = Encoding.UTF8.GetBytes(value);
            //var buffer = CryptographicBuffer.CreateFromByteArray(byteData);
            //var hash = hashProvider.HashData(buffer);
            //return CryptographicBuffer.EncodeToHexString(hash);
            return null;
        }

        /// <summary>
        /// Exits the mobile application.
        /// </summary>
        public void Exit()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}