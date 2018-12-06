using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
  public class Details
  {
    public class Query : IRequest<User> { }

    public class Handler : IRequestHandler<Query, User>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      private readonly IJwtGenerator jwtGenerator;
      public Handler(DataContext context, IUserAccessor userAccessor, IJwtGenerator jwtGenerator)
      {
        this.jwtGenerator = jwtGenerator;
        this.userAccessor = userAccessor;
        this.context = context;
      }
      public async Task<User> Handle(Query request, CancellationToken cancellationToken)
      {
        var userFromDb = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername(), 
                cancellationToken);
        
        var user = new User
        {
            Username = userFromDb.UserName,
            Image = null,
            Token = jwtGenerator.CreateToken(userFromDb),
            Bio = userFromDb.Bio,
            Email = userFromDb.Email
        };

        return user;
      }
    }
  }
}