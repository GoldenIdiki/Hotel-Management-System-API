using HotelManagement.Data.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Services.Interface
{
    public interface IFileUpload
    {
        UploadAvatarResponseDto UploadAvatar(IFormFile file);
        CloudinaryDotNet.Actions.DeletionResult DeleteAvatar(string publicId);
    }
}
