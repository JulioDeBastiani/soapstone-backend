using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Soapstone.WebApi.Settings;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Services
{
    public class ImageUploadService
    {
        private Cloudinary _cloudinary;

        public ImageUploadService(CloudinarySettings settings)
        {
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public Task<ImageViewModel> UploadImageAsync(IFormFile file)
            => Task.Run(() => UploadImage(file));

        private ImageViewModel UploadImage(IFormFile file)
        {
            var extension = file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
            var fileName = $"{Guid.NewGuid().ToString("N")}.{extension}";

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, stream)
                };

                var result = _cloudinary.Upload(uploadParams);

                return new ImageViewModel
                {
                    ImageUrl = $"res.cloudinary.com{result.SecureUri.AbsolutePath}",
                    Size = result.Length
                };
            }
        }
    }
}