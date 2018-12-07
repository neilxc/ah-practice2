using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Profiles;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class Delete
    {
        public class Command : IRequest
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            private readonly IProfileReader profileReader;
            public Handler(DataContext context, IUserAccessor userAccessor, IProfileReader profileReader)
            {
                this.profileReader = profileReader;
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var target = await context.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);

                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new {User = "Not Found"});
                
                var observer = await context.Users
                    .FirstOrDefaultAsync(x => x.UserName == userAccessor
                    .GetCurrentUsername(), cancellationToken);

                var followedPeople = await context.FollowedPeople
                    .FirstOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);
                
                if (followedPeople != null)
                {
                    context.Remove(followedPeople);
                    await context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}