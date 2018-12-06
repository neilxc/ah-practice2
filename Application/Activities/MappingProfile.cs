using AutoMapper;
using Domain;
using static Application.Activities.Create;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ActivityData, Activity>();
            CreateMap<Activity, ActivityDto>();
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName));
        }
    }
}