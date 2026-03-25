using System.IO;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Uploads a file to Cloudinary
        /// </summary>
        /// <param name="fileStream">File content stream</param>
        /// <param name="fileName">Original file name</param>
        /// <param name="folder">Cloudinary folder path</param>
        /// <param name="resourceType">image, raw, video</param>
        /// <returns>Tuple of (PublicId, Version)</returns>
        Task<(string PublicId, string Version)> UploadAsync(
            Stream fileStream, 
            string fileName, 
            string folder, 
            string resourceType = "raw");

        /// <summary>
        /// Generates a URL for a resource
        /// </summary>
        /// <param name="publicId">The PublicID from Cloudinary</param>
        /// <param name="version">The version string</param>
        /// <param name="resourceType">image, raw, video</param>
        /// <param name="isSecure">Whether to generate a signed URL (for private files)</param>
        /// <returns>Full URL string</returns>
        string GetUrl(string publicId, string version, string resourceType = "raw", bool isSecure = false);
    }
}
