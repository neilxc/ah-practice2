using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly DataContext context;
        private readonly IUserAccessor userAccessor;
        private readonly IMapper mapper;
        public ProfileReader(DataContext context, IUserAccessor userAccessor, IMapper mapper)
        {
            this.mapper = mapper;
            this.userAccessor = userAccessor;
            this.context = context;
        }
        public async Task<Profile> ReadProfile(string username)
        {
            var currentUserName = userAccessor.GetCurrentUsername();

            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, new {User = "Not found"});
            
            var profile = mapper.Map<AppUser, Profile>(user);

            var currentUser = await context.Users
                .Include(x => x.Following)
                .Include(x => x.Followers)
                .FirstOrDefaultAsync(x => x.UserName == currentUserName);
            
            if (currentUser.Followers.Any(x => x.TargetId == user.Id))
                profile.IsFollowed = true;

            return profile;
        }
    }
}