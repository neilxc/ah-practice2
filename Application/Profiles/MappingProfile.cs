using System.Linq;
using Domain;

namespace Application.Profiles
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, Profile>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.UserPhotos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}