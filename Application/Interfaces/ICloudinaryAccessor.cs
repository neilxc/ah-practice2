using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICloudinaryAccessor
    {
         UserPhoto AddPhotoForUser(IFormFile file);
         string DeletePhotoForUser(UserPhoto photo);
    }
}