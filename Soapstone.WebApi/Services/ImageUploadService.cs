using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Soapstone.WebApi.Settings;

namespace Soapstone.WebApi.Services
{
    public class ImageUploadService
    {
        private string _uploadPath;
        private string _accessPath;

        public ImageUploadService(ImageUploadSettings settings)
        {
            _uploadPath = settings.UploadPath;
            _accessPath = settings.AccessPath;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var extension = file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
            var fileName = $"{Guid.NewGuid()}.{extension}";
            var path = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            return $"{_accessPath}/{fileName}";
        }
    }
}