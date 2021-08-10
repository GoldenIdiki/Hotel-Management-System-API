using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelManagement.Data.Dtos;
using HotelManagement.Data.Services.Interface;
using HotelManagement.Data.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Services.Implementation
{
    public class FileUpload : IFileUpload
    {
        private readonly CloudinaryConfig _config;
        public Cloudinary _cloudinary { get; set; }

        public FileUpload(IOptions<CloudinaryConfig> config)
        {
            _config = config.Value;
            Account account = new Account
            (
                _config.CloudName,
                _config.ApiKey,
                _config.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public UploadAvatarResponseDto UploadAvatar(IFormFile file)
        {
            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fs),
                    Transformation = new Transformation().Width(300).Height(300)
                                                         .Crop("fill").Gravity("face")
                };
                imageUploadResult = _cloudinary.Upload(imageUploadParams);
            }
            var publicId = imageUploadResult.PublicId;
            var avatarUrl = imageUploadResult.Url.ToString();
            var result = new UploadAvatarResponseDto
            {
                PublicId = publicId,
                AvatarUrl = avatarUrl
            };
            return result;
        }

        public DeletionResult DeleteAvatar(string publicId)
        {
            var delParams = new DeletionParams(publicId) { ResourceType = ResourceType.Image };
            return _cloudinary.Destroy(delParams);
        }
    }
}
