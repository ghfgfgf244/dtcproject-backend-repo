using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dtc.Application.Interfaces;
using dtc.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dtc.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<(string PublicId, string Version)> UploadAsync(
            Stream fileStream, 
            string fileName, 
            string folder, 
            string resourceType = "raw")
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folder,
                PublicId = fileName.Split('.')[0] // Basic public ID from filename
            };

            // If it's an image, use ImageUploadParams for better processing
            if (resourceType == "image")
            {
                var imageParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = folder
                };
                var imageResult = await _cloudinary.UploadAsync(imageParams);
                if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);
                return (imageResult.PublicId, imageResult.Version);
            }

            // Default Raw (PDF, etc)
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null) throw new Exception(result.Error.Message);
            
            return (result.PublicId, result.Version);
        }

        public string GetUrl(string publicId, string version, string resourceType = "raw", bool isSecure = false)
        {
            // For simple public URLs
            if (!isSecure)
            {
                return _cloudinary.Api.UrlImgUp
                    .ResourceType(resourceType)
                    .Version(version)
                    .BuildUrl(publicId);
            }

            // For secure/signed URLs (simplified version)
            // Note: In a real app, you might use PrivateResource or specific transformations
            return _cloudinary.Api.UrlImgUp
                .ResourceType(resourceType)
                .Version(version)
                .Format("pdf") // Default to pdf for secure docs if raw
                .Secure(true)
                .BuildUrl(publicId);
        }
    }
}
