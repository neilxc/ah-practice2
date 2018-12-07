using System;
using System.Net;
using Application.Errors;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class CloudinaryAccessor : ICloudinaryAccessor
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryAccessor(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            var acc = new Account(
                cloudinaryConfig.Value.Cloudname,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public UserPhoto AddPhotoForUser(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }

            }

            var photo = new UserPhoto
            {
                DateAdded = DateTime.Now,
                IsMain = false,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUri.ToString()
            };

            return photo;
        }

        public string DeletePhotoForUser(UserPhoto photo)
        {
            var deleteParams = new DeletionParams(photo.PublicId);

            var result = _cloudinary.Destroy(deleteParams);

            if (result.Result == "ok")
            {
                return result.Result;
            }

            throw new RestException(HttpStatusCode.BadRequest);
        }
    }
}